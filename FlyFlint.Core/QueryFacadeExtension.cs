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
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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
                ConversionContext.Default,
                sql,
                DynamicQueryExecutorFacade.GetParameters(ref parameters, FlyFlint.Query.defaultParameterPrefix),
                FlyFlint.Query.defaultParameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext Query<TParameters>(
            this DbConnection connection, DbTransaction transaction,
            string sql, TParameters parameters) =>
            new QueryContext(
                connection,
                transaction,
                ConversionContext.Default,
                sql,
                DynamicQueryExecutorFacade.GetParameters(ref parameters, FlyFlint.Query.defaultParameterPrefix),
                FlyFlint.Query.defaultParameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext Query<TParameters>(
            this DbConnection connection,
            PreparedQueryContext prepared, TParameters parameters) =>
            new QueryContext(
                connection,
                null,
                prepared.cc,
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
                prepared.cc,
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
                prepared.cc,
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
                prepared.cc,
                prepared.sql,
                DynamicQueryExecutorFacade.GetParameters(ref parameters, prepared.parameterPrefix),
                prepared.parameterPrefix);

        /////////////////////////////////////////////////////////////////////////////

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PreparedQueryContext Parameter<TParameter>(
            this PreparedQueryContext prepared, TParameter parameters) =>
            new PreparedQueryContext(
                prepared.cc,
                prepared.sql,
                DynamicQueryExecutorFacade.GetParameters(ref parameters, prepared.parameterPrefix),
                prepared.parameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext Parameter<TParameter>(
            this QueryContext query, TParameter parameters) =>
            new QueryContext(
                query.connection,
                query.transaction,
                query.cc,
                query.sql,
                DynamicQueryExecutorFacade.GetParameters(ref parameters, query.parameterPrefix),
                query.parameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PreparedQueryContext<T> Parameter<T, TParameter>(
            this PreparedQueryContext<T> prepared, TParameter parameters)
            where T : new() =>
            new PreparedQueryContext<T>(
                prepared.cc,
                prepared.sql,
                DynamicQueryExecutorFacade.GetParameters(ref parameters, prepared.parameterPrefix),
                prepared.parameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext<T> Parameter<T, TParameter>(
            this QueryContext<T> query, TParameter parameters)
            where T : new() =>
            new QueryContext<T>(
                query.connection,
                query.transaction,
                query.cc,
                query.sql,
                DynamicQueryExecutorFacade.GetParameters(ref parameters, query.parameterPrefix),
                query.parameterPrefix);

        /////////////////////////////////////////////////////////////////////////////

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<int> ExecuteNonQueryAsync(this QueryContext query) =>
            DynamicQueryExecutorFacade.ExecuteNonQueryAsync(query);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<T> ExecuteScalarAsync<T>(this QueryContext query) =>
            DynamicQueryExecutorFacade.ExecuteScalarAsync<T>(query);

#if !NET40 && !NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IAsyncEnumerable<T> ExecuteAsync<T>(this QueryContext<T> query)
            where T : new() =>
            DynamicQueryExecutorFacade.ExecuteAsync(query);
#else
        [Obsolete("Before net461 platform, it is not supported async enumeration. Consider upgrades to net461 or upper, or `Execute()` method with `FlyFlint.Synchronized` namespace instead.", true)]
        public static void ExecuteAsync<T>(this QueryContext<T> query)
            where T : new() =>
            throw new InvalidOperationException("Before net461 platform, it is not supported async enumeration. Consider upgrades to net461 or upper, or `Execute()` method with `FlyFlint.Synchronized` namespace instead.");
#endif
    }
}
