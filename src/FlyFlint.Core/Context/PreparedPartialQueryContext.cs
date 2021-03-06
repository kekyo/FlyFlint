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
using System.Runtime.CompilerServices;

namespace FlyFlint.Context
{
    public sealed class PreparedPartialQueryContext : PreparedQueryContext
    {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal PreparedPartialQueryContext(
            Trait trait,
            Func<QueryParameterBuilderResult> builder) :
            base(trait, builder)
        {
        }
    }

    public sealed class PreparedPartialQueryContext<TRecord> : PreparedQueryContext<TRecord>
        where TRecord : new()
    {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal PreparedPartialQueryContext(
            Trait trait,
            Func<QueryParameterBuilderResult> builder) :
            base(trait, builder)
        {
        }
    }
}
