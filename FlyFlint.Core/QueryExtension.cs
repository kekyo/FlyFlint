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
        public static ParameterizableQueryContext Query(
            this DbConnection connection,
            ParameterizableQueryString sql) =>
            new ParameterizableQueryContext(
                connection,
                null,
                ConversionContext.Default,
                sql.Sql,
                FlyFlint.Query.defaultParameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext Query(
            this DbConnection connection,
            FormattableString sql) =>
            new ParameterizedQueryContext(
                connection,
                null,
                ConversionContext.Default,
                QueryHelper.GetFormattedSqlString(sql, FlyFlint.Query.defaultParameterPrefix),
                QueryHelper.GetSqlParameters(sql, FlyFlint.Query.defaultParameterPrefix));

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizableQueryContext Query(
            this DbConnection connection,
            DbTransaction transaction,
            ParameterizableQueryString sql) =>
            new ParameterizableQueryContext(
                connection,
                transaction,
                ConversionContext.Default,
                sql.Sql,
                FlyFlint.Query.defaultParameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext Query(
            this DbConnection connection,
            DbTransaction transaction,
            FormattableString sql) =>
            new ParameterizedQueryContext(
                connection,
                transaction,
                ConversionContext.Default,
                QueryHelper.GetFormattedSqlString(sql, FlyFlint.Query.defaultParameterPrefix),
                QueryHelper.GetSqlParameters(sql, FlyFlint.Query.defaultParameterPrefix));

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizableQueryContext<T> Query<T>(
            this DbConnection connection,
            ParameterizableQueryString sql)
            where T : new() =>
            new ParameterizableQueryContext<T>(
                connection,
                null,
                ConversionContext.Default,
                FlyFlint.Query.defaultFieldComparer,
                sql.Sql,
                FlyFlint.Query.defaultParameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext<T> Query<T>(
            this DbConnection connection,
            FormattableString sql)
            where T : new() =>
            new ParameterizedQueryContext<T>(
                connection,
                null,
                ConversionContext.Default,
                FlyFlint.Query.defaultFieldComparer,
                QueryHelper.GetFormattedSqlString(sql, FlyFlint.Query.defaultParameterPrefix),
                QueryHelper.GetSqlParameters(sql, FlyFlint.Query.defaultParameterPrefix));

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizableQueryContext<T> Query<T>(
            this DbConnection connection,
            DbTransaction transaction,
            ParameterizableQueryString sql)
            where T : new() =>
            new ParameterizableQueryContext<T>(
                connection,
                transaction,
                ConversionContext.Default,
                FlyFlint.Query.defaultFieldComparer,
                sql.Sql,
                FlyFlint.Query.defaultParameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext<T> Query<T>(
            this DbConnection connection,
            DbTransaction transaction,
            FormattableString sql)
            where T : new() =>
            new ParameterizedQueryContext<T>(
                connection,
                transaction,
                ConversionContext.Default,
                FlyFlint.Query.defaultFieldComparer,
                QueryHelper.GetFormattedSqlString(sql, FlyFlint.Query.defaultParameterPrefix),
                QueryHelper.GetSqlParameters(sql, FlyFlint.Query.defaultParameterPrefix));

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext Query(
            this DbConnection connection,
            PreparedParameterizedQueryContext prepared) =>
            new ParameterizedQueryContext(
                connection,
                null,
                prepared.cc,
                prepared.sql,
                prepared.constructParameters());

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizableQueryContext Query(
            this DbConnection connection,
            PreparedParameterizableQueryContext prepared) =>
            new ParameterizableQueryContext(
                connection,
                null,
                prepared.cc,
                prepared.sql,
                prepared.parameterPrefix);

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext Query(
            this DbConnection connection,
            DbTransaction transaction,
            PreparedParameterizedQueryContext prepared) =>
            new ParameterizedQueryContext(
                connection,
                transaction,
                prepared.cc,
                prepared.sql,
                prepared.constructParameters());

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizableQueryContext Query(
            this DbConnection connection,
            DbTransaction transaction,
            PreparedParameterizableQueryContext prepared) =>
            new ParameterizableQueryContext(
                connection,
                transaction,
                prepared.cc,
                prepared.sql,
                prepared.parameterPrefix);

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext<T> Query<T>(
            this DbConnection connection,
            PreparedParameterizedQueryContext<T> prepared)
            where T : new() =>
            new ParameterizedQueryContext<T>(
                connection,
                null,
                prepared.cc,
                prepared.fieldComparer,
                prepared.sql,
                prepared.constructParameters());

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizableQueryContext<T> Query<T>(
            this DbConnection connection,
            PreparedParameterizableQueryContext<T> prepared)
            where T : new() =>
            new ParameterizableQueryContext<T>(
                connection,
                null,
                prepared.cc,
                prepared.fieldComparer,
                prepared.sql,
                prepared.parameterPrefix);

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext<T> Query<T>(
            this DbConnection connection,
            DbTransaction transaction,
            PreparedParameterizedQueryContext<T> prepared)
            where T : new() =>
            new ParameterizedQueryContext<T>(
                connection,
                transaction,
                prepared.cc,
                prepared.fieldComparer,
                prepared.sql,
                prepared.constructParameters());

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizableQueryContext<T> Query<T>(
            this DbConnection connection,
            DbTransaction transaction,
            PreparedParameterizableQueryContext<T> prepared)
            where T : new() =>
            new ParameterizableQueryContext<T>(
                connection,
                transaction,
                prepared.cc,
                prepared.fieldComparer,
                prepared.sql,
                prepared.parameterPrefix);
    }
}
