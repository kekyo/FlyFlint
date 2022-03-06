////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Internal.Static;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace FlyFlint.Internal.Dynamic
{
    internal static class DynamicParameterExtractionHelper
    {
        private static readonly MethodInfo setByRefParameterMethod =
            typeof(StaticParameterExtractionContext).
            GetMethod("SetByRefParameter")!;
        private static readonly MethodInfo setByValParameterMethod =
            typeof(StaticParameterExtractionContext).
            GetMethod("SetByValParameter")!;

        ////////////////////////////////////////////////////////////////////////////

        private static DynamicMemberMetadata? GetTargetGettingMemberMetadata(MemberInfo member)
        {
            if (member is FieldInfo fi)
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
            else if (member is PropertyInfo pi)
            {
                var getter = pi.GetGetMethod(true);
                if (pi.CanRead && getter != null && pi.GetIndexParameters().Length == 0)
                {
                    if (pi.GetCustomAttributes(typeof(QueryFieldAttribute), true) is QueryFieldAttribute[] attributes &&
                        attributes.Length >= 1)
                    {
                        var name = attributes[0].Name;
                        return new DynamicMemberMetadata(
                            pi, QueryHelper.IsNullOrWhiteSpace(name) ? pi.Name : name!, pi.PropertyType);
                    }
                    else if (getter.IsPublic &&
                        !pi.IsDefined(typeof(QueryIgnoreAttribute), true))
                    {
                        return new DynamicMemberMetadata(
                            pi, pi.Name, pi.PropertyType);
                    }
                }
            }
            return null;
        }

        ////////////////////////////////////////////////////////////////////////////

        public static DynamicMemberMetadata[] GetTargetGettingMembers<TParameters>() =>
            typeof(TParameters).
            Traverse(type => type.BaseType).
            SelectMany(type =>
                type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).
                Collect(member => GetTargetGettingMemberMetadata(member))).   // TODO:
            Select(entry => entry!.Value).
            DistinctBy(entry => entry.Name).
            ToArray()!;

        ////////////////////////////////////////////////////////////////////////////

        public static TDelegate CreateParameterExtractor<TDelegate, TParameters>()
            where TDelegate : Delegate
        {
            var targetType = typeof(TParameters);
            var targetMembers = GetTargetGettingMembers<TParameters>();

            var fullName = $"ParameterExtractor.{targetType.Namespace}.{targetType.Name}";

            var dm = new DynamicMethod(
                fullName,
                typeof(void),
                new[]
                {
                    typeof(StaticParameterExtractionContext),
                    targetType.IsValueType ?
                        targetType.MakeByRefType() :   // ref TRecord record
                        targetType                     // TRecord record
                },
                true);

            var ig = dm.GetILGenerator();

            ////////////////////////////////////////////////////////////////

            for (var metadataIndex = 0; metadataIndex < targetMembers.Length; metadataIndex++)
            {
                var targetMember = targetMembers[metadataIndex];

                ig.Emit(OpCodes.Ldarg_0);
                ig.Emit(
                    OpCodes.Ldstr,
                    targetMember.Name);

                if (targetMember.Member is FieldInfo f)
                {
                    ig.Emit(OpCodes.Ldarg_1);
                    ig.Emit(OpCodes.Ldflda, f);

                    ig.Emit(
                        OpCodes.Call,
                        setByRefParameterMethod.MakeGenericMethod(targetMember.Type));
                }
                else
                {
                    Debug.Assert(targetMember.Member is PropertyInfo);

                    var p = (PropertyInfo)targetMember.Member;
                    var gm = p.GetGetMethod(true);
                    Debug.Assert(gm != null);

                    ig.Emit(OpCodes.Ldarg_1);
                    ig.Emit(
                        OpCodes.Call,
                        gm!);

                    ig.Emit(
                        OpCodes.Call,
                        setByValParameterMethod.MakeGenericMethod(targetMember.Type));
                }
            }

            ig.Emit(OpCodes.Ret);

            return (TDelegate)dm.CreateDelegate(typeof(TDelegate));
        }
    }
}
