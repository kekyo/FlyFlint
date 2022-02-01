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
        public static PartialQueryContext<TElement> Query<TElement>(
            this DbConnection connection,
            String sql)
            where TElement : new() =>
            QueryHelper.CurrentDefaultTrait.Query<TElement>(connection, sql);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext<TElement> Query<TElement>(
            this DbConnection connection,
            FormattableString sql)
            where TElement : new() =>
            QueryHelper.CurrentDefaultTrait.Query<TElement>(connection, sql);

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PartialQueryContext<TElement> Query<TElement>(
            this DbConnection connection,
            DbTransaction? transaction,
            String sql)
            where TElement : new() =>
            QueryHelper.CurrentDefaultTrait.Query<TElement>(connection, transaction, sql);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext<TElement> Query<TElement>(
            this DbConnection connection,
            DbTransaction? transaction,
            FormattableString sql)
            where TElement : new() =>
            QueryHelper.CurrentDefaultTrait.Query<TElement>(connection, transaction, sql);

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
        public static ParameterizedQueryContext<TElement> Query<TElement>(
            this DbConnection connection,
            PreparedParameterizedQueryContext<TElement> prepared)
            where TElement : new()
        {
            var built = prepared.builder();
            return new ParameterizedQueryContext<TElement>(
                connection,
                null,
                prepared.trait,
                built.sql,
                built.parameters);
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PartialQueryContext<TElement> Query<TElement>(
            this DbConnection connection,
            PreparedPartialQueryContext<TElement> prepared)
            where TElement : new()
        {
            var built = prepared.builder();
            Debug.Assert(object.ReferenceEquals(built.parameters, QueryHelper.DefaultParameters));
            return new PartialQueryContext<TElement>(
                connection,
                null,
                prepared.trait,
                built.sql);
        }

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext<TElement> Query<TElement>(
            this DbConnection connection,
            DbTransaction transaction,
            PreparedParameterizedQueryContext<TElement> prepared)
            where TElement : new()
        {
            var built = prepared.builder();
            return new ParameterizedQueryContext<TElement>(
                connection,
                transaction,
                prepared.trait,
                built.sql,
                built.parameters);
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PartialQueryContext<TElement> Query<TElement>(
            this DbConnection connection,
            DbTransaction transaction,
            PreparedPartialQueryContext<TElement> prepared)
            where TElement : new()
        {
            var built = prepared.builder();
            Debug.Assert(object.ReferenceEquals(built.parameters, QueryHelper.DefaultParameters));
            return new PartialQueryContext<TElement>(
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

        private static async Task<TElement> InternalExecuteScalarAsync<TElement>(
            QueryContext<TElement> query, CancellationToken ct)
        {
            using var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters);
            return QueryExecutor.ConvertTo<TElement>(
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
        public static Task<TElement> ExecuteScalarAsync<TElement>(
            this ParameterizedQueryContext<TElement> query, CancellationToken ct = default) =>
            InternalExecuteScalarAsync(query, ct);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<int> ExecuteNonParameterizedQueryAsync(
            this PartialQueryContext query, CancellationToken ct = default) =>
            InternalExecuteNonQueryAsync(query, ct);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<TElement> ExecuteNonParameterizedScalarAsync<TElement>(
            this PartialQueryContext<TElement> query, CancellationToken ct = default) =>
            InternalExecuteScalarAsync(query, ct);
    }
}
