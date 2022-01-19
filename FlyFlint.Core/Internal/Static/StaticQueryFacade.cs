////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace FlyFlint.Internal.Static
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class StaticQueryFacade
    {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ParameterizedQueryContext Query<TParameters>(
            DbConnection connection,
            string sql,
            TParameters parameters)
            where TParameters : IParameterExtractable =>
            new ParameterizedQueryContext(
                connection,
                null,
                ConversionContext.Default,
                sql,
                StaticQueryExecutor.GetParameters(
                    ref parameters, FlyFlint.Query.defaultParameterPrefix));

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ParameterizedQueryContext Query<TParameters>(
            DbConnection connection,
            DbTransaction transaction,
            string sql,
            TParameters parameters)
            where TParameters : IParameterExtractable =>
            new ParameterizedQueryContext(
                connection,
                transaction,
                ConversionContext.Default,
                sql,
                StaticQueryExecutor.GetParameters(
                    ref parameters, FlyFlint.Query.defaultParameterPrefix));

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ParameterizedQueryContext Query<TParameters>(
            DbConnection connection,
            PreparedParameterizableQueryContext prepared,
            TParameters parameters)
            where TParameters : IParameterExtractable =>
            new ParameterizedQueryContext(
                connection,
                null,
                prepared.cc,
                prepared.sql,
                StaticQueryExecutor.GetParameters(
                    ref parameters, prepared.parameterPrefix));

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ParameterizedQueryContext Query<TParameters>(
            DbConnection connection,
            DbTransaction transaction,
            PreparedParameterizableQueryContext prepared,
            TParameters parameters)
            where TParameters : IParameterExtractable =>
            new ParameterizedQueryContext(
                connection,
                transaction,
                prepared.cc,
                prepared.sql,
                StaticQueryExecutor.GetParameters(
                    ref parameters, prepared.parameterPrefix));

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ParameterizedQueryContext<T> Query<T, TParameters>(
            DbConnection connection,
            PreparedParameterizableQueryContext<T> prepared,
            TParameters parameters)
            where T : new()
            where TParameters : IParameterExtractable =>
            new ParameterizedQueryContext<T>(
                connection,
                null,
                prepared.cc,
                prepared.fieldComparer,
                prepared.sql,
                StaticQueryExecutor.GetParameters(
                    ref parameters, prepared.parameterPrefix));

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ParameterizedQueryContext<T> Query<T, TParameters>(
            DbConnection connection,
            DbTransaction transaction,
            PreparedParameterizableQueryContext<T> prepared,
            TParameters parameters)
            where T : new()
            where TParameters : IParameterExtractable =>
            new ParameterizedQueryContext<T>(
                connection,
                transaction,
                prepared.cc,
                prepared.fieldComparer,
                prepared.sql,
                StaticQueryExecutor.GetParameters(
                    ref parameters, prepared.parameterPrefix));

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static PreparedParameterizedQueryContext Parameter<TParameters>(
            PreparedParameterizableQueryContext prepared,
            Func<TParameters> getter)
            where TParameters : notnull, IParameterExtractable =>
            new PreparedParameterizedQueryContext(
                prepared.cc,
                prepared.sql,
                StaticQueryExecutor.GetConstructParameters(
                    getter, prepared.parameterPrefix));

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static PreparedParameterizedQueryContext<T> Parameter<T, TParameters>(
            PreparedParameterizableQueryContext<T> prepared,
            Func<TParameters> getter)
            where T : new()
            where TParameters : notnull, IParameterExtractable =>
            new PreparedParameterizedQueryContext<T>(
                prepared.cc,
                prepared.fieldComparer,
                prepared.sql,
                StaticQueryExecutor.GetConstructParameters(
                    getter, prepared.parameterPrefix));

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ParameterizedQueryContext Parameter<TParameters>(
            ParameterizableQueryContext query,
            TParameters parameters)
            where TParameters : notnull, IParameterExtractable =>
            new ParameterizedQueryContext(
                query.connection,
                query.transaction,
                query.cc,
                query.sql,
                StaticQueryExecutor.GetParameters(
                    ref parameters, query.parameterPrefix));

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ParameterizedQueryContext<T> Parameter<T, TParameters>(
            ParameterizableQueryContext<T> query,
            TParameters parameters)
            where T : new()
            where TParameters : IParameterExtractable =>
            new ParameterizedQueryContext<T>(
                query.connection,
                query.transaction,
                query.cc,
                query.fieldComparer,
                query.sql,
                StaticQueryExecutor.GetParameters(
                    ref parameters, query.parameterPrefix));

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void ExecuteNonQuery(QueryContext query) =>
            StaticQueryExecutor.ExecuteNonQuery(query);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static T ExecuteScalar<T>(QueryContext query) =>
            StaticQueryExecutor.ExecuteScalar<T>(query);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IEnumerable<T> Execute<T>(QueryContext<T> query)
            where T : IDataInjectable, new() =>
            StaticQueryExecutor.Execute(query);

        /////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Task ExecuteNonQueryAsync(QueryContext query) =>
            StaticQueryExecutor.ExecuteNonQueryAsync(query);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Task<T> ExecuteScalarAsync<T>(QueryContext query) =>
            StaticQueryExecutor.ExecuteScalarAsync<T>(query);

#if NET461_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IAsyncEnumerable<T> ExecuteAsync<T>(QueryContext<T> query)
            where T : IDataInjectable, new() =>
            StaticQueryExecutor.ExecuteAsync(query);
#else
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void ExecuteAsync<T>(QueryContext<T> query)
            where T : IDataInjectable, new() =>
            throw new InvalidOperationException();
#endif
    }
}
