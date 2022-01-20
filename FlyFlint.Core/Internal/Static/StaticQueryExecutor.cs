////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Context;
using FlyFlint.Internal.Converter;
#if NET35 || NET40
using FlyFlint.Utilities;
#endif
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlyFlint.Internal.Static
{
    internal static class StaticQueryExecutor
    {
        public static Func<KeyValuePair<string, object?>[]> GetConstructParameters<TParameters>(
            Func<TParameters> getter, string parameterPrefix)
            where TParameters : notnull, IParameterExtractable
        {
            return () =>
            {
                var parameters = getter();
                return GetParameters(ref parameters, parameterPrefix);
            };
        }

        public static KeyValuePair<string, object?>[] GetParameters<TParameters>(
            ref TParameters parameters, string parameterPrefix)
            where TParameters : IParameterExtractable
        {
            var extracted = parameters.Extract();
            for (var index = 0; index < extracted.Length; index++)
            {
                extracted[index] = new KeyValuePair<string, object?>(
                    parameterPrefix + extracted[index].Key, extracted[index].Value);
            }
            return extracted;
        }

        /////////////////////////////////////////////////////////////////////

        public static int ExecuteNonQuery(QueryContext query)
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                return command.ExecuteNonQuery();
            }
        }

        public static TElement ExecuteScalar<TElement>(QueryContext<TElement> query)
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                return InternalValueConverter<TElement>.converter.Convert(
                    query.trait.cc, command.ExecuteScalar());
            }
        }

        public static IEnumerable<TElement> Execute<TElement>(QueryContext<TElement> query)
            where TElement : IDataInjectable, new()
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var context = new DataInjectionContext(
                            query.trait.cc, query.trait.fieldComparer, reader);

                        var element = new TElement();
                        var metadataList = element.Prepare(context);

                        element.Inject(context, metadataList);
                        yield return element;

                        while (reader.Read())
                        {
                            element = new TElement();
                            element.Inject(context, metadataList);
                            yield return element;
                        }
                    }
                }
            }
        }

        /////////////////////////////////////////////////////////////////////

        public static async Task<int> ExecuteNonQueryAsync(QueryContext query)
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                return await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        public static async Task<TElement> ExecuteScalarAsync<TElement>(QueryContext<TElement> query)
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                return InternalValueConverter<TElement>.converter.Convert(
                    query.trait.cc, await command.ExecuteScalarAsync().ConfigureAwait(false));
            }
        }

#if NET461_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        public static async IAsyncEnumerable<TElement> ExecuteAsync<TElement>(QueryContext<TElement> query)
            where TElement : IDataInjectable, new()
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    if (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        var context = new DataInjectionContext(
                            query.trait.cc, query.trait.fieldComparer, reader);

                        var element = new TElement();
                        var metadataList = element.Prepare(context);

                        element.Inject(context, metadataList);
                        yield return element;

                        while (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            element = new TElement();
                            element.Inject(context, metadataList);
                            yield return element;
                        }
                    }
                }
            }
        }
#endif
    }
}
