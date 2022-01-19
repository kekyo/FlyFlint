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
using System.Runtime.CompilerServices;

namespace FlyFlint
{
    public static class Query
    {
        internal static readonly IComparer<string> defaultFieldComparer =
            StringComparer.OrdinalIgnoreCase;
        internal static readonly KeyValuePair<string, object?>[] defaultParameters = { };
        internal static readonly string defaultParameterPrefix = "@";
        
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PreparedParameterizableQueryContext Prepare(ParameterizableQueryString sql) =>
            new PreparedParameterizableQueryContext(
                ConversionContext.Default,
                () => new QueryParameterBuilderResult(sql.Sql, defaultParameters),
                defaultParameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PreparedParameterizableQueryContext<T> Prepare<T>(ParameterizableQueryString sql)
            where T : new() =>
            new PreparedParameterizableQueryContext<T>(
                ConversionContext.Default,
                defaultFieldComparer,
                () => new QueryParameterBuilderResult(sql.Sql, defaultParameters),
                defaultParameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PreparedParameterizedQueryContext Prepare(FormattableString sql) =>
            new PreparedParameterizedQueryContext(
                ConversionContext.Default,
                () => new QueryParameterBuilderResult(
                    QueryHelper.GetFormattedSqlString(sql, defaultParameterPrefix),
                    QueryHelper.GetSqlParameters(sql, defaultParameterPrefix)));

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PreparedParameterizedQueryContext<T> Prepare<T>(FormattableString sql)
            where T : new() =>
            new PreparedParameterizedQueryContext<T>(
                ConversionContext.Default,
                defaultFieldComparer,
                () => new QueryParameterBuilderResult(
                    QueryHelper.GetFormattedSqlString(sql, defaultParameterPrefix),
                    QueryHelper.GetSqlParameters(sql, defaultParameterPrefix)));

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PreparedParameterizedQueryContext Prepare(Func<FormattableString> sqlBuilder) =>
            new PreparedParameterizedQueryContext(
                ConversionContext.Default,
                () =>
                {
                    var sql = sqlBuilder();
                    return new QueryParameterBuilderResult(
                        QueryHelper.GetFormattedSqlString(sql, defaultParameterPrefix),
                        QueryHelper.GetSqlParameters(sql, defaultParameterPrefix));
                });

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PreparedParameterizedQueryContext<T> Prepare<T>(Func<FormattableString> sqlBuilder)
            where T : new() =>
            new PreparedParameterizedQueryContext<T>(
                ConversionContext.Default,
                defaultFieldComparer,
                () =>
                {
                    var sql = sqlBuilder();
                    return new QueryParameterBuilderResult(
                        QueryHelper.GetFormattedSqlString(sql, defaultParameterPrefix),
                        QueryHelper.GetSqlParameters(sql, defaultParameterPrefix));
                });
    }
}
