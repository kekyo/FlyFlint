﻿////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Internal.Converter;
using FlyFlint.Internal.Dynamic;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace FlyFlint
{
    public static class ValueConverter
    {
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T Convert<T>(IFormatProvider fp, Encoding encoding, object? value) =>
            InternalValueConverter<T>.converter.Convert(fp, encoding, value);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal static T UnsafeConvert<T>(IFormatProvider fp, Encoding encoding, object value)
        {
            Debug.Assert(value != null);
            Debug.Assert(value is not DBNull);
            return InternalValueConverter<T>.converter.UnsafeConvert(fp, encoding, value!);
        }

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object? Convert(IFormatProvider fp, Encoding encoding, object? value, Type targetType) =>
            DynamicQueryExecutorFacade.Convert(fp, encoding, value, targetType);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal static object? UnsafeConvert(IFormatProvider fp, Encoding encoding, object value, Type targetType) =>
            DynamicQueryExecutorFacade.UnsafeConvert(fp, encoding, value, targetType);
    }
}