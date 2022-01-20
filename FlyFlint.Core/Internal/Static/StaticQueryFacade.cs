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
            String sql,
            TParameters parameters)
            where TParameters : notnull, IParameterExtractable =>
            new ParameterizedQueryContext(
                connection,
                null,
                FlyFlint.Query.DefaultTrait,
                sql.Sql,
                StaticQueryExecutor.GetParameters(
                    ref parameters, FlyFlint.Query.DefaultTrait.parameterPrefix));

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ParameterizedQueryContext Query<TParameters>(
            DbConnection connection,
            DbTransaction transaction,
            String sql,
            TParameters parameters)
            where TParameters : notnull, IParameterExtractable =>
            new ParameterizedQueryContext(
                connection,
                transaction,
                FlyFlint.Query.DefaultTrait,
                sql.Sql,
                StaticQueryExecutor.GetParameters(
                    ref parameters, FlyFlint.Query.DefaultTrait.parameterPrefix));

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ParameterizedQueryContext Query<TParameters>(
            DbConnection connection,
            PreparedPartialQueryContext prepared,
            TParameters parameters)
            where TParameters : notnull, IParameterExtractable
        {
            var built = prepared.builder();
            Debug.Assert(object.ReferenceEquals(built.parameters, Database.defaultParameters));
            return new ParameterizedQueryContext(
                connection,
                null,
                prepared.trait,
                built.sql,
                StaticQueryExecutor.GetParameters(
                    ref parameters, prepared.trait.parameterPrefix));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ParameterizedQueryContext Query<TParameters>(
            DbConnection connection,
            DbTransaction transaction,
            PreparedPartialQueryContext prepared,
            TParameters parameters)
            where TParameters : notnull, IParameterExtractable
        {
            var built = prepared.builder();
            Debug.Assert(object.ReferenceEquals(built.parameters, Database.defaultParameters));
            return new ParameterizedQueryContext(
                connection,
                transaction,
                prepared.trait,
                built.sql,
                StaticQueryExecutor.GetParameters(
                    ref parameters, prepared.trait.parameterPrefix));
        }

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ParameterizedQueryContext<TElement> Query<TElement, TParameters>(
            DbConnection connection,
            PreparedPartialQueryContext<TElement> prepared,
            TParameters parameters)
            where TElement : IDataInjectable, new()
            where TParameters : notnull, IParameterExtractable
        {
            var built = prepared.builder();
            Debug.Assert(object.ReferenceEquals(built.parameters, Database.defaultParameters));
            return new ParameterizedQueryContext<TElement>(
                connection,
                null,
                prepared.trait,
                built.sql,
                StaticQueryExecutor.GetParameters(
                    ref parameters, prepared.trait.parameterPrefix));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ParameterizedQueryContext<TElement> Query<TElement, TParameters>(
            DbConnection connection,
            DbTransaction transaction,
            PreparedPartialQueryContext<TElement> prepared,
            TParameters parameters)
            where TElement : IDataInjectable, new()
            where TParameters : notnull, IParameterExtractable
        {
            var built = prepared.builder();
            Debug.Assert(object.ReferenceEquals(built.parameters, Database.defaultParameters));
            return new ParameterizedQueryContext<TElement>(
                connection,
                transaction,
                prepared.trait,
                built.sql,
                StaticQueryExecutor.GetParameters(
                    ref parameters, prepared.trait.parameterPrefix));
        }

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static PreparedParameterizedQueryContext Parameter<TParameters>(
            PreparedPartialQueryContext prepared,
            TParameters parameters)
            where TParameters : notnull, IParameterExtractable
        {
            var (sql, dps) = prepared.builder();
            Debug.Assert(object.ReferenceEquals(dps, Database.defaultParameters));
            var constructParameters = StaticQueryExecutor.GetConstructParameters(
                () => parameters, prepared.trait.parameterPrefix);
            return new PreparedParameterizedQueryContext(
                prepared.trait,
                () => new QueryParameterBuilderResult(sql, constructParameters()));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static PreparedParameterizedQueryContext<TElement> Parameter<TElement, TParameters>(
            PreparedPartialQueryContext<TElement> prepared,
            TParameters parameters)
            where TElement : IDataInjectable, new()
            where TParameters : notnull, IParameterExtractable
        {
            var (sql, dps) = prepared.builder();
            Debug.Assert(object.ReferenceEquals(dps, Database.defaultParameters));
            var constructParameters = StaticQueryExecutor.GetConstructParameters(
                () => parameters, prepared.trait.parameterPrefix);
            return new PreparedParameterizedQueryContext<TElement>(
                prepared.trait,
                () => new QueryParameterBuilderResult(sql, constructParameters()));
        }

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static PreparedParameterizedQueryContext Parameter<TParameters>(
            PreparedPartialQueryContext prepared,
            Func<TParameters> getter)
            where TParameters : notnull, IParameterExtractable
        {
            var (sql, dps) = prepared.builder();
            Debug.Assert(object.ReferenceEquals(dps, Database.defaultParameters));
            var constructParameters = StaticQueryExecutor.GetConstructParameters(
                getter, prepared.trait.parameterPrefix);
            return new PreparedParameterizedQueryContext(
                prepared.trait,
                () => new QueryParameterBuilderResult(sql, constructParameters()));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static PreparedParameterizedQueryContext<TElement> Parameter<TElement, TParameters>(
            PreparedPartialQueryContext<TElement> prepared,
            Func<TParameters> getter)
            where TElement : IDataInjectable, new()
            where TParameters : notnull, IParameterExtractable
        {
            var (sql, dps) = prepared.builder();
            Debug.Assert(object.ReferenceEquals(dps, Database.defaultParameters));
            var constructParameters = StaticQueryExecutor.GetConstructParameters(
                getter, prepared.trait.parameterPrefix);
            return new PreparedParameterizedQueryContext<TElement>(
                prepared.trait,
                () => new QueryParameterBuilderResult(sql, constructParameters()));
        }

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ParameterizedQueryContext Parameter<TParameters>(
            PartialQueryContext query,
            TParameters parameters)
            where TParameters : notnull, IParameterExtractable =>
            new ParameterizedQueryContext(
                query.connection,
                query.transaction,
                query.trait,
                query.sql,
                StaticQueryExecutor.GetParameters(
                    ref parameters, query.trait.parameterPrefix));

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ParameterizedQueryContext<TElement> Parameter<TElement, TParameters>(
            PartialQueryContext<TElement> query,
            TParameters parameters)
            where TElement : IDataInjectable, new()
            where TParameters : notnull, IParameterExtractable =>
            new ParameterizedQueryContext<TElement>(
                query.connection,
                query.transaction,
                query.trait,
                query.sql,
                StaticQueryExecutor.GetParameters(
                    ref parameters, query.trait.parameterPrefix));

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
        public static TElement ExecuteScalar<TElement>(QueryContext<TElement> query) =>
            StaticQueryExecutor.ExecuteScalar(query);

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
        public static Task<TElement> ExecuteScalarAsync<TElement>(QueryContext<TElement> query) =>
            StaticQueryExecutor.ExecuteScalarAsync(query);

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
