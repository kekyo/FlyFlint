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
using System.Data.Common;

namespace FlyFlint.Internal.Dynamic
{
    internal static class DynamicDataAccessor
    {
        public static object? GetValue(
            IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata, Type targetType)
        {
            if (reader.IsDBNull(metadata.Index))
            {
                return null;
            }

            var value = reader.GetValue(metadata.Index);
            if (targetType == metadata.Type)
            {
                return value;
            }
            else if (targetType.IsEnum)
            {
                return GetEnumValue(targetType, value, fp);
            }
            else
            {
                return Convert.ChangeType(value, targetType, fp);
            }
        }

        /////////////////////////////////////////////////////////////////////////////

        private static readonly Dictionary<Type, (string[], Enum[])> enumMetadataDict =
            new Dictionary<Type, (string[], Enum[])>();

        private static bool TryToEnumValue(Type enumType, string strValue, out object? value)
        {
            (string[] fieldNames, Enum[] fieldValues) enumMetadata;
            lock (enumMetadataDict)
            {
                if (!enumMetadataDict.TryGetValue(enumType, out enumMetadata))
                {
                    enumMetadata = QueryHelper.GetSortedEnumValues(enumType);
                    enumMetadataDict.Add(enumType, enumMetadata);
                }
            }

            var index = Array.BinarySearch(enumMetadata.fieldNames, strValue);
            if (index >= 1)
            {
                value = enumMetadata.fieldValues[index];
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        public static object GetEnumValue(Type enumType, object value, IFormatProvider fp)
        {
            // Makes flexible enum type conversion.
            var ut = Enum.GetUnderlyingType(enumType);
            if (ut == value.GetType())
            {
                return Enum.ToObject(enumType, value);
            }
            else if (value is string sv && TryToEnumValue(enumType, sv, out var ov))
            {
                return ov!;
            }
            else
            {
                return Enum.ToObject(enumType, Convert.ChangeType(value, ut, fp));
            }
        }
    }
}
