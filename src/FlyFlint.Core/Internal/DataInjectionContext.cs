////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Context;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace FlyFlint.Internal
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public delegate void DataInjectorDelegate<TRecord>(ref TRecord record)
        where TRecord : notnull;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class DataInjectionContext
    {
        internal readonly ConversionContext cc;
        internal readonly IComparer<string> fieldComparer;
        internal readonly DbDataReader reader;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private protected DataInjectionContext(
            ConversionContext cc,
            IComparer<string> fieldComparer,
            DbDataReader reader)
        {
            this.cc = cc;
            this.fieldComparer = fieldComparer;
            this.reader = reader;
        }
    }
}
