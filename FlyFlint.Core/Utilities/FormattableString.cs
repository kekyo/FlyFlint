////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

#if !(NET46_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER)
using System;
using System.Runtime.CompilerServices;

namespace System.Runtime.CompilerServices
{
    public struct FormattableString : IFormattable
    {
        public readonly string Format;
        private readonly object[] args;

#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public FormattableString(string format, params object[] args)
        {
            this.Format = format;
            this.args = args;
        }

#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public object[] GetArguments() =>
            this.args;

#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public string ToString(string ignored, IFormatProvider formatProvider) =>
            string.Format(formatProvider, this.Format, this.args);
    }
}
#endif
