////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Internal.Converter;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace FlyFlint.Context
{
    public class ConversionContext
    {
        public readonly IFormatProvider FormatProvider;
        public readonly Encoding Encoding;

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public ConversionContext(IFormatProvider fp, Encoding encoding)
        {
            this.FormatProvider = fp;
            this.Encoding = encoding;
        }

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public virtual T Convert<T>(object value)
        {
            Debug.Assert(value != null);
            Debug.Assert(value is not DBNull);
            return InternalValueConverter<T>.converter.UnsafeConvert(this, value!);
        }

        public static readonly ConversionContext Default =
            new ConversionContext(CultureInfo.InvariantCulture, Encoding.UTF8);
    }
}
