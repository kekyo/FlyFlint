﻿////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Internal.Converter;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlyFlint.Internal.Static
{
    internal static class StaticQueryExecutor
    {
        public static (string name, object? value)[] GetParameters<TParameters>(
            ref TParameters parameters, string parameterPrefix)
            where TParameters : IParameterExtractable
        {
            var extracted = parameters.Extract();
            var ps = new (string name, object? value)[extracted.Length];
            for (var index = 0; index < ps.Length; index++)
            {
                var parameter = extracted[index];
                ps[index] = (parameterPrefix + parameter.name, parameter.value);
            }
            return ps;
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
                return ValueConverter<T>.Convert(
                    query.fp,
                    query.encoding,
                    command.ExecuteScalar());
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
                        var element = new T();
                        var metadataList = element.PrepareAndInject(query.fp, reader);
                        yield return element;

                        while (reader.Read())
                        {
                            element = new T();
                            element.Inject(query.fp, metadataList, reader);
                            yield return element;
                        }
                    }
                }
            }
        }

        /////////////////////////////////////////////////////////////////////

#if !NET40
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
                return ValueConverter<T>.Convert(
                    query.fp,
                    query.encoding,
                    await command.ExecuteScalarAsync().ConfigureAwait(false));
            }
        }

#if !NET45
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
                        var element = new T();
                        var metadataList = element.PrepareAndInject(query.fp, reader);
                        yield return element;

                        while (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            element = new T();
                            element.Inject(query.fp, metadataList, reader);
                            yield return element;
                        }
                    }
                }
            }
        }
#endif
#endif
    }
}