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
using System.Threading.Tasks;

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
                QueryHelper.CurrentDefaultTrait,
                sql.Sql,
                StaticQueryExecutor.GetParameters(
                    QueryHelper.CurrentDefaultTrait.cc,
                    QueryHelper.CurrentDefaultTrait.parameterPrefix,
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
                QueryHelper.CurrentDefaultTrait,
                sql.Sql,
                StaticQueryExecutor.GetParameters(
                    QueryHelper.CurrentDefaultTrait.cc,
                    QueryHelper.CurrentDefaultTrait.parameterPrefix,
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
        public static ParameterizedQueryContext<TRecord> Query<TRecord, TParameters>(
            DbConnection connection,
            PreparedPartialQueryContext<TRecord> prepared,
            TParameters parameters)
            where TRecord : notnull, IRecordInjectable, new()
            where TParameters : notnull, IParameterExtractable
        {
            var built = prepared.builder();
            Debug.Assert(object.ReferenceEquals(built.parameters, QueryHelper.DefaultParameters));
            return new ParameterizedQueryContext<TRecord>(
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
        public static ParameterizedQueryContext<TRecord> Query<TRecord, TParameters>(
            DbConnection connection,
            DbTransaction transaction,
            PreparedPartialQueryContext<TRecord> prepared,
            TParameters parameters)
            where TRecord : notnull, IRecordInjectable, new()
            where TParameters : notnull, IParameterExtractable
        {
            var built = prepared.builder();
            Debug.Assert(object.ReferenceEquals(built.parameters, QueryHelper.DefaultParameters));
            return new ParameterizedQueryContext<TRecord>(
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
        public static PreparedParameterizedQueryContext<TRecord> Parameter<TRecord, TParameters>(
            PreparedPartialQueryContext<TRecord> prepared,
            TParameters parameters)
            where TRecord : notnull, IRecordInjectable, new()
            where TParameters : notnull, IParameterExtractable
        {
            var (sql, dps) = prepared.builder();
            Debug.Assert(object.ReferenceEquals(dps, QueryHelper.DefaultParameters));
            var constructParameters = StaticQueryExecutor.GetConstructParameters(
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
        public static PreparedParameterizedQueryContext<TRecord> Parameter<TRecord, TParameters>(
            PreparedPartialQueryContext<TRecord> prepared,
            Func<TParameters> getter)
            where TRecord : notnull, IRecordInjectable, new()
            where TParameters : notnull, IParameterExtractable
        {
            var (sql, dps) = prepared.builder();
            Debug.Assert(object.ReferenceEquals(dps, QueryHelper.DefaultParameters));
            var constructParameters = StaticQueryExecutor.GetConstructParameters(
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
        public static ParameterizedQueryContext<TRecord> Parameter<TRecord, TParameters>(
            PartialQueryContext<TRecord> query,
            TParameters parameters)
            where TRecord : new()
            where TParameters : notnull, IParameterExtractable =>
            new ParameterizedQueryContext<TRecord>(
                query.connection,
                query.transaction,
                query.trait,
                query.sql,
                StaticQueryExecutor.GetParameters(
                    query.trait.cc,
                    query.trait.parameterPrefix,
                    ref parameters));

        /////////////////////////////////////////////////////////////////////////////

        private static IEnumerable<TRecord> InternalExecute<TRecord>(
            QueryContext<TRecord> query)
            where TRecord : notnull, IRecordInjectable, new()
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var record = new TRecord();

                        var injector = StaticQueryExecutor.GetRecordInjector<TRecord>(
                            query.trait.cc, query.trait.fieldComparer, reader, record);

                        injector(ref record);
                        yield return record;

                        while (reader.Read())
                        {
                            record = new TRecord();
                            injector(ref record);
                            yield return record;
                        }
                    }
                }
            }
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static IEnumerable<TRecord> Execute<TRecord>(
            ParameterizedQueryContext<TRecord> query)
            where TRecord : notnull, IRecordInjectable, new() =>
            InternalExecute(query);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static IEnumerable<TRecord> ExecuteNonParameterized<TRecord>(
            PartialQueryContext<TRecord> query)
            where TRecord : notnull, IRecordInjectable, new() =>
            InternalExecute(query);

        /////////////////////////////////////////////////////////////////////////////

        private static TRecord[] InternalExecuteImmediately<TRecord>(
            QueryContext<TRecord> query)
            where TRecord : notnull, IRecordInjectable, new()
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                using (var reader = command.ExecuteReader())
                {
                    var results = new List<TRecord>();

                    if (reader.Read())
                    {
                        var record = new TRecord();

                        var injector = StaticQueryExecutor.GetRecordInjector<TRecord>(
                            query.trait.cc, query.trait.fieldComparer, reader, record);

                        injector(ref record);
                        results.Add(record);

                        while (reader.Read())
                        {
                            record = new TRecord();
                            injector(ref record);
                            results.Add(record);
                        }
                    }

                    return results.ToArray();
                }
            }
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static TRecord[] ExecuteImmediately<TRecord>(
            ParameterizedQueryContext<TRecord> query)
            where TRecord : notnull, IRecordInjectable, new() =>
            InternalExecuteImmediately(query);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static TRecord[] ExecuteImmediatelyNonParameterized<TRecord>(
            PartialQueryContext<TRecord> query)
            where TRecord : notnull, IRecordInjectable, new() =>
            InternalExecuteImmediately(query);

        /////////////////////////////////////////////////////////////////////////////

#if NET461_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        private static async Task<TRecord[]> InternalExecuteImmediatelyAsync<TRecord>(
            QueryContext<TRecord> query,
            CancellationToken ct)
            where TRecord : notnull, IRecordInjectable, new()
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                using (var reader = await command.ExecuteReaderAsync(ct).ConfigureAwait(false))
                {
                    var results = new List<TRecord>();

                    if (await reader.ReadAsync(ct).ConfigureAwait(false))
                    {
                        var record = new TRecord();

                        var injector = StaticQueryExecutor.GetRecordInjector<TRecord>(
                            query.trait.cc, query.trait.fieldComparer, reader, record);

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

                    return results.ToArray();
                }
            }
        }
#else
        private static Task<TRecord[]> InternalExecuteImmediatelyAsync<TRecord>(
            QueryContext<TRecord> query,
            CancellationToken ct)
            where TRecord : notnull, IRecordInjectable, new() =>
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

                            var injector = StaticQueryExecutor.GetRecordInjector<TRecord>(
                                query.trait.cc, query.trait.fieldComparer, reader, record);

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

                        return results.ToArray();
                    }
                }
            });
#endif

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<TRecord[]> ExecuteImmediatelyAsync<TRecord>(
            ParameterizedQueryContext<TRecord> query,
            CancellationToken ct = default)
            where TRecord : notnull, IRecordInjectable, new() =>
            InternalExecuteImmediatelyAsync(query, ct);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<TRecord[]> ExecuteImmediatelyNonParameterizedAsync<TRecord>(
            PartialQueryContext<TRecord> query,
            CancellationToken ct = default)
            where TRecord : notnull, IRecordInjectable, new() =>
            InternalExecuteImmediatelyAsync(query, ct);

        /////////////////////////////////////////////////////////////////////////////

#if NET461_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        private static async IAsyncEnumerable<TRecord> InternalExecuteAsync<TRecord>(
            QueryContext<TRecord> query,
            [EnumeratorCancellation] CancellationToken ct)
            where TRecord : notnull, IRecordInjectable, new()
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                using (var reader = await command.ExecuteReaderAsync(ct).ConfigureAwait(false))
                {
                    if (await reader.ReadAsync(ct).ConfigureAwait(false))
                    {
                        var record = new TRecord();

                        var injector = StaticQueryExecutor.GetRecordInjector<TRecord>(
                            query.trait.cc, query.trait.fieldComparer, reader, record);

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
            ParameterizedQueryContext<TRecord> query,
            CancellationToken ct = default)
            where TRecord : notnull, IRecordInjectable, new() =>
            InternalExecuteAsync(query, ct);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IAsyncEnumerable<TRecord> ExecuteNonParameterizedAsync<TRecord>(
            PartialQueryContext<TRecord> query,
            CancellationToken ct = default)
            where TRecord : notnull, IRecordInjectable, new() =>
            InternalExecuteAsync(query, ct);
#endif
    }
}
