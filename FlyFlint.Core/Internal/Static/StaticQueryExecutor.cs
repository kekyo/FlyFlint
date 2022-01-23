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
using System.Data.Common;
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
 
        public override InjectorDelegate<TElement> GetInjector<TElement>(
            ConversionContext cc,
            IComparer<string> fieldComparer,
            DbDataReader reader,
            ref TElement element)
        {
            var context = new StaticDataInjectionContext<TElement>(
                cc, fieldComparer, reader);
            ((IDataInjectable)element).Prepare(context);
            return context.Inject;
        }
    }
}
