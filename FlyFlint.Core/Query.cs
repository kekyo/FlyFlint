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
        internal static readonly Func<KeyValuePair<string, object?>[]> constructDefaultParameters =
            new(() => defaultParameters!);
        internal static readonly KeyValuePair<string, object?>[] defaultParameters = { };
        internal static readonly string defaultParameterPrefix = "@";
        
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PreparedQueryContext Prepare(QueryString sql) =>
            new PreparedQueryContext(
                ConversionContext.Default,
                sql.Sql,
                constructDefaultParameters,
                defaultParameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PreparedQueryContext<T> Prepare<T>(QueryString sql)
            where T : new() =>
            new PreparedQueryContext<T>(
                ConversionContext.Default,
                defaultFieldComparer,
                sql.Sql,
                constructDefaultParameters,
                defaultParameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PreparedQueryContext Prepare(FormattableString sql) =>
            new PreparedQueryContext(
                ConversionContext.Default,
                QueryHelper.GetFormattedSqlString(sql, defaultParameterPrefix),
                () => QueryHelper.GetSqlParameters(sql, defaultParameterPrefix),   // TODO: too late
                defaultParameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PreparedQueryContext<T> Prepare<T>(FormattableString sql)
            where T : new() =>
            new PreparedQueryContext<T>(
                ConversionContext.Default,
                defaultFieldComparer,
                QueryHelper.GetFormattedSqlString(sql, defaultParameterPrefix),
                () => QueryHelper.GetSqlParameters(sql, defaultParameterPrefix),   // TODO: too late
                defaultParameterPrefix);
    }
}
