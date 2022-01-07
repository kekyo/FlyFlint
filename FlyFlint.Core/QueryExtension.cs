////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Internal;
using System.Data.Common;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace FlyFlint
{
    public static class QueryExtension
    {
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext<T> Query<T>(
            this DbConnection connection,
            PreparedQueryContext<T> prepared)
            where T : new() =>
            new QueryContext<T>(
                connection,
                null,
                prepared.fp,
                prepared.encoding,
                prepared.sql,
                prepared.parameters,
                prepared.parameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext<T> Query<T>(
            this DbConnection connection, DbTransaction transaction,
            PreparedQueryContext<T> prepared)
            where T : new() =>
            new QueryContext<T>(
                connection,
                transaction,
                prepared.fp,
                prepared.encoding,
                prepared.sql,
                prepared.parameters,
                prepared.parameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext<T> Query<T>(
            this DbConnection connection, string sql)
            where T : new() =>
            new QueryContext<T>(
                connection,
                null,
                CultureInfo.InvariantCulture,
                Encoding.UTF8,
                sql,
                QueryHelper.Empty,
                "@");

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext<T> Query<T>(
            this DbConnection connection,
            DbTransaction transaction,
            string sql)
            where T : new() =>
            new QueryContext<T>(
                connection,
                transaction,
                CultureInfo.InvariantCulture,
                Encoding.UTF8,
                sql,
                QueryHelper.Empty,
                "@");
    }
}
