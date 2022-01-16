////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Context;
using System.Runtime.CompilerServices;

namespace FlyFlint
{
    public static class Query
    {
        internal static readonly (string, object?)[] defaultParameters = { };
        internal static readonly string defaultParameterPrefix = "@";
        
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PreparedQueryContext Prepare(string sql) =>
            new PreparedQueryContext(
                ConversionContext.Default,
                sql,
                defaultParameters,
                defaultParameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PreparedQueryContext<T> Prepare<T>(string sql)
            where T : new() =>
            new PreparedQueryContext<T>(
                ConversionContext.Default,
                sql,
                defaultParameters,
                defaultParameterPrefix);
    }
}
