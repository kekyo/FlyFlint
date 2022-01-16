////////////////////////////////////////////////////////////////////////////
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
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace FlyFlint.Context
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class ConversionContext
    {
        public readonly IFormatProvider FormatProvider;
        public readonly Encoding Encoding;

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        protected internal ConversionContext(IFormatProvider fp, Encoding encoding)
        {
            this.FormatProvider = fp;
            this.Encoding = encoding;
        }

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        protected virtual T Convert<T>(object value)
        {
            Debug.Assert(value != null);
            Debug.Assert(value is not DBNull);
            return InternalValueConverter<T>.converter.UnsafeConvert(this, value!);
        }

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        protected virtual object? Convert(object value, Type targetType)
        {
            Debug.Assert(value != null);
            Debug.Assert(value is not DBNull);
            return DynamicQueryExecutorFacade.UnsafeConvert(this, value!, targetType);
        }
    }
}
