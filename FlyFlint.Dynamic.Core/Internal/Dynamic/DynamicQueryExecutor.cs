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
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace FlyFlint.Internal.Dynamic
{
    internal sealed class DynamicQueryExecutor : IDynamicQueryExecutor
    {
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public object? Convert(ConversionContext context, object? value, Type targetType) =>
            DynamicValueConverter.GetConverter(targetType).Convert(context, value);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public object? UnsafeConvert(ConversionContext context, object value, Type targetType) =>
            DynamicValueConverter.GetConverter(targetType).UnsafeConvert(context, value);

        /////////////////////////////////////////////////////////////////////

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public (string name, object? value)[] GetParameters<TParameters>(
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

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public int ExecuteNonQuery(QueryContext query)
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                return command.ExecuteNonQuery();
            }
        }

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public T ExecuteScalar<T>(QueryContext query)
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                return InternalValueConverter<T>.converter.Convert(
                    query.cc, command.ExecuteScalar());
            }
        }

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public IEnumerable<T> Execute<T>(QueryContext<T> query)
            where T : new()
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var context = new DataInjectionContext(query.cc, reader);

                        var injector = new DynamicInjector<T>(context);
                        do
                        {
                            var element = new T();
                            injector.Inject(ref element);
                            yield return element;
                        }
                        while (reader.Read());
                    }
                }
            }
        }

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public async Task<int> ExecuteNonQueryAsync(QueryContext query)
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                return await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public async Task<T> ExecuteScalarAsync<T>(QueryContext query)
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                return InternalValueConverter<T>.converter.Convert(
                    query.cc, await command.ExecuteScalarAsync().ConfigureAwait(false));
            }
        }

#if !NET40 && !NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async IAsyncEnumerable<T> ExecuteAsync<T>(QueryContext<T> query)
            where T : new()
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    if (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        var context = new DataInjectionContext(query.cc, reader);

                        var injector = new DynamicInjector<T>(context);
                        do
                        {
                            var element = new T();
                            injector.Inject(ref element);
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
