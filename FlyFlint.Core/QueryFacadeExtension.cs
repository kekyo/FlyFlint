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
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext Query<TParameters>(
            this DbConnection connection,
            ParameterizableQueryString sql,
            TParameters parameters)
            where TParameters : notnull =>
            new ParameterizedQueryContext(
                connection,
                null,
                ConversionContext.Default,
                sql.Sql,
                DynamicQueryExecutorFacade.GetParameters(
                    ref parameters, FlyFlint.Query.defaultParameterPrefix));

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext Query<TParameters>(
            this DbConnection connection,
            DbTransaction transaction,
            ParameterizableQueryString sql,
            TParameters parameters)
            where TParameters : notnull =>
            new ParameterizedQueryContext(
                connection,
                transaction,
                ConversionContext.Default,
                sql.Sql,
                DynamicQueryExecutorFacade.GetParameters(
                    ref parameters, FlyFlint.Query.defaultParameterPrefix));

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext Query<TParameters>(
            this DbConnection connection,
            PreparedParameterizableQueryContext prepared,
            TParameters parameters)
            where TParameters : notnull =>
            new ParameterizedQueryContext(
                connection,
                null,
                prepared.cc,
                prepared.sql,
                DynamicQueryExecutorFacade.GetParameters(
                    ref parameters, prepared.parameterPrefix));

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext Query<TParameters>(
            this DbConnection connection,
            DbTransaction transaction,
            PreparedParameterizableQueryContext prepared,
            TParameters parameters)
            where TParameters : notnull =>
            new ParameterizedQueryContext(
                connection,
                transaction,
                prepared.cc,
                prepared.sql,
                DynamicQueryExecutorFacade.GetParameters(
                    ref parameters, prepared.parameterPrefix));

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext<T> Query<T, TParameters>(
            this DbConnection connection,
            PreparedParameterizableQueryContext<T> prepared,
            TParameters parameters)
            where T : new()
            where TParameters : notnull =>
            new ParameterizedQueryContext<T>(
                connection,
                null,
                prepared.cc,
                prepared.fieldComparer,
                prepared.sql,
                DynamicQueryExecutorFacade.GetParameters(
                    ref parameters, prepared.parameterPrefix));

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext<T> Query<T, TParameters>(
            this DbConnection connection,
            DbTransaction transaction,
            PreparedParameterizableQueryContext<T> prepared,
            TParameters parameters)
            where T : new()
            where TParameters : notnull =>
            new ParameterizedQueryContext<T>(
                connection,
                transaction,
                prepared.cc,
                prepared.fieldComparer,
                prepared.sql,
                DynamicQueryExecutorFacade.GetParameters(
                    ref parameters, prepared.parameterPrefix));

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PreparedParameterizedQueryContext Parameter<TParameters>(
            this PreparedParameterizableQueryContext prepared,
            Func<TParameters> getter)
            where TParameters : notnull =>
            new PreparedParameterizedQueryContext(
                prepared.cc,
                prepared.sql,
                DynamicQueryExecutorFacade.GetConstructParameters(
                    getter, prepared.parameterPrefix));

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PreparedParameterizedQueryContext<T> Parameter<T, TParameters>(
            this PreparedParameterizableQueryContext<T> prepared,
            Func<TParameters> getter)
            where T : new()
            where TParameters : notnull =>
            new PreparedParameterizedQueryContext<T>(
                prepared.cc,
                prepared.fieldComparer,
                prepared.sql,
                DynamicQueryExecutorFacade.GetConstructParameters(
                    getter, prepared.parameterPrefix));

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext Parameter<TParameters>(
            this ParameterizableQueryContext query,
            TParameters parameters)
            where TParameters : notnull =>
            new ParameterizedQueryContext(
                query.connection,
                query.transaction,
                query.cc,
                query.sql,
                DynamicQueryExecutorFacade.GetParameters(
                    ref parameters, query.parameterPrefix));

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext<T> Parameter<T, TParameters>(
            this ParameterizableQueryContext<T> query,
            TParameters parameters)
            where T : new()
            where TParameters : notnull =>
            new ParameterizedQueryContext<T>(
                query.connection,
                query.transaction,
                query.cc,
                query.fieldComparer,
                query.sql,
                DynamicQueryExecutorFacade.GetParameters(
                    ref parameters, query.parameterPrefix));

        /////////////////////////////////////////////////////////////////////////////

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
    }
}
