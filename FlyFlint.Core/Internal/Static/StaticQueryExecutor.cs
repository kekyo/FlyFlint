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
            if (parameterPrefix.Length >= 1)
            {
                for (var index = 0; index < extracted.Length; index++)
                {
                    extracted[index] = new KeyValuePair<string, object?>(
                        parameterPrefix + extracted[index].Key, extracted[index].Value);
                }
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

        public static T ExecuteScalar<T>(QueryContext query)
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                return InternalValueConverter<T>.converter.Convert(
                    query.cc, command.ExecuteScalar());
            }
        }

        public static IEnumerable<T> Execute<T>(QueryContext<T> query)
            where T : IDataInjectable, new()
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var context = new DataInjectionContext(
                            query.cc, query.fieldComparer, reader);

                        var element = new T();
                        var metadataList = element.Prepare(context);

                        element.Inject(context, metadataList);
                        yield return element;

                        while (reader.Read())
                        {
                            element = new T();
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

        public static async Task<T> ExecuteScalarAsync<T>(QueryContext query)
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                return InternalValueConverter<T>.converter.Convert(
                    query.cc, await command.ExecuteScalarAsync().ConfigureAwait(false));
            }
        }

#if NET461_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        public static async IAsyncEnumerable<T> ExecuteAsync<T>(QueryContext<T> query)
            where T : IDataInjectable, new()
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    if (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        var context = new DataInjectionContext(
                            query.cc, query.fieldComparer, reader);

                        var element = new T();
                        var metadataList = element.Prepare(context);

                        element.Inject(context, metadataList);
                        yield return element;

                        while (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            element = new T();
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
