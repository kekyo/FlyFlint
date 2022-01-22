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

        public override int ExecuteNonQuery(QueryContext query)
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                return command.ExecuteNonQuery();
            }
        }

        public override TElement ExecuteScalar<TElement>(QueryContext<TElement> query)
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                return InternalValueConverter<TElement>.converter.Convert(
                    query.trait.cc, command.ExecuteScalar());
            }
        }

        /////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static PreparingResult<TElement> CreateAndPrepareAndInject<TElement>(
            DataInjectionContext context, out TElement element)
            where TElement : new()
        {
            element = new();
            var pr = ((IDataInjectable<TElement>)element).Prepare(context);
            pr.Injector(ref element, context, pr.MetadataList);
            return pr;
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static void CreateAndInject<TElement>(
            DataInjectionContext context, PreparingResult<TElement> pr, out TElement element)
            where TElement : new()
        {
            element = new();
            pr.Injector(ref element, context, pr.MetadataList);
        }

        /////////////////////////////////////////////////////////////////////

        public override IEnumerable<TElement> Execute<TElement>(QueryContext<TElement> query)
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

                        TElement element;
                        var pr = CreateAndPrepareAndInject(context, out element);

                        yield return element;

                        while (reader.Read())
                        {
                            CreateAndInject(context, pr, out element);

                            yield return element;
                        }
                    }
                }
            }
        }

        /////////////////////////////////////////////////////////////////////

        public override async Task<int> ExecuteNonQueryAsync(QueryContext query)
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                return await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }

        public override async Task<TElement> ExecuteScalarAsync<TElement>(QueryContext<TElement> query)
        {
            using (var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters))
            {
                return InternalValueConverter<TElement>.converter.Convert(
                    query.trait.cc, await command.ExecuteScalarAsync().ConfigureAwait(false));
            }
        }

#if NET461_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        public override async IAsyncEnumerable<TElement> ExecuteAsync<TElement>(QueryContext<TElement> query)
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

                        TElement element;
                        var pr = CreateAndPrepareAndInject(context, out element);

                        yield return element;

                        while (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            CreateAndInject(context, pr, out element);

                            yield return element;
                        }
                    }
                }
            }
        }
#endif
    }
}
