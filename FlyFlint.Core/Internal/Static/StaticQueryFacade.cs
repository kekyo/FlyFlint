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
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace FlyFlint.Internal.Static
{
    public static class StaticQueryFacade
    {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
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
                    FlyFlint.Query.DefaultTrait.cc,
                    FlyFlint.Query.DefaultTrait.parameterPrefix,
                    ref parameters));

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
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
                    FlyFlint.Query.DefaultTrait.cc,
                    FlyFlint.Query.DefaultTrait.parameterPrefix,
                    ref parameters));

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext Query<TParameters>(
            DbConnection connection,
            PreparedPartialQueryContext prepared,
            TParameters parameters)
            where TParameters : notnull, IParameterExtractable
        {
            var built = prepared.builder();
            Debug.Assert(object.ReferenceEquals(built.parameters, QueryHelper.DefaultParameters));
            return new ParameterizedQueryContext(
                connection,
                null,
                prepared.trait,
                built.sql,
                StaticQueryExecutor.GetParameters(
                    prepared.trait.cc,
                    prepared.trait.parameterPrefix,
                    ref parameters));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext Query<TParameters>(
            DbConnection connection,
            DbTransaction transaction,
            PreparedPartialQueryContext prepared,
            TParameters parameters)
            where TParameters : notnull, IParameterExtractable
        {
            var built = prepared.builder();
            Debug.Assert(object.ReferenceEquals(built.parameters, QueryHelper.DefaultParameters));
            return new ParameterizedQueryContext(
                connection,
                transaction,
                prepared.trait,
                built.sql,
                StaticQueryExecutor.GetParameters(
                    prepared.trait.cc,
                    prepared.trait.parameterPrefix,
                    ref parameters));
        }

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext<TElement> Query<TElement, TParameters>(
            DbConnection connection,
            PreparedPartialQueryContext<TElement> prepared,
            TParameters parameters)
            where TElement : notnull, IDataInjectable, new()
            where TParameters : notnull, IParameterExtractable
        {
            var built = prepared.builder();
            Debug.Assert(object.ReferenceEquals(built.parameters, QueryHelper.DefaultParameters));
            return new ParameterizedQueryContext<TElement>(
                connection,
                null,
                prepared.trait,
                built.sql,
                StaticQueryExecutor.GetParameters(
                    prepared.trait.cc,
                    prepared.trait.parameterPrefix,
                    ref parameters));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext<TElement> Query<TElement, TParameters>(
            DbConnection connection,
            DbTransaction transaction,
            PreparedPartialQueryContext<TElement> prepared,
            TParameters parameters)
            where TElement : notnull, IDataInjectable, new()
            where TParameters : notnull, IParameterExtractable
        {
            var built = prepared.builder();
            Debug.Assert(object.ReferenceEquals(built.parameters, QueryHelper.DefaultParameters));
            return new ParameterizedQueryContext<TElement>(
                connection,
                transaction,
                prepared.trait,
                built.sql,
                StaticQueryExecutor.GetParameters(
                    prepared.trait.cc,
                    prepared.trait.parameterPrefix,
                    ref parameters));
        }

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PreparedParameterizedQueryContext Parameter<TParameters>(
            PreparedPartialQueryContext prepared,
            TParameters parameters)
            where TParameters : notnull, IParameterExtractable
        {
            var (sql, dps) = prepared.builder();
            Debug.Assert(object.ReferenceEquals(dps, QueryHelper.DefaultParameters));
            var constructParameters = StaticQueryExecutor.GetConstructParameters(
                prepared.trait.cc,
                prepared.trait.parameterPrefix,
                () => parameters);
            return new PreparedParameterizedQueryContext(
                prepared.trait,
                () => new QueryParameterBuilderResult(sql, constructParameters()));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PreparedParameterizedQueryContext<TElement> Parameter<TElement, TParameters>(
            PreparedPartialQueryContext<TElement> prepared,
            TParameters parameters)
            where TElement : notnull, IDataInjectable, new()
            where TParameters : notnull, IParameterExtractable
        {
            var (sql, dps) = prepared.builder();
            Debug.Assert(object.ReferenceEquals(dps, QueryHelper.DefaultParameters));
            var constructParameters = StaticQueryExecutor.GetConstructParameters(
                prepared.trait.cc,
                prepared.trait.parameterPrefix,
                () => parameters);
            return new PreparedParameterizedQueryContext<TElement>(
                prepared.trait,
                () => new QueryParameterBuilderResult(sql, constructParameters()));
        }

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PreparedParameterizedQueryContext Parameter<TParameters>(
            PreparedPartialQueryContext prepared,
            Func<TParameters> getter)
            where TParameters : notnull, IParameterExtractable
        {
            var (sql, dps) = prepared.builder();
            Debug.Assert(object.ReferenceEquals(dps, QueryHelper.DefaultParameters));
            var constructParameters = StaticQueryExecutor.GetConstructParameters(
                prepared.trait.cc,
                prepared.trait.parameterPrefix,
                getter);
            return new PreparedParameterizedQueryContext(
                prepared.trait,
                () => new QueryParameterBuilderResult(sql, constructParameters()));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PreparedParameterizedQueryContext<TElement> Parameter<TElement, TParameters>(
            PreparedPartialQueryContext<TElement> prepared,
            Func<TParameters> getter)
            where TElement : notnull, IDataInjectable, new()
            where TParameters : notnull, IParameterExtractable
        {
            var (sql, dps) = prepared.builder();
            Debug.Assert(object.ReferenceEquals(dps, QueryHelper.DefaultParameters));
            var constructParameters = StaticQueryExecutor.GetConstructParameters(
                prepared.trait.cc,
                prepared.trait.parameterPrefix,
                getter);
            return new PreparedParameterizedQueryContext<TElement>(
                prepared.trait,
                () => new QueryParameterBuilderResult(sql, constructParameters()));
        }

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
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
                    query.trait.cc,
                    query.trait.parameterPrefix,
                    ref parameters));

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext<TElement> Parameter<TElement, TParameters>(
            PartialQueryContext<TElement> query,
            TParameters parameters)
            where TElement : new()
            where TParameters : notnull, IParameterExtractable =>
            new ParameterizedQueryContext<TElement>(
                query.connection,
                query.transaction,
                query.trait,
                query.sql,
                StaticQueryExecutor.GetParameters(
                    query.trait.cc,
                    query.trait.parameterPrefix,
                    ref parameters));

        /////////////////////////////////////////////////////////////////////////////

        public static IEnumerable<TElement> Execute<TElement>(
            QueryContext<TElement> query)
            where TElement : notnull, IDataInjectable, new()
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var element = new TElement();

                        var injector = StaticQueryExecutor.GetDataInjector<TElement>(
                            query.trait.cc, query.trait.fieldComparer, reader, element);

                        injector(ref element);
                        yield return element;

                        while (reader.Read())
                        {
                            element = new TElement();
                            injector(ref element);
                            yield return element;
                        }
                    }
                }
            }
        }

#if NET461_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        public static async IAsyncEnumerable<TElement> ExecuteAsync<TElement>(
            QueryContext<TElement> query,
            [EnumeratorCancellation] CancellationToken ct = default)
            where TElement : notnull, IDataInjectable, new()
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                using (var reader = await command.ExecuteReaderAsync(ct).ConfigureAwait(false))
                {
                    if (await reader.ReadAsync(ct).ConfigureAwait(false))
                    {
                        var element = new TElement();

                        var injector = StaticQueryExecutor.GetDataInjector<TElement>(
                            query.trait.cc, query.trait.fieldComparer, reader, element);

                        injector(ref element);
                        var prefetchAwaitable = reader.ReadAsync(ct).ConfigureAwait(false);
                        yield return element;

                        while (await prefetchAwaitable)
                        {
                            element = new TElement();
                            injector(ref element);
                            prefetchAwaitable = reader.ReadAsync(ct).ConfigureAwait(false);
                            yield return element;
                        }
                    }
                }
            }
        }
#endif
    }
}
