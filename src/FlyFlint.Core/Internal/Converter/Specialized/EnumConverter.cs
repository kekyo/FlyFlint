////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.Linq;

namespace FlyFlint.Internal.Converter.Specialized
{
    internal abstract class EnumConverter<TEnum>
    {
        public static readonly Func<object, IFormatProvider, TEnum> convert;

        static EnumConverter()
        {
            if (typeof(TEnum).IsEnum)
            {
                var converter = new NonNullableEnumConverter<TEnum>();
                convert = converter.Convert;
            }
            else
            {
                Debug.Assert(Nullable.GetUnderlyingType(typeof(TEnum))?.IsEnum ?? false);

                var converter = new NullableEnumConverter<TEnum>();
                convert = converter.Convert;
            }
        }

        public abstract TEnum Convert(object value, IFormatProvider fp);
    }

    internal sealed class NonNullableEnumConverter<TEnum> : EnumConverter<TEnum>
    {
        private static readonly Type enumType = typeof(TEnum);
        private static readonly Type underlyingType = Enum.GetUnderlyingType(typeof(TEnum));
        private static readonly string[] fieldNames;
        private static readonly TEnum[] fieldNameValues;

        static NonNullableEnumConverter()
        {
            Debug.Assert(enumType.IsEnum);

            fieldNames = Enum.GetNames(enumType);
            fieldNameValues = Enum.GetValues(enumType).
                Cast<TEnum>().
                ToArray();

            Array.Sort(fieldNames, fieldNameValues);
        }

        public override TEnum Convert(object value, IFormatProvider fp)
        {
            if (value is string sv)
            {
                var index = Array.BinarySearch(fieldNames, sv);
                if (index >= 0)
                {
                    return fieldNameValues[index]!;
                }
            }
            else if (value is TEnum ev)
            {
                return ev;
            }
            else if (underlyingType == value.GetType())
            {
                // CLR can unbox directly from underlying type.
                return (TEnum)value;
            }

            return (TEnum)System.Convert.ChangeType(value, underlyingType, fp);
        }
    }

    internal sealed class NullableEnumConverter<TNullableEnum> : EnumConverter<TNullableEnum>
    {
        private static readonly Type enumType = Nullable.GetUnderlyingType(typeof(TNullableEnum))!;
        private static readonly Type underlyingType;
        private static readonly string[] fieldNames;
        private static readonly TNullableEnum[] fieldNameValues;

        static NullableEnumConverter()
        {
            Debug.Assert(enumType?.IsEnum ?? false);

            underlyingType = Enum.GetUnderlyingType(enumType);

            fieldNames = Enum.GetNames(enumType);
            fieldNameValues = Enum.GetValues(enumType).
                Cast<TNullableEnum>().
                ToArray();

            Array.Sort(fieldNames, fieldNameValues);
        }

        public override TNullableEnum Convert(object value, IFormatProvider fp)
        {
            if (value is string sv)
            {
                var index = Array.BinarySearch(fieldNames, sv);
                if (index >= 1)
                {
                    return fieldNameValues[index]!;
                }
            }
            else if (value is TNullableEnum ev)
            {
                return ev;
            }
            else if (enumType == value.GetType())
            {
                return (TNullableEnum)value;
            }
            else if (underlyingType == value.GetType())
            {
                return (TNullableEnum)Enum.ToObject(enumType, value);
            }

            return (TNullableEnum)Enum.ToObject(
                enumType,
                System.Convert.ChangeType(value, underlyingType, fp));
        }
    }
}
