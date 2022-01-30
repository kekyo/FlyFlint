////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Context;
using FlyFlint.Internal.Static;
using System;
using System.Runtime.CompilerServices;

namespace FlyFlint
{
    public static class Query
    {
        public static Trait DefaultTrait =
            Database.Default;

        public static bool IsParameterExtractable<TParameters>(TParameters parameters)
            where TParameters : notnull, new() =>
            parameters is IParameterExtractable;

        public static bool IsDataInjectable<TElement>(TElement parameters)
            where TElement : new() =>
            parameters is IDataInjectable;

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
        public static PreparedParameterizedQueryContext<TElement> Prepare<TElement>(Func<FormattableString> sqlBuilder)
            where TElement : new() =>
            DefaultTrait.Prepare<TElement>(sqlBuilder);
    }
}
