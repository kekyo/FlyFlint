////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Collections;
using FlyFlint.Context;
using FlyFlint.Internal;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
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
            String sql,
            TParameters parameters)
            where TParameters : notnull =>
            new ParameterizedQueryContext(
                connection,
                null,
                QueryHelper.CurrentDefaultTrait,
                sql.Sql,
                QueryExecutor.GetParameters(
                    QueryHelper.CurrentDefaultTrait.cc,
                    QueryHelper.CurrentDefaultTrait.parameterPrefix,
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
                QueryHelper.CurrentDefaultTrait,
                sql.Sql,
                QueryExecutor.GetParameters(
                    QueryHelper.CurrentDefaultTrait.cc,
                    QueryHelper.CurrentDefaultTrait.parameterPrefix,
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
        public static ParameterizedQueryContext<TRecord> Query<TRecord, TParameters>(
            this DbConnection connection,
            PreparedPartialQueryContext<TRecord> prepared,
            TParameters parameters)
            where TRecord : notnull, new()
            where TParameters : notnull
        {
            var built = prepared.builder();
            Debug.Assert(object.ReferenceEquals(built.parameters, QueryHelper.DefaultParameters));
            return new ParameterizedQueryContext<TRecord>(
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
        public static ParameterizedQueryContext<TRecord> Query<TRecord, TParameters>(
            this DbConnection connection,
            DbTransaction transaction,
            PreparedPartialQueryContext<TRecord> prepared,
            TParameters parameters)
            where TRecord : notnull, new()
            where TParameters : notnull
        {
            var built = prepared.builder();
            Debug.Assert(object.ReferenceEquals(built.parameters, QueryHelper.DefaultParameters));
            return new ParameterizedQueryContext<TRecord>(
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
        public static PreparedParameterizedQueryContext<TRecord> Parameter<TRecord, TParameters>(
            this PreparedPartialQueryContext<TRecord> prepared,
            TParameters parameters)
            where TRecord : notnull, new()
            where TParameters : notnull
        {
            var (sql, dps) = prepared.builder();
            Debug.Assert(object.ReferenceEquals(dps, QueryHelper.DefaultParameters));
            var constructParameters = QueryExecutor.GetConstructParameters(
                prepared.trait.cc,
                prepared.trait.parameterPrefix,
                () => parameters);
            return new PreparedParameterizedQueryContext<TRecord>(
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
        public static PreparedParameterizedQueryContext<TRecord> Parameter<TRecord, TParameters>(
            this PreparedPartialQueryContext<TRecord> prepared,
            Func<TParameters> getter)
            where TRecord : notnull, new()
            where TParameters : notnull
        {
            var (sql, dps) = prepared.builder();
            Debug.Assert(object.ReferenceEquals(dps, QueryHelper.DefaultParameters));
            var constructParameters = QueryExecutor.GetConstructParameters(
                prepared.trait.cc,
                prepared.trait.parameterPrefix,
                getter);
            return new PreparedParameterizedQueryContext<TRecord>(
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
        public static ParameterizedQueryContext<TRecord> Parameter<TRecord, TParameters>(
            this PartialQueryContext<TRecord> query,
            TParameters parameters)
            where TRecord : notnull, new()
            where TParameters : notnull =>
            new ParameterizedQueryContext<TRecord>(
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
        private static async Task<ReadOnlyList<TRecord>> InternalExecuteImmediatelyAsync<TRecord>(
            QueryContext<TRecord> query,
            CancellationToken ct)
            where TRecord : notnull, new()
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    var results = new List<TRecord>();

                    if (await reader.ReadAsync(ct).ConfigureAwait(false))
                    {
                        var record = new TRecord();

                        var injector = QueryExecutor.GetRecordInjector(
                            query.trait.cc, query.trait.fieldComparer, reader, ref record);

                        injector(ref record);
                        var prefetchAwaitable = reader.ReadAsync(ct).ConfigureAwait(false);
                        results.Add(record);

                        while (await prefetchAwaitable)
                        {
                            record = new TRecord();
                            injector(ref record);
                            prefetchAwaitable = reader.ReadAsync(ct).ConfigureAwait(false);
                            results.Add(record);
                        }
                    }

                    return new ReadOnlyList<TRecord>(results);
                }
            }
        }
#else
        private static Task<ReadOnlyList<TRecord>> InternalExecuteImmediatelyAsync<TRecord>(
            QueryContext<TRecord> query,
            CancellationToken ct)
            where TRecord : notnull, new() =>
            Task.Factory.StartNew(() =>
            {
                using (var command = QueryHelper.CreateCommand(
                    query.connection, query.transaction, query.sql, query.parameters))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        ct.ThrowIfCancellationRequested();

                        var results = new List<TRecord>();

                        if (reader.Read())
                        {
                            ct.ThrowIfCancellationRequested();

                            var record = new TRecord();

                            var injector = QueryExecutor.GetRecordInjector(
                                query.trait.cc, query.trait.fieldComparer, reader, ref record);

                            injector(ref record);
                            results.Add(record);

                            while (reader.Read())
                            {
                                ct.ThrowIfCancellationRequested();

                                record = new TRecord();
                                injector(ref record);
                                results.Add(record);
                            }
                        }

                        return new ReadOnlyList<TRecord>(results);
                    }
                }
            });
#endif

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<ReadOnlyList<TRecord>> ExecuteImmediatelyAsync<TRecord>(
            this ParameterizedQueryContext<TRecord> query,
            CancellationToken ct = default)
            where TRecord : notnull, new() =>
            InternalExecuteImmediatelyAsync(query, ct);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<ReadOnlyList<TRecord>> ExecuteImmediatelyNonParameterizedAsync<TRecord>(
            this PartialQueryContext<TRecord> query,
            CancellationToken ct = default)
            where TRecord : notnull, new() =>
            InternalExecuteImmediatelyAsync(query, ct);

        /////////////////////////////////////////////////////////////////////////////

#if NET461_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        private static async IAsyncEnumerable<TRecord> InternalExecuteAsync<TRecord>(
            QueryContext<TRecord> query,
            [EnumeratorCancellation] CancellationToken ct)
            where TRecord : notnull, new()
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                using (var reader = await command.ExecuteReaderAsync(ct).ConfigureAwait(false))
                {
                    if (await reader.ReadAsync(ct).ConfigureAwait(false))
                    {
                        var record = new TRecord();

                        var injector = QueryExecutor.GetRecordInjector(
                            query.trait.cc, query.trait.fieldComparer, reader, ref record);

                        injector(ref record);
                        var prefetchAwaitable = reader.ReadAsync(ct).ConfigureAwait(false);
                        yield return record;

                        while (await prefetchAwaitable)
                        {
                            record = new TRecord();
                            injector(ref record);
                            prefetchAwaitable = reader.ReadAsync(ct).ConfigureAwait(false);
                            yield return record;
                        }
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IAsyncEnumerable<TRecord> ExecuteAsync<TRecord>(
            this ParameterizedQueryContext<TRecord> query,
            CancellationToken ct = default)
            where TRecord : notnull, new() =>
            InternalExecuteAsync(query, ct);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IAsyncEnumerable<TRecord> ExecuteNonParameterizedAsync<TRecord>(
            this PartialQueryContext<TRecord> query,
            CancellationToken ct = default)
            where TRecord : notnull, new() =>
            InternalExecuteAsync(query, ct);
#else
        [Obsolete("Before net461 platform, it is not supported async enumeration. Consider upgrades to net461 or upper, or `Execute()` method with `FlyFlint.Synchronized` namespace instead.", true)]
        public static void ExecuteAsync<TRecord>(
            this ParameterizedQueryContext<TRecord> query, CancellationToken ct = default)
            where TRecord : new() =>
            throw new InvalidOperationException();
        [Obsolete("Before net461 platform, it is not supported async enumeration. Consider upgrades to net461 or upper, or `Execute()` method with `FlyFlint.Synchronized` namespace instead.", true)]
        public static void ExecuteNonParameterizedAsync<TRecord>(
            this PartialQueryContext<TRecord> query, CancellationToken ct = default)
            where TRecord : new() =>
            throw new InvalidOperationException();
#endif
    }
}
