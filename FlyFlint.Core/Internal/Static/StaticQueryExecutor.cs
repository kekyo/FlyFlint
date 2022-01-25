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

namespace FlyFlint.Internal.Static
{
    internal static class StaticQueryExecutor
    {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Func<ExtractedParameter[]> GetConstructParameters<TParameters>(
            ConversionContext cc,
            string parameterPrefix,
            Func<TParameters> getter)
            where TParameters : notnull, IParameterExtractable
        {
            return () =>
            {
                var parameters = getter();
                return GetParameters(cc, parameterPrefix, ref parameters);
            };
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ExtractedParameter[] GetParameters<TParameters>(
            ConversionContext cc,
            string parameterPrefix,
            ref TParameters parameters)
            where TParameters : notnull, IParameterExtractable
        {
            var context = new StaticParameterExtractionContext(cc);
            var extracted = parameters.Extract(context);
            for (var index = 0; index < extracted.Length; index++)
            {
                extracted[index] = new ExtractedParameter(
                    parameterPrefix + extracted[index].Name, extracted[index].Value);
            }
            return extracted;
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static DataInjectorDelegate<TElement> GetDataInjector<TElement>(
            ConversionContext cc,
            IComparer<string> fieldComparer,
            DbDataReader reader,
            IDataInjectable di)
            where TElement : notnull
        {
            var context = new StaticDataInjectionContext<TElement>(
                cc, fieldComparer, reader);
            di.Prepare(context);

            return context.Inject;
        }
    }
}
