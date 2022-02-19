////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Context;
using FlyFlint.Internal.Converter.Specialized;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace FlyFlint.Internal.Converter
{
    internal abstract class InternalValueConverter<TValue>
    {
        public static readonly InternalValueConverter<TValue> converter;

        static InternalValueConverter()
        {
            if (typeof(TValue) == typeof(bool))
            {
                converter = new BooleanValueConverter<TValue>();
            }
            else if (typeof(TValue) == typeof(byte))
            {
                converter = new ByteValueConverter<TValue>();
            }
            else if (typeof(TValue) == typeof(short))
            {
                converter = new Int16ValueConverter<TValue>();
            }
            else if (typeof(TValue) == typeof(int))
            {
                converter = new Int32ValueConverter<TValue>();
            }
            else if (typeof(TValue) == typeof(long))
            {
                converter = new Int64ValueConverter<TValue>();
            }
            else if (typeof(TValue) == typeof(float))
            {
                converter = new SingleValueConverter<TValue>();
            }
            else if (typeof(TValue) == typeof(double))
            {
                converter = new DoubleValueConverter<TValue>();
            }
            else if (typeof(TValue) == typeof(decimal))
            {
                converter = new DecimalValueConverter<TValue>();
            }
            else if (typeof(TValue) == typeof(Guid))
            {
                converter = new GuidValueConverter<TValue>();
            }
            else if (typeof(TValue) == typeof(DateTime))
            {
                converter = new DateTimeValueConverter<TValue>();
            }
            else if (typeof(TValue) == typeof(char))
            {
                converter = new CharValueConverter<TValue>();
            }
            else if (typeof(TValue) == typeof(string))
            {
                converter = new NullableStringValueConverter<TValue>();
            }
            else if (typeof(TValue).IsEnum)
            {
                converter = new EnumValueConverter<TValue>();
            }
            else if (typeof(TValue) == typeof(byte[]))
            {
                converter = new NullableByteArrayValueConverter<TValue>();
            }
            else if (typeof(TValue) == typeof(bool?))
            {
                converter = new NullableBooleanValueConverter<TValue>();
            }
            else if (typeof(TValue) == typeof(byte?))
            {
                converter = new NullableByteValueConverter<TValue>();
            }
            else if (typeof(TValue) == typeof(short?))
            {
                converter = new NullableInt16ValueConverter<TValue>();
            }
            else if (typeof(TValue) == typeof(int?))
            {
                converter = new NullableInt32ValueConverter<TValue>();
            }
            else if (typeof(TValue) == typeof(long?))
            {
                converter = new NullableInt64ValueConverter<TValue>();
            }
            else if (typeof(TValue) == typeof(float?))
            {
                converter = new NullableSingleValueConverter<TValue>();
            }
            else if (typeof(TValue) == typeof(double?))
            {
                converter = new NullableDoubleValueConverter<TValue>();
            }
            else if (typeof(TValue) == typeof(decimal?))
            {
                converter = new NullableDecimalValueConverter<TValue>();
            }
            else if (typeof(TValue) == typeof(Guid?))
            {
                converter = new NullableGuidValueConverter<TValue>();
            }
            else if (typeof(TValue) == typeof(DateTime?))
            {
                converter = new NullableDateTimeValueConverter<TValue>();
            }
            else if (typeof(TValue) == typeof(char?))
            {
                converter = new NullableCharValueConverter<TValue>();
            }
            else if (Nullable.GetUnderlyingType(typeof(TValue))?.IsEnum ?? false)
            {
                converter = new NullableEnumValueConverter<TValue>();
            }
            else
            {
                converter = new InvalidOperationExceptionConverter<TValue>();
            }
        }

        public abstract TValue ConvertTo(ConversionContext context, object? value);
        public abstract TValue UnsafeConvertTo(ConversionContext context, object value);
    }

    internal sealed class InvalidOperationExceptionConverter<T> : InternalValueConverter<T>
    {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override T ConvertTo(ConversionContext context, object? value) =>
            throw new InvalidOperationException($"Could not convert to {typeof(T).FullName}");
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override T UnsafeConvertTo(ConversionContext context, object value) =>
            throw new InvalidOperationException($"Could not convert to {typeof(T).FullName}");
    }

    /////////////////////////////////////////////////////////////////////////////

    internal abstract class InternalValueConverterBase<TValue, TR> : InternalValueConverter<TValue>
    {
        static InternalValueConverterBase() =>
            Debug.Assert(typeof(TValue) == typeof(TR));

        protected readonly Func<object, IFormatProvider, TValue> convert;

        protected InternalValueConverterBase(Func<object, IFormatProvider, TR> convert) =>
            this.convert = (Func<object, IFormatProvider, TValue>)(Delegate)convert;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override sealed TValue ConvertTo(ConversionContext context, object? value) =>
            value is null ? throw new NullReferenceException($"Could not convert from null to {typeof(TValue).FullName}.") :
            value is TValue v ? v :
            convert(value, context.FormatProvider);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override sealed TValue UnsafeConvertTo(ConversionContext context, object value) =>
            value is TValue v ? v :
            convert(value, context.FormatProvider);
    }

    internal sealed class BooleanValueConverter<T> : InternalValueConverterBase<T, bool>
    {
        public BooleanValueConverter() :
            base(System.Convert.ToBoolean)
        { }
    }

    internal sealed class ByteValueConverter<T> : InternalValueConverterBase<T, byte>
    {
        public ByteValueConverter() :
            base(System.Convert.ToByte)
        { }
    }

    internal sealed class Int16ValueConverter<T> : InternalValueConverterBase<T, short>
    {
        public Int16ValueConverter() :
            base(System.Convert.ToInt16)
        { }
    }

    internal sealed class Int32ValueConverter<T> : InternalValueConverterBase<T, int>
    {
        public Int32ValueConverter() :
            base(System.Convert.ToInt32)
        { }
    }

    internal sealed class Int64ValueConverter<T> : InternalValueConverterBase<T, long>
    {
        public Int64ValueConverter() :
            base(System.Convert.ToInt64)
        { }
    }

    internal sealed class SingleValueConverter<T> : InternalValueConverterBase<T, float>
    {
        public SingleValueConverter() :
            base(System.Convert.ToSingle)
        { }
    }

    internal sealed class DoubleValueConverter<T> : InternalValueConverterBase<T, double>
    {
        public DoubleValueConverter() :
            base(System.Convert.ToDouble)
        { }
    }

    internal sealed class DecimalValueConverter<T> : InternalValueConverterBase<T, decimal>
    {
        public DecimalValueConverter() :
            base(System.Convert.ToDecimal)
        { }
    }

    internal sealed class GuidValueConverter<T> : InternalValueConverterBase<T, Guid>
    {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static Guid ToGuid(object value, IFormatProvider fp) =>
            value is byte[] blob ?
                new Guid(blob) :   // vv Couldn't convert from byte array.
                new Guid(System.Convert.ToString(value, fp)!);

        public GuidValueConverter() :
            base(ToGuid)
        { }
    }

    internal sealed class DateTimeValueConverter<T> : InternalValueConverterBase<T, DateTime>
    {
        public DateTimeValueConverter() :
            base(System.Convert.ToDateTime)
        { }
    }

    internal sealed class CharValueConverter<T> : InternalValueConverterBase<T, char>
    {
        public CharValueConverter() :
            base(System.Convert.ToChar)
        { }
    }

    internal sealed class EnumValueConverter<T> : InternalValueConverterBase<T, T>
    {
        static EnumValueConverter() =>
            Debug.Assert(typeof(T).IsEnum);

        public EnumValueConverter() :
            base(EnumConverter<T>.convertTo)
        { }
    }

    /////////////////////////////////////////////////////////////////////////////

    internal abstract class InternalNullableValueConverterBase<TValue, TNullable, TUnderlying> : InternalValueConverter<TValue>
    {
        static InternalNullableValueConverterBase()
        {
            Debug.Assert(typeof(TValue) == typeof(TNullable));
            var ut = typeof(Nullable<>).MakeGenericType(typeof(TUnderlying));
            var nt = typeof(TNullable);
            Debug.Assert(ut == nt);
        }

        protected readonly Func<TUnderlying, TValue> cast;
        protected readonly Func<object, IFormatProvider, TValue> convert;

        protected InternalNullableValueConverterBase(
            Func<TUnderlying, TNullable> cast,
            Func<object, IFormatProvider, TNullable> convert)
        {
            this.cast = (Func<TUnderlying, TValue>)(Delegate)cast;
            this.convert = (Func<object, IFormatProvider, TValue>)(Delegate)convert;
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override sealed TValue ConvertTo(ConversionContext context, object? value) =>
            value is null ? default! :
            value is TUnderlying v ? cast(v) :
            convert(value, context.FormatProvider);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override sealed TValue UnsafeConvertTo(ConversionContext context, object value) =>
            value is TUnderlying v ? cast(v) :
            convert(value, context.FormatProvider);
    }

    internal sealed class NullableBooleanValueConverter<T> :
        InternalNullableValueConverterBase<T, bool?, bool>
    {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static bool? ToBoolean(object? value, IFormatProvider fp) =>
            System.Convert.ToBoolean(value, fp);

        public NullableBooleanValueConverter() :
            base(value => value, ToBoolean)
        { }
    }

    internal sealed class NullableByteValueConverter<T> :
        InternalNullableValueConverterBase<T, byte?, byte>
    {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static byte? ToByte(object? value, IFormatProvider fp) =>
            System.Convert.ToByte(value, fp);

        public NullableByteValueConverter() :
            base(value => value, ToByte)
        { }
    }

    internal sealed class NullableInt16ValueConverter<T> :
        InternalNullableValueConverterBase<T, short?, short>
    {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static short? ToInt16(object? value, IFormatProvider fp) =>
            System.Convert.ToInt16(value, fp);

        public NullableInt16ValueConverter() :
            base(value => value, ToInt16)
        { }
    }

    internal sealed class NullableInt32ValueConverter<T> :
        InternalNullableValueConverterBase<T, int?, int>
    {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static int? ToInt32(object? value, IFormatProvider fp) =>
            System.Convert.ToInt32(value, fp);

        public NullableInt32ValueConverter() :
            base(value => value, ToInt32)
        { }
    }

    internal sealed class NullableInt64ValueConverter<T> :
        InternalNullableValueConverterBase<T, long?, long>
    {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static long? ToInt64(object? value, IFormatProvider fp) =>
            System.Convert.ToInt64(value, fp);

        public NullableInt64ValueConverter() :
            base(value => value, ToInt64)
        { }
    }

    internal sealed class NullableSingleValueConverter<T> :
        InternalNullableValueConverterBase<T, float?, float>
    {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static float? ToSingle(object? value, IFormatProvider fp) =>
            System.Convert.ToSingle(value, fp);

        public NullableSingleValueConverter() :
            base(value => value, ToSingle)
        { }
    }

    internal sealed class NullableDoubleValueConverter<T> :
        InternalNullableValueConverterBase<T, double?, double>
    {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static double? ToDouble(object? value, IFormatProvider fp) =>
            System.Convert.ToDouble(value, fp);

        public NullableDoubleValueConverter() :
            base(value => value, ToDouble)
        { }
    }

    internal sealed class NullableDecimalValueConverter<T> :
        InternalNullableValueConverterBase<T, decimal?, decimal>
    {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static decimal? ToDecimal(object? value, IFormatProvider fp) =>
            System.Convert.ToDecimal(value, fp);

        public NullableDecimalValueConverter() :
            base(value => value, ToDecimal)
        { }
    }

    internal sealed class NullableGuidValueConverter<T> :
        InternalNullableValueConverterBase<T, Guid?, Guid>
    {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static Guid? ToGuid(object value, IFormatProvider fp) =>
            value is byte[] blob ?
                new Guid(blob) :   // vv Couldn't convert from byte array.
                new Guid(System.Convert.ToString(value, fp)!);

        public NullableGuidValueConverter() :
            base(value => value, ToGuid)
        { }
    }

    internal sealed class NullableDateTimeValueConverter<T> :
        InternalNullableValueConverterBase<T, DateTime?, DateTime>
    {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static DateTime? ToDateTime(object value, IFormatProvider fp) =>
            System.Convert.ToDateTime(value, fp);

        public NullableDateTimeValueConverter() :
            base(value => value, ToDateTime)
        { }
    }

    internal sealed class NullableCharValueConverter<T> :
        InternalNullableValueConverterBase<T, char?, char>
    {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static char? ToChar(object value, IFormatProvider fp) =>
            System.Convert.ToChar(value, fp);

        public NullableCharValueConverter() :
            base(value => value, ToChar)
        { }
    }

    internal sealed class NullableStringValueConverter<T> :
        InternalValueConverter<T>
    {
        private static readonly Func<object, IFormatProvider, T> convert =
            (Func<object, IFormatProvider, T>)(Delegate)
            new Func<object, IFormatProvider, string>(System.Convert.ToString!);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override T ConvertTo(ConversionContext context, object? value) =>
            value is null ? default! :
            value is T v ? v :
            convert(value!, context.FormatProvider);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override T UnsafeConvertTo(ConversionContext context, object value) =>
            value is T v ? v :
            convert(value!, context.FormatProvider);
    }

    internal sealed class NullableEnumValueConverter<T> :
        InternalValueConverter<T>
    {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override T ConvertTo(ConversionContext context, object? value) =>
            value is null ? default! :
            value is T v ? v :
            EnumConverter<T>.convertTo(value!, context.FormatProvider);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override T UnsafeConvertTo(ConversionContext context, object value) =>
            value is T v ? v :
            EnumConverter<T>.convertTo(value, context.FormatProvider);
    }

    internal sealed class NullableByteArrayValueConverter<T> :
        InternalValueConverter<T>
    {
        private static readonly Func<ConversionContext, object, T> convert =
            (Func<ConversionContext, object, T>)(Delegate)
            new Func<ConversionContext, object, byte[]>(ByteArrayConverter.Convert);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override T ConvertTo(ConversionContext context, object? value) =>
            value is null ? default! :
            value is T v ? v :
            convert(context, value!);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override T UnsafeConvertTo(ConversionContext context, object value) =>
            value is T v ? v :
            convert(context, value);
    }
}
