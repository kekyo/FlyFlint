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

        public override Func<ExtractedParameter[]> GetConstructParameters<TParameters>(
            ConversionContext cc,
            string parameterPrefix,
            Func<TParameters> getter)
        {
            var members = DynamicHelper.GetGetterMetadataList<TParameters>();
            return () =>
            {
                var parameters = getter();
                var ps = new ExtractedParameter[members.Length];
                for (var index = 0; index < ps.Length; index++)
                {
                    var m = members[index];
                    ps[index] = new ExtractedParameter(
                        parameterPrefix + m.FieldName, m.Accessor(ref parameters, cc));
                }
                return ps;
            };
        }

        public override ExtractedParameter[] GetParameters<TParameters>(
            ConversionContext cc,
            string parameterPrefix,
            ref TParameters parameters)
        {
            var members = DynamicHelper.GetGetterMetadataList<TParameters>();
            var ps = new ExtractedParameter[members.Length];
            for (var index = 0; index < ps.Length; index++)
            {
                var m = members[index];
                ps[index] = new ExtractedParameter(
                    parameterPrefix + m.FieldName, m.Accessor(ref parameters, cc));
            }
            return ps;
        }

        public override DataInjectorDelegate<TElement> GetDataInjector<TElement>(
            ConversionContext cc,
            IComparer<string> fieldComparer,
            DbDataReader reader) =>
            new DynamicDataInjectionContext<TElement>(
                cc, fieldComparer, reader).Inject;
    }
}
