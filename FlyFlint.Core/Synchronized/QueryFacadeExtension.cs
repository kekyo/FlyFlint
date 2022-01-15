////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Internal.Dynamic;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FlyFlint.Synchronized
{
    public static class QueryFacadeExtension
    {
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int ExecuteNonQuery(this QueryContext query) =>
            DynamicQueryExecutorFacade.ExecuteNonQuery(query);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T ExecuteScalar<T>(this QueryContext query) =>
            DynamicQueryExecutorFacade.ExecuteScalar<T>(query);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static IEnumerable<T> Execute<T>(this QueryContext<T> query)
            where T : new() =>
            DynamicQueryExecutorFacade.Execute(query);
    }
}
