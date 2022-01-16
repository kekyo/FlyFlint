////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace FlyFlint.Internal.Converter
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class ConversionContext
    {
        internal readonly IFormatProvider fp;
        internal readonly Encoding encoding;

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal ConversionContext(IFormatProvider fp, Encoding encoding)
        {
            this.fp = fp;
            this.encoding = encoding;
        }
    }
}
