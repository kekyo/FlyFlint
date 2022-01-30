////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Context;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FlyFlint.Internal.Converter
{
    internal abstract class DynamicValueConverter
    {
        private static readonly Dictionary<Type, DynamicValueConverter> converters = new();

        public abstract object? Convert(ConversionContext context, object? value);
        public abstract object? UnsafeConvert(ConversionContext context, object value);

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
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override object? Convert(ConversionContext context, object? value) =>
            InternalValueConverter<T>.converter.ConvertTo(context, value);
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override object? UnsafeConvert(ConversionContext context, object value) =>
            InternalValueConverter<T>.converter.UnsafeConvertTo(context, value);
    }
}
