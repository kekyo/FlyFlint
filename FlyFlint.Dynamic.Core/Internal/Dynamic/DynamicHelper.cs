////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace FlyFlint.Internal.Dynamic
{
    internal static class DynamicHelper
    {
        public delegate object? MemberGetter<T>(ref T element);
        public delegate void MemberSetter<T>(ref T element, object? value);

        private static class GetterMetadata<T>
        {
            public static readonly (string name, Type type, MemberGetter<T> getter)[] Members;

            static GetterMetadata()
            {
                var requiredDataMemberAttribute =
                    typeof(T).GetCustomAttributes(typeof(DataContractAttribute), true).Length >= 1;
                Members = typeof(T).
                    GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).
                    Select(m => GetTargetMember(m, requiredDataMemberAttribute)).
                    Where(entry => entry != null).
                    Select(entry => entry!.Value).
                    ToArray()!;
            }

            private static (string name, Type type, MemberGetter<T> getter)? GetTargetMember(
                MemberInfo member, bool requiredDataMemberAttribute)
            {
                if (member is FieldInfo fi)
                {
                    if (fi.GetCustomAttributes(typeof(DataMemberAttribute), true) is DataMemberAttribute[] attributes &&
                        attributes.Length >= 1)
                    {
                        var name = attributes[0].Name;
                        return (string.IsNullOrWhiteSpace(name) ? fi.Name : name,
                            fi.FieldType,
                            (MemberGetter<T>)DynamicMemberAccessor.CreateDirectGetter(fi).
                                CreateDelegate(typeof(MemberGetter<T>)));
                    }
                    else if (!requiredDataMemberAttribute && fi.IsPublic)
                    {
                        return (fi.Name,
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
                        if (pi.GetCustomAttributes(typeof(DataMemberAttribute), true) is DataMemberAttribute[] attributes &&
                            attributes.Length >= 1)
                        {
                            var name = attributes[0].Name;
                            return (string.IsNullOrWhiteSpace(name) ? pi.Name : name,
                                pi.PropertyType,
                                (MemberGetter<T>)DynamicMemberAccessor.CreateDirectGetter(pi).
                                    CreateDelegate(typeof(MemberGetter<T>)));
                        }
                        else if (!requiredDataMemberAttribute && getter.IsPublic)
                        {
                            return (pi.Name,
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
            public static readonly (string name, Type type, MemberSetter<T> setter)[] Members;

            static SetterMetadata()
            {
                var requiredDataMemberAttribute =
                    typeof(T).GetCustomAttributes(typeof(DataContractAttribute), true).Length >= 1;
                Members = typeof(T).
                    GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).
                    Select(m => GetTargetMember(m, requiredDataMemberAttribute)).
                    Where(entry => entry != null).
                    Select(entry => entry!.Value).
                    ToArray()!;
            }

            private static (string name, Type type, MemberSetter<T> setter)? GetTargetMember(
                MemberInfo member, bool requiredDataMemberAttribute)
            {
                if (member is FieldInfo fi)
                {
                    if (!fi.IsInitOnly)
                    {
                        if (fi.GetCustomAttributes(typeof(DataMemberAttribute), true) is DataMemberAttribute[] attributes &&
                            attributes.Length >= 1)
                        {
                            var name = attributes[0].Name;
                            return (string.IsNullOrWhiteSpace(name) ? fi.Name : name,
                                fi.FieldType,
                                (MemberSetter<T>)DynamicMemberAccessor.CreateDirectSetter(fi).
                                    CreateDelegate(typeof(MemberSetter<T>)));
                        }
                        else if (!requiredDataMemberAttribute && fi.IsPublic)
                        {
                            return (fi.Name,
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
                        if (pi.GetCustomAttributes(typeof(DataMemberAttribute), true) is DataMemberAttribute[] attributes &&
                            attributes.Length >= 1)
                        {
                            var name = attributes[0].Name;
                            return (string.IsNullOrWhiteSpace(name) ? pi.Name : name,
                                pi.PropertyType,
                                (MemberSetter<T>)DynamicMemberAccessor.CreateDirectSetter(pi).
                                    CreateDelegate(typeof(MemberSetter<T>)));
                        }
                        else if (!requiredDataMemberAttribute && setter.IsPublic)
                        {
                            return (pi.Name,
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
        public static (string name, Type type, MemberGetter<T> getter)[] GetGetterMetadataList<T>() =>
            GetterMetadata<T>.Members;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static (string name, Type type, MemberSetter<T> setter)[] GetSetterMetadataList<T>() =>
            SetterMetadata<T>.Members;
    }
}
