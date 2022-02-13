////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Internal;
using FlyFlint.Internal.Converter;
using FlyFlint.Internal.Converter.Specialized;
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

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public ConversionContext(IFormatProvider fp, Encoding encoding)
        {
            this.FormatProvider = fp;
            this.Encoding = encoding;
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public virtual T ConvertTo<T>(object? value)
        {
            Debug.Assert(value is not DBNull);
            return InternalValueConverter<T>.converter.UnsafeConvertTo(this, value!);
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public virtual object? ConvertFrom<T>(in T value, string? format) =>
            QueryHelper.IsNullOrWhiteSpace(format) ?
                value :
                value is Enum ev ?
                    EnumConverter.ConvertFrom(ev, format) :
                    value is IFormattable f ?
                        f.ToString(format, this.FormatProvider) :
                        value;

        public static readonly ConversionContext Default =
            new ConversionContext(CultureInfo.InvariantCulture, Encoding.UTF8);
    }
}
