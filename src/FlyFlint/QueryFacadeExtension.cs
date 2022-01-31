////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Context;
using FlyFlint.Internal;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace FlyFlint
{
    public static class QueryFacadeExtension
    {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext Query<TParameters>(
            this DbConnection connection,
            String sql,
            TParameters parameters)
            where TParameters : notnull =>
            new ParameterizedQueryContext(
                connection,
                null,
                FlyFlint.Query.DefaultTrait,
                sql.Sql,
                QueryExecutor.GetParameters(
                    FlyFlint.Query.DefaultTrait.cc,
                    FlyFlint.Query.DefaultTrait.parameterPrefix,
                    ref parameters));

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext Query<TParameters>(
            this DbConnection connection,
            DbTransaction transaction,
            String sql,
            TParameters parameters)
            where TParameters : notnull =>
            new ParameterizedQueryContext(
                connection,
                transaction,
                FlyFlint.Query.DefaultTrait,
                sql.Sql,
                QueryExecutor.GetParameters(
                    FlyFlint.Query.DefaultTrait.cc,
                    FlyFlint.Query.DefaultTrait.parameterPrefix,
                    ref parameters));

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext Query<TParameters>(
            this DbConnection connection,
            PreparedPartialQueryContext prepared,
            TParameters parameters)
            where TParameters : notnull
        {
            var built = prepared.builder();
            Debug.Assert(object.ReferenceEquals(built.parameters, QueryHelper.DefaultParameters));
            return new ParameterizedQueryContext(
                connection,
                null,
                prepared.trait,
                built.sql,
                QueryExecutor.GetParameters(
                    prepared.trait.cc,
                    prepared.trait.parameterPrefix,
                    ref parameters));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext Query<TParameters>(
            this DbConnection connection,
            DbTransaction transaction,
            PreparedPartialQueryContext prepared,
            TParameters parameters)
            where TParameters : notnull
        {
            var built = prepared.builder();
            Debug.Assert(object.ReferenceEquals(built.parameters, QueryHelper.DefaultParameters));
            return new ParameterizedQueryContext(
                connection,
                transaction,
                prepared.trait,
                built.sql,
                QueryExecutor.GetParameters(
                    prepared.trait.cc,
                    prepared.trait.parameterPrefix,
                    ref parameters));
        }

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext<TElement> Query<TElement, TParameters>(
            this DbConnection connection,
            PreparedPartialQueryContext<TElement> prepared,
            TParameters parameters)
            where TElement : notnull, new()
            where TParameters : notnull
        {
            var built = prepared.builder();
            Debug.Assert(object.ReferenceEquals(built.parameters, QueryHelper.DefaultParameters));
            return new ParameterizedQueryContext<TElement>(
                connection,
                null,
                prepared.trait,
                built.sql,
                QueryExecutor.GetParameters(
                    prepared.trait.cc,
                    prepared.trait.parameterPrefix,
                    ref parameters));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext<TElement> Query<TElement, TParameters>(
            this DbConnection connection,
            DbTransaction transaction,
            PreparedPartialQueryContext<TElement> prepared,
            TParameters parameters)
            where TElement : notnull, new()
            where TParameters : notnull
        {
            var built = prepared.builder();
            Debug.Assert(object.ReferenceEquals(built.parameters, QueryHelper.DefaultParameters));
            return new ParameterizedQueryContext<TElement>(
                connection,
                transaction,
                prepared.trait,
                built.sql,
                QueryExecutor.GetParameters(
                    prepared.trait.cc,
                    prepared.trait.parameterPrefix,
                    ref parameters));
        }

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PreparedParameterizedQueryContext Parameter<TParameters>(
            this PreparedPartialQueryContext prepared,
            TParameters parameters)
            where TParameters : notnull
        {
            var (sql, dps) = prepared.builder();
            Debug.Assert(object.ReferenceEquals(dps, QueryHelper.DefaultParameters));
            var constructParameters = QueryExecutor.GetConstructParameters(
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
            this PreparedPartialQueryContext<TElement> prepared,
            TParameters parameters)
            where TElement : notnull, new()
            where TParameters : notnull
        {
            var (sql, dps) = prepared.builder();
            Debug.Assert(object.ReferenceEquals(dps, QueryHelper.DefaultParameters));
            var constructParameters = QueryExecutor.GetConstructParameters(
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
            this PreparedPartialQueryContext prepared,
            Func<TParameters> getter)
            where TParameters : notnull
        {
            var (sql, dps) = prepared.builder();
            Debug.Assert(object.ReferenceEquals(dps, QueryHelper.DefaultParameters));
            var constructParameters = QueryExecutor.GetConstructParameters(
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
            this PreparedPartialQueryContext<TElement> prepared,
            Func<TParameters> getter)
            where TElement : notnull, new()
            where TParameters : notnull
        {
            var (sql, dps) = prepared.builder();
            Debug.Assert(object.ReferenceEquals(dps, QueryHelper.DefaultParameters));
            var constructParameters = QueryExecutor.GetConstructParameters(
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
            this PartialQueryContext query,
            TParameters parameters)
            where TParameters : notnull =>
            new ParameterizedQueryContext(
                query.connection,
                query.transaction,
                query.trait,
                query.sql,
                QueryExecutor.GetParameters(
                    query.trait.cc,
                    query.trait.parameterPrefix,
                    ref parameters));

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext<TElement> Parameter<TElement, TParameters>(
            this PartialQueryContext<TElement> query,
            TParameters parameters)
            where TElement : notnull, new()
            where TParameters : notnull =>
            new ParameterizedQueryContext<TElement>(
                query.connection,
                query.transaction,
                query.trait,
                query.sql,
                QueryExecutor.GetParameters(
                    query.trait.cc,
                    query.trait.parameterPrefix,
                    ref parameters));

        /////////////////////////////////////////////////////////////////////////////

#if NET461_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        public static async IAsyncEnumerable<TElement> ExecuteAsync<TElement>(
            this QueryContext<TElement> query,
            [EnumeratorCancellation] CancellationToken ct = default)
            where TElement : notnull, new()
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                using (var reader = await command.ExecuteReaderAsync(ct).ConfigureAwait(false))
                {
                    if (await reader.ReadAsync(ct).ConfigureAwait(false))
                    {
                        var element = new TElement();

                        var injector = QueryExecutor.GetDataInjector(
                            query.trait.cc, query.trait.fieldComparer, reader, ref element);

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
#else
        [Obsolete("Before net461 platform, it is not supported async enumeration. Consider upgrades to net461 or upper, or `Execute()` method with `FlyFlint.Synchronized` namespace instead.", true)]
        public static void ExecuteAsync<TElement>(
            this QueryContext<TElement> query, CancellationToken ct = default)
            where TElement : new() =>
            throw new InvalidOperationException();
#endif
    }
}
