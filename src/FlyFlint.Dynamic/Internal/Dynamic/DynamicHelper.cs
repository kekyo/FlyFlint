////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Context;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace FlyFlint.Internal.Dynamic
{
    internal static class DynamicHelper
    {
        public delegate object? MemberGetter<T>(ref T record, ConversionContext cc);
        public delegate void MemberSetter<T>(ref T record, object? value);

        public struct Metadata<TAccessor>
            where TAccessor : Delegate
        {
            public readonly string FieldName;
            public readonly Type FieldType;
            public readonly TAccessor Accessor;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
            public Metadata(string fieldName, Type fieldType, TAccessor accessor)
            {
                this.FieldName = fieldName;
                this.FieldType = fieldType;
                this.Accessor = accessor;
            }
        }

        private static class GetterMetadata<T>
        {
            public static readonly Metadata<MemberGetter<T>>[] Members;

            static GetterMetadata()
            {
                Members = typeof(T).
                    GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).
                    Select(m => GetTargetMember(m)).
                    Where(entry => entry != null).
                    Select(entry => entry!.Value).
                    ToArray()!;
            }

            private static Metadata<MemberGetter<T>>? GetTargetMember(
                MemberInfo member)
            {
                if (member is FieldInfo fi)
                {
                    if (fi.GetCustomAttributes(typeof(QueryFieldAttribute), true) is QueryFieldAttribute[] attributes &&
                        attributes.Length >= 1)
                    {
                        var name = attributes[0].Name;
                        return new Metadata<MemberGetter<T>>(
                            QueryHelper.IsNullOrWhiteSpace(name) ? fi.Name : name!,
                            fi.FieldType,
                            (MemberGetter<T>)DynamicMemberAccessor.CreateDirectGetter(fi).
                                CreateDelegate(typeof(MemberGetter<T>)));
                    }
                    else if (fi.IsPublic &&
                        !fi.IsDefined(typeof(QueryIgnoreAttribute), true))
                    {
                        return new Metadata<MemberGetter<T>>(
                            fi.Name,
                            fi.FieldType,
                            (MemberGetter<T>)DynamicMemberAccessor.CreateDirectGetter(fi).
                                CreateDelegate(typeof(MemberGetter<T>)));
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
                            return new Metadata<MemberGetter<T>>(
                                QueryHelper.IsNullOrWhiteSpace(name) ? pi.Name : name!,
                                pi.PropertyType,
                                (MemberGetter<T>)DynamicMemberAccessor.CreateDirectGetter(pi).
                                    CreateDelegate(typeof(MemberGetter<T>)));
                        }
                        else if (getter.IsPublic &&
                            !pi.IsDefined(typeof(QueryIgnoreAttribute), true))
                        {
                            return new Metadata<MemberGetter<T>>(
                                pi.Name,
                                pi.PropertyType,
                                (MemberGetter<T>)DynamicMemberAccessor.CreateDirectGetter(pi).
                                    CreateDelegate(typeof(MemberGetter<T>)));
                        }
                    }
                }

                return null;
            }
        }

        private static class SetterMetadata<T>
        {
            public static readonly Metadata<MemberSetter<T>>[] Members;

            static SetterMetadata()
            {
                Members = typeof(T).
                    GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).
                    Select(m => GetTargetMember(m)).
                    Where(entry => entry != null).
                    Select(entry => entry!.Value).
                    ToArray()!;
            }

            private static Metadata<MemberSetter<T>>? GetTargetMember(
                MemberInfo member)
            {
                if (member is FieldInfo fi)
                {
                    if (!fi.IsInitOnly)
                    {
                        if (fi.GetCustomAttributes(typeof(QueryFieldAttribute), true) is QueryFieldAttribute[] attributes &&
                            attributes.Length >= 1)
                        {
                            var name = attributes[0].Name;
                            return new Metadata<MemberSetter<T>>(
                                QueryHelper.IsNullOrWhiteSpace(name) ? fi.Name : name!,
                                fi.FieldType,
                                (MemberSetter<T>)DynamicMemberAccessor.CreateDirectSetter(fi).
                                    CreateDelegate(typeof(MemberSetter<T>)));
                        }
                        else if (fi.IsPublic &&
                            !fi.IsDefined(typeof(QueryIgnoreAttribute), true))
                        {
                            return new Metadata<MemberSetter<T>>(
                                fi.Name,
                                fi.FieldType,
                                (MemberSetter<T>)DynamicMemberAccessor.CreateDirectSetter(fi).
                                    CreateDelegate(typeof(MemberSetter<T>)));
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
                            return new Metadata<MemberSetter<T>>(
                                QueryHelper.IsNullOrWhiteSpace(name) ? pi.Name : name!,
                                pi.PropertyType,
                                (MemberSetter<T>)DynamicMemberAccessor.CreateDirectSetter(pi).
                                    CreateDelegate(typeof(MemberSetter<T>)));
                        }
                        else if (setter.IsPublic &&
                            !pi.IsDefined(typeof(QueryIgnoreAttribute), true))
                        {
                            return new Metadata<MemberSetter<T>>(
                                pi.Name,
                                pi.PropertyType,
                                (MemberSetter<T>)DynamicMemberAccessor.CreateDirectSetter(pi).
                                    CreateDelegate(typeof(MemberSetter<T>)));
                        }
                    }
                }

                return null;
            }
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Metadata<MemberGetter<T>>[] GetGetterMetadataList<T>() =>
            GetterMetadata<T>.Members;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Metadata<MemberSetter<T>>[] GetSetterMetadataList<T>() =>
            SetterMetadata<T>.Members;
    }
}
