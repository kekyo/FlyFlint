////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace FlyFlint
{
    public static class Query
    {
        internal static readonly IFormatProvider defaultFp = CultureInfo.InvariantCulture;
        internal static readonly Encoding defaultEncoding = Encoding.UTF8;
        internal static readonly (string, object?)[] defaultParameters = { };
        internal static readonly string defaultParameterPrefix = "@";
        
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PreparedQueryContext Prepare(string sql) =>
            new PreparedQueryContext(defaultFp, defaultEncoding, sql, defaultParameters, defaultParameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PreparedQueryContext<T> Prepare<T>(string sql)
            where T : new() =>
            new PreparedQueryContext<T>(defaultFp, defaultEncoding, sql, defaultParameters, defaultParameterPrefix);
    }
}
