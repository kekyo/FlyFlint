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
using System.ComponentModel;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Text;

namespace FlyFlint.Internal
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class DataInjectionContext : ConversionContext
    {
        internal readonly DbDataReader reader;

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public DataInjectionContext(
            DbDataReader reader, IFormatProvider fp, Encoding encoding) :
            base(fp, encoding) =>
            this.reader = reader;
    }
}
