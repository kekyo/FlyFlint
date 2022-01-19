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
using System.Diagnostics;
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
            where TParameters : notnull, IParameterExtractable =>
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
            where TParameters : notnull, IParameterExtractable =>
            new ParameterizedQueryContext(
                connection,
                transaction,
                ConversionContext.Default,
                sql,
                StaticQueryExecutor.GetParameters(
                    ref parameters, FlyFlint.Query.defaultParameterPrefix));

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ParameterizedQueryContext Query<TParameters>(
            DbConnection connection,
            PreparedParameterizableQueryContext prepared,
            TParameters parameters)
            where TParameters : notnull, IParameterExtractable
        {
            var built = prepared.builder();
            Debug.Assert(object.ReferenceEquals(built.parameters, FlyFlint.Query.defaultParameters));
            return new ParameterizedQueryContext(
                connection,
                null,
                prepared.cc,
                built.sql,
                StaticQueryExecutor.GetParameters(
                    ref parameters, prepared.parameterPrefix));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ParameterizedQueryContext Query<TParameters>(
            DbConnection connection,
            DbTransaction transaction,
            PreparedParameterizableQueryContext prepared,
            TParameters parameters)
            where TParameters : notnull, IParameterExtractable
        {
            var built = prepared.builder();
            Debug.Assert(object.ReferenceEquals(built.parameters, FlyFlint.Query.defaultParameters));
            return new ParameterizedQueryContext(
                connection,
                transaction,
                prepared.cc,
                built.sql,
                StaticQueryExecutor.GetParameters(
                    ref parameters, prepared.parameterPrefix));
        }

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ParameterizedQueryContext<TElement> Query<TElement, TParameters>(
            DbConnection connection,
            PreparedParameterizableQueryContext<TElement> prepared,
            TParameters parameters)
            where TElement : new()
            where TParameters : notnull, IParameterExtractable
        {
            var built = prepared.builder();
            Debug.Assert(object.ReferenceEquals(built.parameters, FlyFlint.Query.defaultParameters));
            return new ParameterizedQueryContext<TElement>(
                connection,
                null,
                prepared.cc,
                prepared.fieldComparer,
                built.sql,
                StaticQueryExecutor.GetParameters(
                    ref parameters, prepared.parameterPrefix));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ParameterizedQueryContext<TElement> Query<TElement, TParameters>(
            DbConnection connection,
            DbTransaction transaction,
            PreparedParameterizableQueryContext<TElement> prepared,
            TParameters parameters)
            where TElement : new()
            where TParameters : notnull, IParameterExtractable
        {
            var built = prepared.builder();
            Debug.Assert(object.ReferenceEquals(built.parameters, FlyFlint.Query.defaultParameters));
            return new ParameterizedQueryContext<TElement>(
                connection,
                transaction,
                prepared.cc,
                prepared.fieldComparer,
                built.sql,
                StaticQueryExecutor.GetParameters(
                    ref parameters, prepared.parameterPrefix));
        }

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static PreparedParameterizedQueryContext Parameter<TParameters>(
            PreparedParameterizableQueryContext prepared,
            Func<TParameters> getter)
            where TParameters : notnull, IParameterExtractable
        {
            var (sql, parameters) = prepared.builder();
            Debug.Assert(object.ReferenceEquals(parameters, FlyFlint.Query.defaultParameters));
            var constructParameters = StaticQueryExecutor.GetConstructParameters(
                getter, prepared.parameterPrefix);
            return new PreparedParameterizedQueryContext(
                prepared.cc,
                () => new QueryBuilderResult(sql, constructParameters()));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static PreparedParameterizedQueryContext<TElement> Parameter<TElement, TParameters>(
            PreparedParameterizableQueryContext<TElement> prepared,
            Func<TParameters> getter)
            where TElement : new()
            where TParameters : notnull, IParameterExtractable
        {
            var (sql, parameters) = prepared.builder();
            Debug.Assert(object.ReferenceEquals(parameters, FlyFlint.Query.defaultParameters));
            var constructParameters = StaticQueryExecutor.GetConstructParameters(
                getter, prepared.parameterPrefix);
            return new PreparedParameterizedQueryContext<TElement>(
                prepared.cc,
                prepared.fieldComparer,
                () => new QueryBuilderResult(sql, constructParameters()));
        }

        /////////////////////////////////////////////////////////////////////////////

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
        public static ParameterizedQueryContext<TElement> Parameter<TElement, TParameters>(
            ParameterizableQueryContext<TElement> query,
            TParameters parameters)
            where TElement : new()
            where TParameters : IParameterExtractable =>
            new ParameterizedQueryContext<TElement>(
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
        public static TResult ExecuteScalar<TResult>(QueryContext query) =>
            StaticQueryExecutor.ExecuteScalar<TResult>(query);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IEnumerable<TElement> Execute<TElement>(QueryContext<TElement> query)
            where TElement : IDataInjectable, new() =>
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
        public static Task<TResult> ExecuteScalarAsync<TResult>(QueryContext query) =>
            StaticQueryExecutor.ExecuteScalarAsync<TResult>(query);

#if NET461_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IAsyncEnumerable<TElement> ExecuteAsync<TElement>(QueryContext<TElement> query)
            where TElement : IDataInjectable, new() =>
            StaticQueryExecutor.ExecuteAsync(query);
#else
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void ExecuteAsync<TElement>(QueryContext<TElement> query)
            where TElement : IDataInjectable, new() =>
            throw new InvalidOperationException();
#endif
    }
}
