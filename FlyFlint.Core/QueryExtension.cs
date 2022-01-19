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
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace FlyFlint
{
    public static class QueryExtension
    {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext Query(
            this DbConnection connection,
            QueryString sql) =>
            new QueryContext(
                connection,
                null,
                ConversionContext.Default,
                sql.Sql,
                FlyFlint.Query.defaultParameters,
                FlyFlint.Query.defaultParameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext Query(
            this DbConnection connection,
            FormattableString sql) =>
            new QueryContext(
                connection,
                null,
                ConversionContext.Default,
                QueryHelper.GetFormattedSqlString(sql, FlyFlint.Query.defaultParameterPrefix),
                QueryHelper.GetSqlParameters(sql, FlyFlint.Query.defaultParameterPrefix),
                FlyFlint.Query.defaultParameterPrefix);

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext Query(
            this DbConnection connection,
            DbTransaction transaction,
            QueryString sql) =>
            new QueryContext(
                connection,
                transaction,
                ConversionContext.Default,
                sql.Sql,
                FlyFlint.Query.defaultParameters,
                FlyFlint.Query.defaultParameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext Query(
            this DbConnection connection,
            DbTransaction transaction,
            FormattableString sql) =>
            new QueryContext(
                connection,
                transaction,
                ConversionContext.Default,
                QueryHelper.GetFormattedSqlString(sql, FlyFlint.Query.defaultParameterPrefix),
                QueryHelper.GetSqlParameters(sql, FlyFlint.Query.defaultParameterPrefix),
                FlyFlint.Query.defaultParameterPrefix);

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext<T> Query<T>(
            this DbConnection connection,
            QueryString sql)
            where T : new() =>
            new QueryContext<T>(
                connection,
                null,
                ConversionContext.Default,
                FlyFlint.Query.defaultFieldComparer,
                sql.Sql,
                FlyFlint.Query.defaultParameters,
                FlyFlint.Query.defaultParameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext<T> Query<T>(
            this DbConnection connection,
            FormattableString sql)
            where T : new() =>
            new QueryContext<T>(
                connection,
                null,
                ConversionContext.Default,
                FlyFlint.Query.defaultFieldComparer,
                QueryHelper.GetFormattedSqlString(sql, FlyFlint.Query.defaultParameterPrefix),
                QueryHelper.GetSqlParameters(sql, FlyFlint.Query.defaultParameterPrefix),
                FlyFlint.Query.defaultParameterPrefix);

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext<T> Query<T>(
            this DbConnection connection,
            DbTransaction transaction,
            QueryString sql)
            where T : new() =>
            new QueryContext<T>(
                connection,
                transaction,
                ConversionContext.Default,
                FlyFlint.Query.defaultFieldComparer,
                sql.Sql,
                FlyFlint.Query.defaultParameters,
                FlyFlint.Query.defaultParameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext<T> Query<T>(
            this DbConnection connection,
            DbTransaction transaction,
            FormattableString sql)
            where T : new() =>
            new QueryContext<T>(
                connection,
                transaction,
                ConversionContext.Default,
                FlyFlint.Query.defaultFieldComparer,
                QueryHelper.GetFormattedSqlString(sql, FlyFlint.Query.defaultParameterPrefix),
                QueryHelper.GetSqlParameters(sql, FlyFlint.Query.defaultParameterPrefix),
                FlyFlint.Query.defaultParameterPrefix);

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext Query(
            this DbConnection connection,
            PreparedQueryContext prepared) =>
            new QueryContext(
                connection,
                null,
                prepared.cc,
                prepared.sql,
                prepared.constructParameters(),
                prepared.parameterPrefix);

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext Query(
            this DbConnection connection,
            DbTransaction transaction,
            PreparedQueryContext prepared) =>
            new QueryContext(
                connection,
                transaction,
                prepared.cc,
                prepared.sql,
                prepared.constructParameters(),
                prepared.parameterPrefix);

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext<T> Query<T>(
            this DbConnection connection,
            PreparedQueryContext<T> prepared)
            where T : new() =>
            new QueryContext<T>(
                connection,
                null,
                prepared.cc,
                prepared.fieldComparer,
                prepared.sql,
                prepared.constructParameters(),
                prepared.parameterPrefix);

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext<T> Query<T>(
            this DbConnection connection,
            DbTransaction transaction,
            PreparedQueryContext<T> prepared)
            where T : new() =>
            new QueryContext<T>(
                connection,
                transaction,
                prepared.cc,
                prepared.fieldComparer,
                prepared.sql,
                prepared.constructParameters(),
                prepared.parameterPrefix);
    }
}
