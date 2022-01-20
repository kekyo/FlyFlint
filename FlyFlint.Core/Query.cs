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
using System.Runtime.CompilerServices;

namespace FlyFlint
{
    public static class Query
    {
        public static Database DefaultTrait =
            Database.Default;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PreparedPartialQueryContext Prepare(String sql) =>
            DefaultTrait.Prepare(sql);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PreparedPartialQueryContext<TElement> Prepare<TElement>(String sql)
            where TElement : new() =>
            DefaultTrait.Prepare<TElement>(sql);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PreparedParameterizedQueryContext Prepare(FormattableString sql) =>
            DefaultTrait.Prepare(sql);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PreparedParameterizedQueryContext<TElement> Prepare<TElement>(FormattableString sql)
            where TElement : new() =>
            DefaultTrait.Prepare<TElement>(sql);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PreparedParameterizedQueryContext Prepare(Func<FormattableString> sqlBuilder) =>
            DefaultTrait.Prepare(sqlBuilder);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PreparedParameterizedQueryContext<T> Prepare<T>(Func<FormattableString> sqlBuilder)
            where T : new() =>
            DefaultTrait.Prepare<T>(sqlBuilder);
    }
}
