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
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;

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
            FlyFlint.Query.DefaultTrait.Query(connection, sql);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext Query(
            this DbConnection connection,
            FormattableString sql) =>
            FlyFlint.Query.DefaultTrait.Query(connection, sql);

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PartialQueryContext Query(
            this DbConnection connection,
            DbTransaction? transaction,
            String sql) =>
            FlyFlint.Query.DefaultTrait.Query(connection, transaction, sql);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext Query(
            this DbConnection connection,
            DbTransaction? transaction,
            FormattableString sql) =>
            FlyFlint.Query.DefaultTrait.Query(connection, transaction, sql);

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PartialQueryContext<TElement> Query<TElement>(
            this DbConnection connection,
            String sql)
            where TElement : new() =>
            FlyFlint.Query.DefaultTrait.Query<TElement>(connection, sql);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext<TElement> Query<TElement>(
            this DbConnection connection,
            FormattableString sql)
            where TElement : new() =>
            FlyFlint.Query.DefaultTrait.Query<TElement>(connection, sql);

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PartialQueryContext<TElement> Query<TElement>(
            this DbConnection connection,
            DbTransaction? transaction,
            String sql)
            where TElement : new() =>
            FlyFlint.Query.DefaultTrait.Query<TElement>(connection, transaction, sql);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext<TElement> Query<TElement>(
            this DbConnection connection,
            DbTransaction? transaction,
            FormattableString sql)
            where TElement : new() =>
            FlyFlint.Query.DefaultTrait.Query<TElement>(connection, transaction, sql);

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
            Debug.Assert(object.ReferenceEquals(built.parameters, Database.defaultParameters));
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
            Debug.Assert(object.ReferenceEquals(built.parameters, Database.defaultParameters));
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
            Debug.Assert(object.ReferenceEquals(built.parameters, Database.defaultParameters));
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
            Debug.Assert(object.ReferenceEquals(built.parameters, Database.defaultParameters));
            return new PartialQueryContext<TElement>(
                connection,
                transaction,
                prepared.trait,
                built.sql);
        }
    }
}
