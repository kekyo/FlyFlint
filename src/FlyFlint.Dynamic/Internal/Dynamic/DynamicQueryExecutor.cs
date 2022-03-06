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
using System.Data.Common;

namespace FlyFlint.Internal.Dynamic
{
    internal sealed class DynamicQueryExecutor : DynamicQueryExecutorFacade
    {
        public override object? Convert(ConversionContext context, object? value, Type targetType) =>
            DynamicValueConverter.GetConverter(targetType).Convert(context, value);

        private static DynamicParameterExtractionContext<TParameters> CreateParameterExtractionContext<TParameters>(
            ConversionContext cc)
            where TParameters : notnull =>
            typeof(TParameters).IsValueType ?
                new DynamicParameterExtractionByRefContext<TParameters>(cc) :
                new DynamicParameterExtractionObjRefContext<TParameters>(cc);

        public override Func<ExtractedParameter[]> GetConstructParameters<TParameters>(
            ConversionContext cc,
            string parameterPrefix,
            Func<TParameters> getter)
        {
            var context = CreateParameterExtractionContext<TParameters>(cc);
            return () =>
            {
                var parameters = getter();
                return context.ExtractParameters(ref parameters, parameterPrefix);
            };
        }

        public override ExtractedParameter[] GetParameters<TParameters>(
            ConversionContext cc,
            string parameterPrefix,
            ref TParameters parameters) =>
            CreateParameterExtractionContext<TParameters>(cc).
                ExtractParameters(ref parameters, parameterPrefix);

        public override RecordInjectorDelegate<TRecord> GetRecordInjector<TRecord>(
            ConversionContext cc,
            IComparer<string> fieldComparer,
            DbDataReader reader) =>
            new DynamicRecordInjectionContext<TRecord>(
                cc, fieldComparer, reader).Inject;
    }
}
