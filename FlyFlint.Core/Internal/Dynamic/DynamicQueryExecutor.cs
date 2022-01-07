////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

namespace FlyFlint.Internal.Dynamic
{
    internal static class DynamicQueryExecutor
    {
        public static (string name, object? value)[] GetParameters<TParameters>(
            ref TParameters parameters, string parameterPrefix)
        {
            var members = DynamicHelper.GetGetterMetadataList<TParameters>();
            var ps = new (string name, object? value)[members.Length];
            for (var index = 0; index < ps.Length; index++)
            {
                var m = members[index];
                ps[index] = (parameterPrefix + m.name, m.getter(ref parameters));
            }
            return ps;
        }

        /////////////////////////////////////////////////////////////////////

        public static IEnumerable<T> Execute<T>(QueryContext<T> query)
            where T : new()
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var injector = new DynamicInjector<T>(reader, query.fp);
                        do
                        {
                            var element = new T();
                            injector.Inject(reader, ref element);
                            yield return element;
                        }
                        while (reader.Read());
                    }
                }
            }
        }

#if !NET40 && !NET45
        public static async IAsyncEnumerable<T> ExecuteAsync<T>(QueryContext<T> query)
            where T : new()
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    if (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        var injector = new DynamicInjector<T>(reader, query.fp);
                        do
                        {
                            var element = new T();
                            injector.Inject(reader, ref element);
                            yield return element;
                        }
                        while (await reader.ReadAsync().ConfigureAwait(false));
                    }
                }
            }
        }
#endif
    }
}
