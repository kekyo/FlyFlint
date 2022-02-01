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
#if NET35 || NET40
using FlyFlint.Utilities;
#endif
using System;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace FlyFlint
{
    public static class QueryExtension
    {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PartialQueryContext Query(
            this DbConnection connection,
            String sql) =>
            QueryHelper.CurrentDefaultTrait.Query(connection, sql);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext Query(
            this DbConnection connection,
            FormattableString sql) =>
            QueryHelper.CurrentDefaultTrait.Query(connection, sql);

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PartialQueryContext Query(
            this DbConnection connection,
            DbTransaction? transaction,
            String sql) =>
            QueryHelper.CurrentDefaultTrait.Query(connection, transaction, sql);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext Query(
            this DbConnection connection,
            DbTransaction? transaction,
            FormattableString sql) =>
            QueryHelper.CurrentDefaultTrait.Query(connection, transaction, sql);

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PartialQueryContext<TRecord> Query<TRecord>(
            this DbConnection connection,
            String sql)
            where TRecord : new() =>
            QueryHelper.CurrentDefaultTrait.Query<TRecord>(connection, sql);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext<TRecord> Query<TRecord>(
            this DbConnection connection,
            FormattableString sql)
            where TRecord : new() =>
            QueryHelper.CurrentDefaultTrait.Query<TRecord>(connection, sql);

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PartialQueryContext<TRecord> Query<TRecord>(
            this DbConnection connection,
            DbTransaction? transaction,
            String sql)
            where TRecord : new() =>
            QueryHelper.CurrentDefaultTrait.Query<TRecord>(connection, transaction, sql);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext<TRecord> Query<TRecord>(
            this DbConnection connection,
            DbTransaction? transaction,
            FormattableString sql)
            where TRecord : new() =>
            QueryHelper.CurrentDefaultTrait.Query<TRecord>(connection, transaction, sql);

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext Query(
            this DbConnection connection,
            PreparedParameterizedQueryContext prepared)
        {
            var built = prepared.builder();
            return new ParameterizedQueryContext(
                connection,
                null,
                prepared.trait,
                built.sql,
                built.parameters);
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PartialQueryContext Query(
            this DbConnection connection,
            PreparedPartialQueryContext prepared)
        {
            var built = prepared.builder();
            Debug.Assert(object.ReferenceEquals(built.parameters, QueryHelper.DefaultParameters));
            return new PartialQueryContext(
                connection,
                null,
                prepared.trait,
                built.sql);
        }

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext Query(
            this DbConnection connection,
            DbTransaction transaction,
            PreparedParameterizedQueryContext prepared)
        {
            var built = prepared.builder();
            return new ParameterizedQueryContext(
                connection,
                transaction,
                prepared.trait,
                built.sql,
                built.parameters);
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PartialQueryContext Query(
            this DbConnection connection,
            DbTransaction transaction,
            PreparedPartialQueryContext prepared)
        {
            var built = prepared.builder();
            Debug.Assert(object.ReferenceEquals(built.parameters, QueryHelper.DefaultParameters));
            return new PartialQueryContext(
                connection,
                transaction,
                prepared.trait,
                built.sql);
        }

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext<TRecord> Query<TRecord>(
            this DbConnection connection,
            PreparedParameterizedQueryContext<TRecord> prepared)
            where TRecord : new()
        {
            var built = prepared.builder();
            return new ParameterizedQueryContext<TRecord>(
                connection,
                null,
                prepared.trait,
                built.sql,
                built.parameters);
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PartialQueryContext<TRecord> Query<TRecord>(
            this DbConnection connection,
            PreparedPartialQueryContext<TRecord> prepared)
            where TRecord : new()
        {
            var built = prepared.builder();
            Debug.Assert(object.ReferenceEquals(built.parameters, QueryHelper.DefaultParameters));
            return new PartialQueryContext<TRecord>(
                connection,
                null,
                prepared.trait,
                built.sql);
        }

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext<TRecord> Query<TRecord>(
            this DbConnection connection,
            DbTransaction transaction,
            PreparedParameterizedQueryContext<TRecord> prepared)
            where TRecord : new()
        {
            var built = prepared.builder();
            return new ParameterizedQueryContext<TRecord>(
                connection,
                transaction,
                prepared.trait,
                built.sql,
                built.parameters);
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PartialQueryContext<TRecord> Query<TRecord>(
            this DbConnection connection,
            DbTransaction transaction,
            PreparedPartialQueryContext<TRecord> prepared)
            where TRecord : new()
        {
            var built = prepared.builder();
            Debug.Assert(object.ReferenceEquals(built.parameters, QueryHelper.DefaultParameters));
            return new PartialQueryContext<TRecord>(
                connection,
                transaction,
                prepared.trait,
                built.sql);
        }

        /////////////////////////////////////////////////////////////////////////////

        private static async Task<int> InternalExecuteNonQueryAsync(
            QueryContext query, CancellationToken ct)
        {
            using var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters);
            return await command.ExecuteNonQueryAsync(ct).ConfigureAwait(false);
        }

        private static async Task<TRecord> InternalExecuteScalarAsync<TRecord>(
            QueryContext<TRecord> query, CancellationToken ct)
        {
            using var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters);
            return QueryExecutor.ConvertTo<TRecord>(
                query.trait.cc,
                await command.ExecuteScalarAsync(ct).ConfigureAwait(false));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<int> ExecuteNonQueryAsync(
            this ParameterizedQueryContext query, CancellationToken ct = default) =>
            InternalExecuteNonQueryAsync(query, ct);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<TRecord> ExecuteScalarAsync<TRecord>(
            this ParameterizedQueryContext<TRecord> query, CancellationToken ct = default) =>
            InternalExecuteScalarAsync(query, ct);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<int> ExecuteNonQueryNonParameterizedAsync(
            this PartialQueryContext query, CancellationToken ct = default) =>
            InternalExecuteNonQueryAsync(query, ct);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<TRecord> ExecuteScalarNonParameterizedAsync<TRecord>(
            this PartialQueryContext<TRecord> query, CancellationToken ct = default) =>
            InternalExecuteScalarAsync(query, ct);
    }
}
