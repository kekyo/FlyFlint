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
    internal sealed class DynamicQueryExecutor : QueryExecutor
    {
        public override object? Convert(ConversionContext context, object? value, Type targetType) =>
            DynamicValueConverter.GetConverter(targetType).Convert(context, value);

        public override object? UnsafeConvert(ConversionContext context, object value, Type targetType) =>
            DynamicValueConverter.GetConverter(targetType).UnsafeConvert(context, value);

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

        public override InjectorDelegate<TElement> GetInjector<TElement>(
            ConversionContext cc,
            IComparer<string> fieldComparer,
            DbDataReader reader,
            ref TElement element) =>
            new DynamicDataInjectionContext<TElement>(
                cc, fieldComparer, reader).Inject;
    }
}
