////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Internal.Static;
using FlyFlint.Nullability;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace FlyFlint.Internal.Dynamic
{
    internal struct DynamicMemberMetadata
    {
        public readonly MemberInfo Member;
        public readonly string Name;
        public readonly Type Type;

        public DynamicMemberMetadata(MemberInfo member, string name, Type type)
        {
            this.Member = member;
            this.Name = name;
            this.Type = type;
        }
    }

    internal static class Utilities
    {
        private sealed class TypeKey : IEquatable<TypeKey?>
        {
            public readonly Type Type;
            public readonly bool IsNullable;

            public TypeKey(Type type, bool isNullable)
            {
                this.Type = type;
                this.IsNullable = isNullable;
            }

            public override int GetHashCode() =>
                this.Type.GetHashCode() ^ this.IsNullable.GetHashCode();

            public bool Equals(TypeKey? other) =>
                other is TypeKey o &&
                this.Type.Equals(o.Type) &&
                this.IsNullable == o.IsNullable;

            public override string ToString() =>
                $"{this.Type.FullName}{(!this.Type.IsValueType ? this.IsNullable ? "?" : "" : "")}";
        }

        ////////////////////////////////////////////////////////////////////////////

        private static readonly Dictionary<TypeKey, MethodInfo> getValueMethods =
            typeof(StaticRecordInjectionContext).
            GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).
            Where(m => m.IsPublic && !m.IsStatic && !m.IsGenericMethod && m.Name.StartsWith("Get")).
            ToDictionary(m => new TypeKey(m.ReturnType, m.Name.StartsWith("GetNullable")));

        private static readonly MethodInfo getObjRefValueMethod =
            typeof(StaticRecordInjectionContext).
            GetMethod("GetObjRefValue")!;
        private static readonly MethodInfo getValueMethod =
            typeof(StaticRecordInjectionContext).
            GetMethod("GetValue")!;
        private static readonly MethodInfo getNullableObjRefValueMethod =
            typeof(StaticRecordInjectionContext).
            GetMethod("GetNullableObjRefValue")!;
        private static readonly MethodInfo getNullableValueMethod =
            typeof(StaticRecordInjectionContext).
            GetMethod("GetNullableValue")!;

        private static readonly FieldInfo staticRecordInjectionContextCurrentOffsetField =
            typeof(StaticRecordInjectionContext).GetField("CurrentOffset")!;
        private static readonly FieldInfo staticRecordInjectionContextIsAvailableField =
            typeof(StaticRecordInjectionContext).GetField("IsAvailable")!;

        ////////////////////////////////////////////////////////////////////////////

        private static DynamicMemberMetadata? GetTargetSettingMemberMetadata(MemberInfo member)
        {
            if (member is FieldInfo fi)
            {
                if (!fi.IsInitOnly)
                {
                    if (fi.GetCustomAttributes(typeof(QueryFieldAttribute), true) is QueryFieldAttribute[] attributes &&
                        attributes.Length >= 1)
                    {
                        var name = attributes[0].Name;
                        return new DynamicMemberMetadata(
                            fi, QueryHelper.IsNullOrWhiteSpace(name) ? fi.Name : name!, fi.FieldType);
                    }
                    else if (fi.IsPublic &&
                        !fi.IsDefined(typeof(QueryIgnoreAttribute), true))
                    {
                        return new DynamicMemberMetadata(
                            fi, fi.Name, fi.FieldType);
                    }
                }
            }
            else if (member is PropertyInfo pi)
            {
                var setter = pi.GetSetMethod(true);
                if (pi.CanWrite && setter != null && pi.GetIndexParameters().Length == 0)
                {
                    if (pi.GetCustomAttributes(typeof(QueryFieldAttribute), true) is QueryFieldAttribute[] attributes &&
                        attributes.Length >= 1)
                    {
                        var name = attributes[0].Name;
                        return new DynamicMemberMetadata(
                            pi, QueryHelper.IsNullOrWhiteSpace(name) ? pi.Name : name!, pi.PropertyType);
                    }
                    else if (setter.IsPublic &&
                        !pi.IsDefined(typeof(QueryIgnoreAttribute), true))
                    {
                        return new DynamicMemberMetadata(
                            pi, pi.Name, pi.PropertyType);
                    }
                }
            }
            return null;
        }

        private static bool IsNullableForMember(
            MemberInfo targetMember, Type memberType)
        {
            if (memberType.IsValueType)
            {
                return Nullable.GetUnderlyingType(memberType) != null;
            }
            else
            {
                var context = new NullabilityInfoContext();
                var nullabilityInfo = targetMember is FieldInfo f ?
                    context.Create(f) :
                    context.Create((PropertyInfo)targetMember);
                return nullabilityInfo.ReadState != NullabilityState.NotNull;
            }
        }

        public static Type DereferenceWhenNullableType(Type type) =>
            (type.IsValueType && type.IsGenericType &&
             type.GetGenericTypeDefinition() == typeof(Nullable<>)) ?
                type.GetGenericArguments()[0] :
                type;

        ////////////////////////////////////////////////////////////////////////////

        private static MethodInfo GetValueMethod(MemberInfo targetMember, Type memberType)
        {
            var isNullable = IsNullableForMember(targetMember, memberType);
            var key = new TypeKey(memberType, isNullable);

            if (getValueMethods.TryGetValue(key, out var gvm))
            {
                return gvm;
            }
            else
            {
                if (!isNullable && !memberType.IsValueType)
                {
                    return getObjRefValueMethod.MakeGenericMethod(
                        DereferenceWhenNullableType(memberType));
                }
                else if (!isNullable && memberType.IsValueType)
                {
                    return getValueMethod.MakeGenericMethod(
                        DereferenceWhenNullableType(memberType));
                }
                else if (isNullable && !memberType.IsValueType)
                {
                    return getNullableObjRefValueMethod.MakeGenericMethod(
                        DereferenceWhenNullableType(memberType));
                }
                else
                {
                    return getNullableValueMethod.MakeGenericMethod(
                        DereferenceWhenNullableType(memberType));
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        public static DynamicMemberMetadata[] GetTargetMembers<TRecord>() =>
            typeof(TRecord).
            Traverse(type => type.BaseType).
            SelectMany(type =>
                type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).
                Collect(member => GetTargetSettingMemberMetadata(member))).
            Select(entry => entry!.Value).
            DistinctBy(entry => entry.Name).
            ToArray()!;

        ////////////////////////////////////////////////////////////////////////////

        public static TDelegate CreateSetter<TDelegate, TRecord>()
            where TDelegate : Delegate
        {
            var targetType = typeof(TRecord);
            var targetMembers = GetTargetMembers<TRecord>();

            var fullName = $"MemberAccessor.{targetType.Namespace}.{targetType.Name}.setter";

            var dm = new DynamicMethod(
                fullName,
                typeof(void),
                new[]
                {
                    typeof(StaticRecordInjectionContext),
                    targetType.IsValueType ?
                        targetType.MakeByRefType() :   // ref TRecord record
                        targetType                     // TRecord record
                },
                true);

            var ig = dm.GetILGenerator();

            ////////////////////////////////////////////////////////////////
            // var offset = context.CurrentOffset;

            if (!targetType.IsValueType)
            {
                ig.DeclareLocal(typeof(int));

                ig.Emit(OpCodes.Ldarg_0);
                ig.Emit(
                    OpCodes.Ldfld,
                    staticRecordInjectionContextCurrentOffsetField);
                ig.Emit(OpCodes.Stloc_0);
            }

            ////////////////////////////////////////////////////////////////
            // var isAvailable = context.IsAvailable;

            ig.Emit(OpCodes.Ldarg_0);
            ig.Emit(
                OpCodes.Ldfld,
                staticRecordInjectionContextIsAvailableField);

            ////////////////////////////////////////////////////////////////

            // First one is dummy.
            var branchTargetLabels =
                Enumerable.Range(0, targetMembers.Length + 1).
                Select(index => ig.DefineLabel()).
                ToArray();

            for (var metadataIndex = 0; metadataIndex < targetMembers.Length; metadataIndex++)
            {
                ig.MarkLabel(branchTargetLabels[metadataIndex]);

                // if (isAvailable[metadataIndex + offset])

                ig.Emit(OpCodes.Dup);
                ig.Emit(
                    OpCodes.Ldc_I4_S,
                    (sbyte)metadataIndex);

                if (!targetType.IsValueType)
                {
                    ig.Emit(OpCodes.Ldloc_0);
                    ig.Emit(OpCodes.Add);
                }

                ig.Emit(OpCodes.Ldelem_U1);

                ////////////////////////

                // Set branch target
                ig.Emit(
                    OpCodes.Brfalse_S,
                    branchTargetLabels[metadataIndex + 1]);

                ////////////////////////

                var targetMember = targetMembers[metadataIndex];

                var memberType = targetMember.Type;
                var getValueMethod = GetValueMethod(
                    targetMember.Member, memberType);

                ig.Emit(OpCodes.Ldarg_1);
                ig.Emit(OpCodes.Ldarg_0);
                ig.Emit(OpCodes.Ldc_I4_S, (sbyte)metadataIndex);

                if (!targetType.IsValueType)
                {
                    ig.Emit(OpCodes.Ldloc_0);
                    ig.Emit(OpCodes.Add);
                }

                ig.Emit(OpCodes.Call, getValueMethod);

                if (targetMember.Member is FieldInfo f)
                {
                    ig.Emit(OpCodes.Stfld, f);
                }
                else
                {
                    Debug.Assert(targetMember.Member is PropertyInfo);

                    var p = (PropertyInfo)targetMember.Member;
                    var sm = p.GetSetMethod(true);
                    Debug.Assert(sm != null);

                    ig.Emit(
                        sm!.IsVirtual ? OpCodes.Callvirt : OpCodes.Call,
                        sm);
                }
            }

            // Set last label
            ig.MarkLabel(branchTargetLabels[targetMembers.Length]);

            // Remove isAvailable.
            ig.Emit(OpCodes.Pop);

            ig.Emit(OpCodes.Ret);

            var dlg = typeof(TDelegate);
            return (TDelegate)dm.CreateDelegate(dlg);
        }
    }
}
