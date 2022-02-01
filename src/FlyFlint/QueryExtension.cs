﻿////////////////////////////////////////////////////////////////////////////
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

        public static async Task<int> ExecuteNonQueryAsync(
            this QueryContext query, CancellationToken ct = default)
        {
            using var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters);
            return await command.ExecuteNonQueryAsync(ct).ConfigureAwait(false);
        }

        public static async Task<TElement> ExecuteScalarAsync<TElement>(
            this QueryContext<TElement> query, CancellationToken ct = default)
        {
            using var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters);
            return QueryExecutor.ConvertTo<TElement>(
                query.trait.cc,
                await command.ExecuteScalarAsync(ct).ConfigureAwait(false));
        }
    }
}