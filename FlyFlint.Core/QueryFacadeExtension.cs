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
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace FlyFlint
{
    public static class QueryFacadeExtension
    {
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext Query<TParameters>(
            this DbConnection connection,
            string sql, TParameters parameters) =>
            new QueryContext(
                connection,
                null,
                FlyFlint.Query.fp,
                FlyFlint.Query.encoding,
                sql,
                DynamicQueryExecutorFacade.GetParameters(ref parameters, FlyFlint.Query.parameterPrefix),
                FlyFlint.Query.parameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext Query<TParameters>(
            this DbConnection connection, DbTransaction transaction,
            string sql, TParameters parameters) =>
            new QueryContext(
                connection,
                transaction,
                FlyFlint.Query.fp,
                FlyFlint.Query.encoding,
                sql,
                DynamicQueryExecutorFacade.GetParameters(ref parameters, FlyFlint.Query.parameterPrefix),
                FlyFlint.Query.parameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext Query<TParameters>(
            this DbConnection connection,
            PreparedQueryContext prepared, TParameters parameters) =>
            new QueryContext(
                connection,
                null,
                prepared.fp,
                prepared.encoding,
                prepared.sql,
                DynamicQueryExecutorFacade.GetParameters(ref parameters, prepared.parameterPrefix),
                prepared.parameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext Query<TParameters>(
            this DbConnection connection, DbTransaction transaction,
            PreparedQueryContext prepared, TParameters parameters) =>
            new QueryContext(
                connection,
                transaction,
                prepared.fp,
                prepared.encoding,
                prepared.sql,
                DynamicQueryExecutorFacade.GetParameters(ref parameters, prepared.parameterPrefix),
                prepared.parameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext<T> Query<T, TParameters>(
            this DbConnection connection,
            PreparedQueryContext<T> prepared, TParameters parameters)
            where T : new() =>
            new QueryContext<T>(
                connection,
                null,
                prepared.fp,
                prepared.encoding,
                prepared.sql,
                DynamicQueryExecutorFacade.GetParameters(ref parameters, prepared.parameterPrefix),
                prepared.parameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext<T> Query<T, TParameters>(
            this DbConnection connection, DbTransaction transaction,
            PreparedQueryContext<T> prepared, TParameters parameters)
            where T : new() =>
            new QueryContext<T>(
                connection,
                transaction,
                prepared.fp,
                prepared.encoding,
                prepared.sql,
                DynamicQueryExecutorFacade.GetParameters(ref parameters, prepared.parameterPrefix),
                prepared.parameterPrefix);

        /////////////////////////////////////////////////////////////////////////////

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PreparedQueryContext Parameter<TParameter>(
            this PreparedQueryContext query, TParameter parameters) =>
            new PreparedQueryContext(
                query.fp,
                query.encoding,
                query.sql,
                DynamicQueryExecutorFacade.GetParameters(ref parameters, query.parameterPrefix),
                query.parameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext Parameter<TParameter>(
            this QueryContext query, TParameter parameters) =>
            new QueryContext(
                query.connection,
                query.transaction,
                query.fp,
                query.encoding,
                query.sql,
                DynamicQueryExecutorFacade.GetParameters(ref parameters, query.parameterPrefix),
                query.parameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PreparedQueryContext<T> Parameter<T, TParameter>(
            this PreparedQueryContext<T> query, TParameter parameters)
            where T : new() =>
            new PreparedQueryContext<T>(
                query.fp,
                query.encoding,
                query.sql,
                DynamicQueryExecutorFacade.GetParameters(ref parameters, query.parameterPrefix),
                query.parameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext<T> Parameter<T, TParameter>(
            this QueryContext<T> query, TParameter parameters)
            where T : new() =>
            new QueryContext<T>(
                query.connection,
                query.transaction,
                query.fp,
                query.encoding,
                query.sql,
                DynamicQueryExecutorFacade.GetParameters(ref parameters, query.parameterPrefix),
                query.parameterPrefix);

        /////////////////////////////////////////////////////////////////////////////

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static IEnumerable<T> Execute<T>(this QueryContext<T> query)
            where T : new() =>
            DynamicQueryExecutorFacade.Execute(query);

#if !NET40 && !NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IAsyncEnumerable<T> ExecuteAsync<T>(this QueryContext<T> query)
            where T : new() =>
            DynamicQueryExecutorFacade.ExecuteAsync(query);
#endif
    }
}
