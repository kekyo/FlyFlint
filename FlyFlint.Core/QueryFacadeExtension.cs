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
#if NET40_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
using System.Threading.Tasks;
#endif

namespace FlyFlint
{
    public static class QueryFacadeExtension
    {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext Query<TParameters>(
            this DbConnection connection,
            string sql, TParameters parameters)
            where TParameters : notnull =>
            new QueryContext(
                connection,
                null,
                ConversionContext.Default,
                sql,
                DynamicQueryExecutorFacade.GetParameters(
                    ref parameters, FlyFlint.Query.defaultParameterPrefix),
                FlyFlint.Query.defaultParameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext Query<TParameters>(
            this DbConnection connection, DbTransaction transaction,
            string sql, TParameters parameters)
            where TParameters : notnull =>
            new QueryContext(
                connection,
                transaction,
                ConversionContext.Default,
                sql,
                DynamicQueryExecutorFacade.GetParameters(
                    ref parameters, FlyFlint.Query.defaultParameterPrefix),
                FlyFlint.Query.defaultParameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext Query<TParameters>(
            this DbConnection connection,
            PreparedQueryContext prepared, TParameters parameters)
            where TParameters : notnull =>
            new QueryContext(
                connection,
                null,
                prepared.cc,
                prepared.sql,
                DynamicQueryExecutorFacade.GetParameters(
                    ref parameters, prepared.parameterPrefix),
                prepared.parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext Query<TParameters>(
            this DbConnection connection, DbTransaction transaction,
            PreparedQueryContext prepared, TParameters parameters)
            where TParameters : notnull =>
            new QueryContext(
                connection,
                transaction,
                prepared.cc,
                prepared.sql,
                DynamicQueryExecutorFacade.GetParameters(
                    ref parameters, prepared.parameterPrefix),
                prepared.parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext<T> Query<T, TParameters>(
            this DbConnection connection,
            PreparedQueryContext<T> prepared, TParameters parameters)
            where T : new()
            where TParameters : notnull =>
            new QueryContext<T>(
                connection,
                null,
                prepared.cc,
                prepared.fieldComparer,
                prepared.sql,
                DynamicQueryExecutorFacade.GetParameters(
                    ref parameters, prepared.parameterPrefix),
                prepared.parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext<T> Query<T, TParameters>(
            this DbConnection connection, DbTransaction transaction,
            PreparedQueryContext<T> prepared, TParameters parameters)
            where T : new()
            where TParameters : notnull =>
            new QueryContext<T>(
                connection,
                transaction,
                prepared.cc,
                prepared.fieldComparer,
                prepared.sql,
                DynamicQueryExecutorFacade.GetParameters(
                    ref parameters, prepared.parameterPrefix),
                prepared.parameterPrefix);

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PreparedQueryContext Parameter<TParameters>(
            this PreparedQueryContext prepared, Func<TParameters> getter)
            where TParameters : notnull =>
            new PreparedQueryContext(
                prepared.cc,
                prepared.sql,
                DynamicQueryExecutorFacade.GetConstructParameters(
                    getter, prepared.parameterPrefix),
                prepared.parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PreparedQueryContext<T> Parameter<T, TParameters>(
            this PreparedQueryContext<T> prepared, Func<TParameters> getter)
            where T : new()
            where TParameters : notnull =>
            new PreparedQueryContext<T>(
                prepared.cc,
                prepared.fieldComparer,
                prepared.sql,
                DynamicQueryExecutorFacade.GetConstructParameters(
                    getter, prepared.parameterPrefix),
                prepared.parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext Parameter<TParameters>(
            this QueryContext query, TParameters parameters)
            where TParameters : notnull =>
            new QueryContext(
                query.connection,
                query.transaction,
                query.cc,
                query.sql,
                DynamicQueryExecutorFacade.GetParameters(
                    ref parameters, query.parameterPrefix),
                query.parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext<T> Parameter<T, TParameters>(
            this QueryContext<T> query, TParameters parameters)
            where T : new()
            where TParameters : notnull =>
            new QueryContext<T>(
                query.connection,
                query.transaction,
                query.cc,
                query.fieldComparer,
                query.sql,
                DynamicQueryExecutorFacade.GetParameters(
                    ref parameters, query.parameterPrefix),
                query.parameterPrefix);

        /////////////////////////////////////////////////////////////////////////////

#if NET40_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<int> ExecuteNonQueryAsync(this QueryContext query) =>
            DynamicQueryExecutorFacade.ExecuteNonQueryAsync(query);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<T> ExecuteScalarAsync<T>(this QueryContext query) =>
            DynamicQueryExecutorFacade.ExecuteScalarAsync<T>(query);

#if NET461_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IAsyncEnumerable<T> ExecuteAsync<T>(this QueryContext<T> query)
            where T : new() =>
            DynamicQueryExecutorFacade.ExecuteAsync(query);
#else
        [Obsolete("Before net461 platform, it is not supported async enumeration. Consider upgrades to net461 or upper, or `Execute()` method with `FlyFlint.Synchronized` namespace instead.", true)]
        public static void ExecuteAsync<T>(this QueryContext<T> query)
            where T : new() =>
            throw new InvalidOperationException();
#endif
#else
        [Obsolete("Before net40 platform, it is not supported async operation. Consider upgrades to net40 or upper, or `ExecuteNonQuery()` method with `FlyFlint.Synchronized` namespace instead.", true)]
        public static void ExecuteNonQueryAsync(this QueryContext query) =>
            throw new InvalidOperationException();

        [Obsolete("Before net461 platform, it is not supported async operation. Consider upgrades to net40 or upper, or `ExecuteScalar()` method with `FlyFlint.Synchronized` namespace instead.", true)]
        public static void ExecuteScalarAsync<T>(this QueryContext query) =>
            throw new InvalidOperationException();

        [Obsolete("Before net461 platform, it is not supported async operation. Consider upgrades to net461 or upper, or `Execute()` method with `FlyFlint.Synchronized` namespace instead.", true)]
        public static void ExecuteAsync<T>(this QueryContext<T> query)
            where T : new() =>
            throw new InvalidOperationException();
#endif
    }
}
