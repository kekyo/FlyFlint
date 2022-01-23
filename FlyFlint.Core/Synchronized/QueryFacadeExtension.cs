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
using FlyFlint.Internal.Converter;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FlyFlint.Synchronized
{
    public static class QueryFacadeExtension
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
            return InternalValueConverter<TElement>.converter.Convert(
                query.trait.cc, command.ExecuteScalar());
        }

        public static IEnumerable<TElement> Execute<TElement>(
            this QueryContext<TElement> query)
            where TElement : notnull, new()
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var element = new TElement();

                        var injector = QueryExecutor.Instance.GetInjector(
                            query.trait.cc, query.trait.fieldComparer, reader, ref element);

                        injector(ref element);
                        yield return element;

                        while (reader.Read())
                        {
                            element = new TElement();
                            injector(ref element);
                            yield return element;
                        }
                    }
                }
            }
        }
    }
}
