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
using System.Collections.Generic;

namespace FlyFlint.Synchronized
{
    public static class QueryFacadeExtension
    {
        private static IEnumerable<TRecord> InternalExecute<TRecord>(
            QueryContext<TRecord> query)
            where TRecord : notnull, new()
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var element = new TRecord();

                        var injector = QueryExecutor.GetDataInjector(
                            query.trait.cc, query.trait.fieldComparer, reader, ref element);

                        injector(ref element);
                        yield return element;

                        while (reader.Read())
                        {
                            element = new TRecord();
                            injector(ref element);
                            yield return element;
                        }
                    }
                }
            }
        }

        public static IEnumerable<TRecord> Execute<TRecord>(
            this ParameterizedQueryContext<TRecord> query)
            where TRecord : notnull, new() =>
            InternalExecute(query);

        public static IEnumerable<TRecord> ExecuteNonParameterized<TRecord>(
            this PartialQueryContext<TRecord> query)
            where TRecord : notnull, new() =>
            InternalExecute(query);
    }
}
