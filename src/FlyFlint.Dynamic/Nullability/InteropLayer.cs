////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Reflection;

// Nullability checking API available only .NET 6.0.
// This directory contains interoperability code of Nullability checking API,
// backported source code from Nullability.Source nuget package
// originated by Simon Cropp.
// https://github.com/SimonCropp/NullabilityInfo

namespace FlyFlint.Nullability
{
    internal static class InteropLayer
    {
        public static Type GetAttributeType(
            this CustomAttributeData attributeData) =>
            attributeData.Constructor.DeclaringType!;
#if NET35
        public static IList<CustomAttributeData> GetCustomAttributesData(
            this MemberInfo member) =>
            CustomAttributeData.GetCustomAttributes(member);

        public static IList<CustomAttributeData> GetCustomAttributesData(
            this ParameterInfo parameter) =>
            CustomAttributeData.GetCustomAttributes(parameter);

        public static IList<CustomAttributeData> GetCustomAttributesData(
            this Module module) =>
            CustomAttributeData.GetCustomAttributes(module);

        public static bool HasFlag<T>(this T value, T flag)
            where T : Enum
        {
            switch (Convert.GetTypeCode(value))
            {
                case TypeCode.Byte:
                    return ((byte)(object)value & (byte)(object)flag) != 0;
                case TypeCode.Int16:
                    return ((short)(object)value & (short)(object)flag) != 0;
                case TypeCode.Int32:
                    return ((int)(object)value & (int)(object)flag) != 0;
                case TypeCode.Int64:
                    return ((long)(object)value & (long)(object)flag) != 0;
                case TypeCode.SByte:
                    return ((sbyte)(object)value & (sbyte)(object)flag) != 0;
                case TypeCode.UInt16:
                    return ((ushort)(object)value & (ushort)(object)flag) != 0;
                case TypeCode.UInt32:
                    return ((uint)(object)value & (uint)(object)flag) != 0;
                case TypeCode.UInt64:
                    return ((ulong)(object)value & (ulong)(object)flag) != 0;
                default:
                    throw new InvalidOperationException();
            }
        }
#endif
    }
}
