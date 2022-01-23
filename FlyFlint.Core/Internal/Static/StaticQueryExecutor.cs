////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Context;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace FlyFlint.Internal.Static
{
    internal sealed class StaticQueryExecutor : QueryExecutor
    {
        public new static readonly QueryExecutor Instance = new StaticQueryExecutor();

        private StaticQueryExecutor()
        {
        }

        public override object? Convert(ConversionContext context, object? value, Type targetType) =>
            throw new NotImplementedException();

        public override object? UnsafeConvert(ConversionContext context, object value, Type targetType) =>
            throw new NotImplementedException();

        public override Func<KeyValuePair<string, object?>[]> GetConstructParameters<TParameters>(
            Func<TParameters> getter, string parameterPrefix)
        {
            return () =>
            {
                var parameters = (IParameterExtractable)getter();
                return GetParameters(ref parameters, parameterPrefix);
            };
        }

        public override KeyValuePair<string, object?>[] GetParameters<TParameters>(
            ref TParameters parameters, string parameterPrefix)
        {
            var extracted = ((IParameterExtractable)parameters).Extract();
            for (var index = 0; index < extracted.Length; index++)
            {
                extracted[index] = new KeyValuePair<string, object?>(
                    parameterPrefix + extracted[index].Key, extracted[index].Value);
            }
            return extracted;
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

                        var context = new DataInjectionContext<TElement>(
                            query.trait.cc, query.trait.fieldComparer, reader);
                        ((IDataInjectable)element).Prepare(context);

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

                        var context = new DataInjectionContext<TElement>(
                            query.trait.cc, query.trait.fieldComparer, reader);
                        ((IDataInjectable)element).Prepare(context);

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
