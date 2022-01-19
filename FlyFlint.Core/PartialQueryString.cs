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
    public struct PartialQueryString
    {
        public readonly string Sql;

#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PartialQueryString(string sql) =>
            this.Sql = sql;

#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static implicit operator PartialQueryString(string sql) =>
            new PartialQueryString(sql);

        // HACK: Fixing overload resolution between string type and FormattableString type.
        // https://ufcpp.net/study/csharp/st_string.html  (in japanese)
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static implicit operator PartialQueryString(FormattableString str) =>
            throw new InvalidCastException();
    }
}
