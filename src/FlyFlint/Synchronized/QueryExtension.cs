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

namespace FlyFlint.Synchronized
{
    public static class QueryExtension
    {
        public static int ExecuteNonQuery(
            this QueryContext query)
        {
            using var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters);
            return command.ExecuteNonQuery();
        }

        public static TElement ExecuteScalar<TElement>(
            this QueryContext<TElement> query)
        {
            using var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters);
            return QueryExecutor.ConvertTo<TElement>(
                query.trait.cc, command.ExecuteScalar());
        }
    }
}
