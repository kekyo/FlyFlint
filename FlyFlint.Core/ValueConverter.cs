////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Context;
using FlyFlint.Internal;
using FlyFlint.Internal.Converter;
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace FlyFlint
{
    public static class ValueConverter
    {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T Convert<T>(IFormatProvider fp, Encoding encoding, object? value) =>
            InternalValueConverter<T>.converter.Convert(new ConversionContext(fp, encoding), value);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object? Convert(IFormatProvider fp, Encoding encoding, object? value, Type targetType) =>
            QueryExecutor.Instance.Convert(new ConversionContext(fp, encoding), value, targetType);
    }
}
