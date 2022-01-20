////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Context;
using FlyFlint.Internal.Dynamic;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FlyFlint.Synchronized
{
    public static class QueryFacadeExtension
    {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int ExecuteNonQuery(this QueryContext query) =>
            DynamicQueryExecutorFacade.ExecuteNonQuery(query);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static TElement ExecuteScalar<TElement>(this QueryContext<TElement> query) =>
            DynamicQueryExecutorFacade.ExecuteScalar(query);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static IEnumerable<TElement> Execute<TElement>(this QueryContext<TElement> query)
            where TElement : new() =>
            DynamicQueryExecutorFacade.Execute(query);
    }
}
