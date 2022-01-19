////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.CompilerServices;

namespace FlyFlint
{
    public struct QueryString
    {
        public readonly string Sql;

#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public QueryString(string sql) =>
            this.Sql = sql;

#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static implicit operator QueryString(string sql) =>
            new QueryString(sql);

        // HACK: Fixing overload resolution between string type and FormattableString type.
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static implicit operator QueryString(FormattableString str) =>
            throw new InvalidCastException();
    }
}
