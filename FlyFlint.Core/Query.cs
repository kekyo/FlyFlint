////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Internal;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace FlyFlint
{
    public static class Query
    {
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PreparedQueryContext<T> Prepare<T>(string sql)
            where T : new() =>
            new PreparedQueryContext<T>(
                CultureInfo.InvariantCulture,
                Encoding.UTF8,
                sql,
                QueryHelper.Empty,
                "@");
    }
}
