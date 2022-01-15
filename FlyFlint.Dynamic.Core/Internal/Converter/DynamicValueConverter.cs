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

        protected abstract object? Convert(IFormatProvider fp, Encoding encoding, object? value);
        protected abstract object? UnsafeConvert(IFormatProvider fp, Encoding encoding, object value);

        private static DynamicValueConverter GetConverter(Type targetType)
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

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object? Convert(IFormatProvider fp, Encoding encoding, object? value, Type targetType) =>
            GetConverter(targetType).Convert(fp, encoding, value);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object? UnsafeConvert(IFormatProvider fp, Encoding encoding, object value, Type targetType) =>
            GetConverter(targetType).UnsafeConvert(fp, encoding, value);
    }

    internal sealed class DynamicValueConverter<T> : DynamicValueConverter
    {
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        protected override object? Convert(IFormatProvider fp, Encoding encoding, object? value) =>
            InternalValueConverter<T>.converter.Convert(fp, encoding, value);
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        protected override object? UnsafeConvert(IFormatProvider fp, Encoding encoding, object value) =>
            InternalValueConverter<T>.converter.UnsafeConvert(fp, encoding, value);
    }
}
