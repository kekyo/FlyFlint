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
    internal abstract class InternalValueConverter<T>
    {
        public static readonly InternalValueConverter<T> converter;

        static InternalValueConverter()
        {
            if (typeof(T) == typeof(bool))
            {
                converter = new BooleanValueConverter<T>();
            }
            else if (typeof(T) == typeof(byte))
            {
                converter = new ByteValueConverter<T>();
            }
            else if (typeof(T) == typeof(short))
            {
                converter = new Int16ValueConverter<T>();
            }
            else if (typeof(T) == typeof(int))
            {
                converter = new Int32ValueConverter<T>();
            }
            else if (typeof(T) == typeof(long))
            {
                converter = new Int64ValueConverter<T>();
            }
            else if (typeof(T) == typeof(float))
            {
                converter = new SingleValueConverter<T>();
            }
            else if (typeof(T) == typeof(double))
            {
                converter = new DoubleValueConverter<T>();
            }
            else if (typeof(T) == typeof(decimal))
            {
                converter = new DecimalValueConverter<T>();
            }
            else if (typeof(T) == typeof(Guid))
            {
                converter = new GuidValueConverter<T>();
            }
            else if (typeof(T) == typeof(DateTime))
            {
                converter = new DateTimeValueConverter<T>();
            }
            else if (typeof(T) == typeof(char))
            {
                converter = new CharValueConverter<T>();
            }
            else if (typeof(T) == typeof(string))
            {
                converter = new NullableStringValueConverter<T>();
            }
            else if (typeof(T).IsEnum)
            {
                converter = new EnumValueConverter<T>();
            }
            else if (typeof(T) == typeof(byte[]))
            {
                converter = new NullableByteArrayValueConverter<T>();
            }
            else if (typeof(T) == typeof(bool?))
            {
                converter = new NullableBooleanValueConverter<T>();
            }
            else if (typeof(T) == typeof(byte?))
            {
                converter = new NullableByteValueConverter<T>();
            }
            else if (typeof(T) == typeof(short?))
            {
                converter = new NullableInt16ValueConverter<T>();
            }
            else if (typeof(T) == typeof(int?))
            {
                converter = new NullableInt32ValueConverter<T>();
            }
            else if (typeof(T) == typeof(long?))
            {
                converter = new NullableInt64ValueConverter<T>();
            }
            else if (typeof(T) == typeof(float?))
            {
                converter = new NullableSingleValueConverter<T>();
            }
            else if (typeof(T) == typeof(double?))
            {
                converter = new NullableDoubleValueConverter<T>();
            }
            else if (typeof(T) == typeof(decimal?))
            {
                converter = new NullableDecimalValueConverter<T>();
            }
            else if (typeof(T) == typeof(Guid?))
            {
                converter = new NullableGuidValueConverter<T>();
            }
            else if (typeof(T) == typeof(DateTime?))
            {
                converter = new NullableDateTimeValueConverter<T>();
            }
            else if (typeof(T) == typeof(char?))
            {
                converter = new NullableCharValueConverter<T>();
            }
            else if (Nullable.GetUnderlyingType(typeof(T))?.IsEnum ?? false)
            {
                converter = new NullableEnumValueConverter<T>();
            }
            else
            {
                converter = new InvalidOperationExceptionConverter<T>();
            }
        }

        public abstract T Convert(ConversionContext context, object? value);
        public abstract T UnsafeConvert(ConversionContext context, object value);
    }

    internal sealed class InvalidOperationExceptionConverter<T> : InternalValueConverter<T>
    {
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override T Convert(ConversionContext context, object? value) =>
            throw new InvalidOperationException($"Could not convert to {typeof(T).FullName}");
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override T UnsafeConvert(ConversionContext context, object value) =>
            throw new InvalidOperationException($"Could not convert to {typeof(T).FullName}");
    }

    /////////////////////////////////////////////////////////////////////////////

    internal abstract class InternalValueConverterBase<T, TR> : InternalValueConverter<T>
    {
        static InternalValueConverterBase() =>
            Debug.Assert(typeof(T) == typeof(TR));

        protected readonly Func<object, IFormatProvider, T> convert;

        protected InternalValueConverterBase(Func<object, IFormatProvider, TR> convert) =>
            this.convert = (Func<object, IFormatProvider, T>)(Delegate)convert;

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override sealed T Convert(ConversionContext context, object? value) =>
            value is null ? throw new NullReferenceException($"Could not convert from null to {typeof(T).FullName}.") :
            value is DBNull ? throw new NullReferenceException($"Could not convert from DBNull to {typeof(T).FullName}.") :
            value is T v ? v :
            convert(value, context.FormatProvider);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override sealed T UnsafeConvert(ConversionContext context, object value) =>
            value is T v ? v :
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
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static Guid ToGuid(object value, IFormatProvider fp) =>
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
            base(EnumConverter<T>.convert)
        { }
    }

    /////////////////////////////////////////////////////////////////////////////

    internal abstract class InternalNullableValueConverterBase<T, TNullable, TUnderlying> : InternalValueConverter<T>
    {
        static InternalNullableValueConverterBase()
        {
            Debug.Assert(typeof(T) == typeof(TNullable));
            var ut = typeof(Nullable<>).MakeGenericType(typeof(TUnderlying));
            var nt = typeof(TNullable);
            Debug.Assert(ut == nt);
        }

        protected readonly Func<TUnderlying, T> cast;
        protected readonly Func<object, IFormatProvider, T> convert;

        protected InternalNullableValueConverterBase(
            Func<TUnderlying, TNullable> cast,
            Func<object, IFormatProvider, TNullable> convert)
        {
            this.cast = (Func<TUnderlying, T>)(Delegate)cast;
            this.convert = (Func<object, IFormatProvider, T>)(Delegate)convert;
        }

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override sealed T Convert(ConversionContext context, object? value) =>
            value is null ? default! :
            value is DBNull ? default! :
            value is TUnderlying v ? cast(v) :
            convert(value, context.FormatProvider);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override sealed T UnsafeConvert(ConversionContext context, object value) =>
            value is TUnderlying v ? cast(v) :
            convert(value, context.FormatProvider);
    }

    internal sealed class NullableBooleanValueConverter<T> :
        InternalNullableValueConverterBase<T, bool?, bool>
    {
#if !NET40
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
#if !NET40
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
#if !NET40
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
#if !NET40
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
#if !NET40
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
#if !NET40
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
#if !NET40
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
#if !NET40
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
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static Guid? ToGuid(object value, IFormatProvider fp) =>
            new Guid(System.Convert.ToString(value, fp)!);

        public NullableGuidValueConverter() :
            base(value => value, ToGuid)
        { }
    }

    internal sealed class NullableDateTimeValueConverter<T> :
        InternalNullableValueConverterBase<T, DateTime?, DateTime>
    {
#if !NET40
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
#if !NET40
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

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override T Convert(ConversionContext context, object? value) =>
            value is null ? default! :
            value is DBNull ? default! :
            value is T v ? v :
            convert(value!, context.FormatProvider);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override T UnsafeConvert(ConversionContext context, object value) =>
            value is T v ? v :
            convert(value!, context.FormatProvider);
    }

    internal sealed class NullableEnumValueConverter<T> :
        InternalValueConverter<T>
    {
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override T Convert(ConversionContext context, object? value) =>
            value is null ? default! :
            value is DBNull ? default! :
            value is T v ? v :
            EnumConverter<T>.convert(value!, context.FormatProvider);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override T UnsafeConvert(ConversionContext context, object value) =>
            value is T v ? v :
            EnumConverter<T>.convert(value, context.FormatProvider);
    }

    internal sealed class NullableByteArrayValueConverter<T> :
        InternalValueConverter<T>
    {
        private static readonly Func<ConversionContext, object, T> convert =
            (Func<ConversionContext, object, T>)(Delegate)
            new Func<ConversionContext, object, byte[]>(ByteArrayConverter.Convert);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override T Convert(ConversionContext context, object? value) =>
            value is null ? default! :
            value is DBNull ? default! :
            value is T v ? v :
            convert(context, value!);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override T UnsafeConvert(ConversionContext context, object value) =>
            value is T v ? v :
            convert(context, value);
    }
}
