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
using System.Threading;

namespace FlyFlint.Internal.Dynamic
{
    internal sealed class DynamicQueryExecutor : QueryExecutor
    {
        public override object? Convert(ConversionContext context, object? value, Type targetType) =>
            DynamicValueConverter.GetConverter(targetType).Convert(context, value);

        public override object? UnsafeConvert(ConversionContext context, object value, Type targetType) =>
            DynamicValueConverter.GetConverter(targetType).UnsafeConvert(context, value);

        /////////////////////////////////////////////////////////////////////

        public override Func<KeyValuePair<string, object?>[]> GetConstructParameters<TParameters>(
            Func<TParameters> getter, string parameterPrefix)
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

        public override KeyValuePair<string, object?>[] GetParameters<TParameters>(
            ref TParameters parameters, string parameterPrefix)
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

        public override IEnumerable<TElement> Execute<TElement>(
            QueryContext<TElement> query)
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var element = new TElement();

                        var context = new DynamicDataInjectionContext<TElement>(
                            query.trait.cc, query.trait.fieldComparer, reader);

                        context.Inject(ref element);
                        yield return element;

                        while (reader.Read())
                        {
                            element = new TElement();
                            context.Inject(ref element);
                            yield return element;
                        }
                    }
                }
            }
        }

        /////////////////////////////////////////////////////////////////////

#if NET461_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        public override async IAsyncEnumerable<TElement> ExecuteAsync<TElement>(
            QueryContext<TElement> query, [EnumeratorCancellation] CancellationToken ct)
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                using (var reader = await command.ExecuteReaderAsync(ct).ConfigureAwait(false))
                {
                    if (await reader.ReadAsync(ct).ConfigureAwait(false))
                    {
                        var element = new TElement();

                        var context = new DynamicDataInjectionContext<TElement>(
                            query.trait.cc, query.trait.fieldComparer, reader);

                        context.Inject(ref element);
                        var prefetchAwaitable = reader.ReadAsync(ct).ConfigureAwait(false);
                        yield return element;

                        while (await prefetchAwaitable)
                        {
                            element = new TElement();
                            context.Inject(ref element);
                            prefetchAwaitable = reader.ReadAsync(ct).ConfigureAwait(false);
                            yield return element;
                        }
                    }
                }
            }
        }
#endif
    }
}
