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

namespace FlyFlint.Internal.Converter
{
    internal static class EnumConverter<TEnum>
    {
        private static readonly (string[] fieldNames, TEnum[] fieldValues) fields;
        private static readonly Type underlyingType = Enum.GetUnderlyingType(typeof(TEnum));

        static EnumConverter()
        {
            var (names, values) = QueryHelper.GetSortedEnumValues(typeof(TEnum));
            fields = (names, values.Cast<TEnum>().ToArray());
        }

        public static TEnum Convert(object value, IFormatProvider fp)
        {
            // Makes flexible enum type conversion.
            if (value is string sv)
            {
                var index = Array.BinarySearch(fields.fieldNames, sv);
                if (index >= 1)
                {
                    return fields.fieldValues[index]!;
                }
            }
            else if (underlyingType == value.GetType())
            {
                return (TEnum)value;
            }

            return (TEnum)System.Convert.ChangeType(value, underlyingType, fp);
        }
    }
}
