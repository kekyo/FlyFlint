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
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace FlyFlint.Internal.Dynamic
{
    internal sealed class DynamicQueryExecutor : IDynamicQueryExecutor
    {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public object? Convert(ConversionContext context, object? value, Type targetType) =>
            DynamicValueConverter.GetConverter(targetType).Convert(context, value);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public object? UnsafeConvert(ConversionContext context, object value, Type targetType) =>
            DynamicValueConverter.GetConverter(targetType).UnsafeConvert(context, value);

        /////////////////////////////////////////////////////////////////////

        public Func<KeyValuePair<string, object?>[]> GetConstructParameters<TParameters>(
            Func<TParameters> getter, string parameterPrefix)
            where TParameters : notnull
        {
            var members = DynamicHelper.GetGetterMetadataList<TParameters>();
            return () =>
            {
                var parameters = getter();
                var ps = new KeyValuePair<string, object?>[members.Length];
                for (var index = 0; index < ps.Length; index++)
                {
                    var m = members[index];
                    ps[index] = new KeyValuePair<string, object?>(
                        parameterPrefix + m.FieldName, m.Accessor(ref parameters));
                }
                return ps;
            };
        }

        public KeyValuePair<string, object?>[] GetParameters<TParameters>(
            ref TParameters parameters, string parameterPrefix)
            where TParameters : notnull
        {
            var members = DynamicHelper.GetGetterMetadataList<TParameters>();
            var ps = new KeyValuePair<string, object?>[members.Length];
            for (var index = 0; index < ps.Length; index++)
            {
                var m = members[index];
                ps[index] = new KeyValuePair<string, object?>(
                    parameterPrefix + m.FieldName, m.Accessor(ref parameters));
            }
            return ps;
        }

        /////////////////////////////////////////////////////////////////////

        public int ExecuteNonQuery(QueryContext query)
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                return command.ExecuteNonQuery();
            }
        }

        public TElement ExecuteScalar<TElement>(QueryContext<TElement> query)
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                return InternalValueConverter<TElement>.converter.Convert(
                    query.trait.cc, command.ExecuteScalar());
            }
        }

        public IEnumerable<TElement> Execute<TElement>(QueryContext<TElement> query)
            where TElement : new()
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var context = new DynamicDataInjectionContext(
                            query.trait.cc, query.trait.fieldComparer, reader);

                        var injector = new DynamicInjector<TElement>(context);
                        do
                        {
                            var element = new TElement();
                            injector.Inject(ref element);
                            yield return element;
                        }
                        while (reader.Read());
                    }
                }
            }
        }

        /////////////////////////////////////////////////////////////////////

        public async Task<int> ExecuteNonQueryAsync(QueryContext query)
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                return await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        public async Task<TElement> ExecuteScalarAsync<TElement>(QueryContext<TElement> query)
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                return InternalValueConverter<TElement>.converter.Convert(
                    query.trait.cc, await command.ExecuteScalarAsync().ConfigureAwait(false));
            }
        }

#if NET461_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async IAsyncEnumerable<TElement> ExecuteAsync<TElement>(QueryContext<TElement> query)
            where TElement : new()
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    if (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        var context = new DynamicDataInjectionContext(
                            query.trait.cc, query.trait.fieldComparer, reader);

                        var injector = new DynamicInjector<TElement>(context);
                        do
                        {
                            var element = new TElement();
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
