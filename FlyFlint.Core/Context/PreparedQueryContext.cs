////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Internal;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FlyFlint.Context
{
    public abstract class PreparedQueryContext
    {
        internal readonly ConversionContext cc;
        internal readonly Func<QueryParameterBuilderResult> builder;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private protected PreparedQueryContext(
            ConversionContext cc,
            Func<QueryParameterBuilderResult> builder)
        {
            this.cc = cc;
            this.builder = builder;
        }
    }

    public abstract class PreparedQueryContext<TElement>
        where TElement : new()
    {
        internal readonly ConversionContext cc;
        internal readonly IComparer<string> fieldComparer;
        internal readonly Func<QueryParameterBuilderResult> builder;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private protected PreparedQueryContext(
            ConversionContext cc,
            IComparer<string> fieldComparer,
            Func<QueryParameterBuilderResult> builder)
        {
            this.cc = cc;
            this.fieldComparer = fieldComparer;
            this.builder = builder;
        }
    }
}
