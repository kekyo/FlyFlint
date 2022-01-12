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
using System.Runtime.CompilerServices;
using System.Text;

namespace FlyFlint.Internal.Converter
{
    internal static class InternalValueConverter
    {
        public static InternalValueConverter<T> Create<T>()
        {
            if (typeof(T) == typeof(bool))
            {
                return new BooleanValueConverter<T>();
            }
            else if (typeof(T) == typeof(byte))
            {
                return new ByteValueConverter<T>();
            }
            else if (typeof(T) == typeof(short))
            {
                return new Int16ValueConverter<T>();
            }
            else if (typeof(T) == typeof(int))
            {
                return new Int32ValueConverter<T>();
            }
            else if (typeof(T) == typeof(long))
            {
                return new Int64ValueConverter<T>();
            }
            else if (typeof(T) == typeof(float))
            {
                return new SingleValueConverter<T>();
            }
            else if (typeof(T) == typeof(double))
            {
                return new DoubleValueConverter<T>();
            }
            else if (typeof(T) == typeof(decimal))
            {
                return new DecimalValueConverter<T>();
            }
            else if (typeof(T) == typeof(Guid))
            {
                return new GuidValueConverter<T>();
            }
            else if (typeof(T) == typeof(DateTime))
            {
                return new DateTimeValueConverter<T>();
            }
            else if (typeof(T) == typeof(char))
            {
                return new CharValueConverter<T>();
            }
            else if (typeof(T) == typeof(string))
            {
                return new NullableStringValueConverter<T>();
            }
            else if (typeof(T).IsEnum)
            {
                return new EnumValueConverter<T>();
            }
            else if (typeof(T) == typeof(byte[]))
            {
                return new NullableByteArrayValueConverter<T>();
            }
            else if (typeof(T) == typeof(bool?))
            {
                return new NullableBooleanValueConverter<T>();
            }
            else if (typeof(T) == typeof(byte?))
            {
                return new NullableByteValueConverter<T>();
            }
            else if (typeof(T) == typeof(short?))
            {
                return new NullableInt16ValueConverter<T>();
            }
            else if (typeof(T) == typeof(int?))
            {
                return new NullableInt32ValueConverter<T>();
            }
            else if (typeof(T) == typeof(long?))
            {
                return new NullableInt64ValueConverter<T>();
            }
            else if (typeof(T) == typeof(float?))
            {
                return new NullableSingleValueConverter<T>();
            }
            else if (typeof(T) == typeof(double?))
            {
                return new NullableDoubleValueConverter<T>();
            }
            else if (typeof(T) == typeof(decimal?))
            {
                return new NullableDecimalValueConverter<T>();
            }
            else if (typeof(T) == typeof(Guid?))
            {
                return new NullableGuidValueConverter<T>();
            }
            else if (typeof(T) == typeof(DateTime?))
            {
                return new NullableDateTimeValueConverter<T>();
            }
            else if (typeof(T) == typeof(char?))
            {
                return new NullableCharValueConverter<T>();
            }
            else if (Nullable.GetUnderlyingType(typeof(T))?.IsEnum ?? false)
            {
                return new NullableEnumValueConverter<T>();
            }
            else
            {
                return new InvalidOperationExceptionConverter<T>();
            }
        }
    }

    internal abstract class InternalValueConverter<T>
    {
        public abstract T Convert(IFormatProvider fp, Encoding encoding, object? value);
        public abstract T UnsafeConvert(IFormatProvider fp, Encoding encoding, object value);
    }

    public static class ValueConverter<T>
    {
        private static readonly InternalValueConverter<T> converter =
            InternalValueConverter.Create<T>();

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T Convert(IFormatProvider fp, Encoding encoding, object? value) =>
            converter.Convert(fp, encoding, value);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T UnsafeConvert(IFormatProvider fp, Encoding encoding, object value)
        {
            Debug.Assert(value != null);
            Debug.Assert(value is not DBNull);
            return converter.UnsafeConvert(fp, encoding, value!);
        }
    }

    /////////////////////////////////////////////////////////////////////////////

    internal static class ByteArrayValueConverter
    {
        public static byte[] ToByteArray(object value, IFormatProvider fp, Encoding encoding)
        {
            if (value is string str)
            {
                return encoding.GetBytes(str);
            }
            else if (value is char c)
            {
                return BitConverter.GetBytes(c);
            }
            else if (value is bool b)
            {
                return BitConverter.GetBytes(b);
            }
            else if (value is byte b8)
            {
                return BitConverter.GetBytes(b8);
            }
            else if (value is short i16)
            {
                return BitConverter.GetBytes(i16);
            }
            else if (value is int i32)
            {
                return BitConverter.GetBytes(i32);
            }
            else if (value is long i64)
            {
                return BitConverter.GetBytes(i64);
            }
            else if (value is float f)
            {
                return BitConverter.GetBytes(f);
            }
            else if (value is double d)
            {
                return BitConverter.GetBytes(d);
            }
            else if (value is Guid g)
            {
                return g.ToByteArray();
            }
            else if (value is DateTime dt)
            {
                return BitConverter.GetBytes(dt.ToBinary());
            }
            else
            {
                return encoding.GetBytes(Convert.ToString(value, fp)!);
            }
        }
    }

    /////////////////////////////////////////////////////////////////////////////

    internal abstract class InternalValueConverterBase<T, TR> : InternalValueConverter<T>
    {
        static InternalValueConverterBase() =>
            Debug.Assert(typeof(T) == typeof(TR));

        protected readonly Func<object, IFormatProvider, T> converter;

        protected InternalValueConverterBase(Func<object, IFormatProvider, TR> converter) =>
            this.converter = (Func<object, IFormatProvider, T>)(Delegate)converter;

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override sealed T Convert(IFormatProvider fp, Encoding encoding, object? value) =>
            value is null ? throw new NullReferenceException($"Could not convert from null to {typeof(T).FullName}.") :
            value is DBNull ? throw new NullReferenceException($"Could not convert from DBNull to {typeof(T).FullName}.") :
            value is T v ? v :
            converter(value!, fp);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override sealed T UnsafeConvert(IFormatProvider fp, Encoding encoding, object value) =>
            value is T v ? v :
            converter(value!, fp);
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
            base(EnumConverter<T>.Convert)
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

        protected readonly Func<TUnderlying, T> caster;
        protected readonly Func<object, IFormatProvider, T> converter;

        protected InternalNullableValueConverterBase(
            Func<TUnderlying, TNullable> caster,
            Func<object, IFormatProvider, TNullable> converter)
        {
            this.caster = (Func<TUnderlying, T>)(Delegate)caster;
            this.converter = (Func<object, IFormatProvider, T>)(Delegate)converter;
        }

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override sealed T Convert(IFormatProvider fp, Encoding encoding, object? value) =>
            value is null ? default! :
            value is DBNull ? default! :
            value is TUnderlying v ? caster(v) :
            converter(value!, fp);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override sealed T UnsafeConvert(IFormatProvider fp, Encoding encoding, object value) =>
            value is TUnderlying v ? caster(v) :
            converter(value!, fp);
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
        private static readonly Func<object, IFormatProvider, T> converter =
            (Func<object, IFormatProvider, T>)(Delegate)
            new Func<object, IFormatProvider, string>(System.Convert.ToString!);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override sealed T Convert(IFormatProvider fp, Encoding encoding, object? value) =>
            value is null ? default! :
            value is DBNull ? default! :
            value is T v ? v :
            converter(value!, fp);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override sealed T UnsafeConvert(IFormatProvider fp, Encoding encoding, object value) =>
            value is T v ? v :
            converter(value!, fp);
    }

    internal sealed class NullableEnumValueConverter<T> :
        InternalNullableValueConverterBase<T, T?, T>
    {
        private static T? ToEnum(object value, IFormatProvider fp) =>
            EnumConverter<T>.Convert(value, fp);

        public NullableEnumValueConverter() :
            base(value => value, ToEnum)
        { }
    }

    internal sealed class NullableByteArrayValueConverter<T> :
        InternalValueConverter<T>
    {
        private static readonly Func<object, IFormatProvider, Encoding, T> converter =
            (Func<object, IFormatProvider, Encoding, T>)(Delegate)
            new Func<object, IFormatProvider, Encoding, byte[]>(ByteArrayValueConverter.ToByteArray);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override sealed T Convert(IFormatProvider fp, Encoding encoding, object? value) =>
            value is null ? default! :
            value is DBNull ? default! :
            value is T v ? v :
            converter(value!, fp, encoding);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override sealed T UnsafeConvert(IFormatProvider fp, Encoding encoding, object value) =>
            value is T v ? v :
            converter(value!, fp, encoding);
    }

    /////////////////////////////////////////////////////////////////////////////

    internal sealed class InvalidOperationExceptionConverter<T> : InternalValueConverter<T>
    {
        public override T Convert(IFormatProvider fp, Encoding encoding, object? value) =>
            throw new InvalidOperationException($"Could not convert to {typeof(T).FullName}");
        public override T UnsafeConvert(IFormatProvider fp, Encoding encoding, object value) =>
            throw new InvalidOperationException($"Could not convert to {typeof(T).FullName}");
    }
}
