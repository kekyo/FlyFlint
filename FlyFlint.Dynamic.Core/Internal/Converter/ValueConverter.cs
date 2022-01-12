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
using System.Runtime.CompilerServices;
using System.Text;

namespace FlyFlint.Internal.Converter
{
    internal abstract class DynamicValueConverter
    {
        private static readonly Dictionary<Type, DynamicValueConverter> converters = new();

        public abstract object? Convert(IFormatProvider fp, Encoding encoding, object? value);
        public abstract object? UnsafeConvert(IFormatProvider fp, Encoding encoding, object value);

        public static DynamicValueConverter GetConverter(Type targetType)
        {
            lock (converters)
            {
                if (!converters.TryGetValue(targetType, out var converter))
                {
                    var ct = typeof(DynamicValueConverter<>).MakeGenericType(targetType);
                    converter = (DynamicValueConverter)Activator.CreateInstance(ct)!;
                    converters.Add(targetType, converter);
                }
                return converter;
            }
        }
    }

    internal sealed class DynamicValueConverter<T> : DynamicValueConverter
    {
        private static readonly InternalValueConverter<T> converter =
            InternalValueConverter.Create<T>();

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override object? Convert(IFormatProvider fp, Encoding encoding, object? value) =>
            converter.Convert(fp, encoding, value);
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override object? UnsafeConvert(IFormatProvider fp, Encoding encoding, object value) =>
            converter.UnsafeConvert(fp, encoding, value);
    }

    public static class ValueConverter
    {
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object? Convert(IFormatProvider fp, Encoding encoding, object? value, Type targetType) =>
            DynamicValueConverter.GetConverter(targetType).Convert(fp, encoding, value);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object? UnsafeConvert(IFormatProvider fp, Encoding encoding, object value, Type targetType) =>
            DynamicValueConverter.GetConverter(targetType).UnsafeConvert(fp, encoding, value);
    }
}
