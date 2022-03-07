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
            parameters.Extract(context);

            return context.ExtractParameters(parameterPrefix);
        }

        /////////////////////////////////////////////////////////////////

        private static class RecordInjectorGenerator<TRecord>
            where TRecord : notnull   // Couldn't apply IRecordInjectable constraint.
        {
            private static readonly bool isValueType =
                typeof(TRecord).IsValueType;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
            public static RecordInjectorDelegate<TRecord> GetRecordInjector(
                ConversionContext cc,
                IComparer<string> fieldComparer,
                DbDataReader reader,
                IRecordInjectable di)
            {
                StaticRecordInjectionContext<TRecord> context =
                    isValueType ?
                        new StaticRecordInjectionByRefContext<TRecord>(
                            cc, fieldComparer, reader) :
                        new StaticRecordInjectionObjRefContext<TRecord>(
                            cc, fieldComparer, reader);
                di.Prepare(context);
                context.MakeInjectable();

                return context.Inject;
            }
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static RecordInjectorDelegate<TRecord> GetRecordInjector<TRecord>(
            ConversionContext cc,
            IComparer<string> fieldComparer,
            DbDataReader reader,
            IRecordInjectable di)
            where TRecord : notnull =>
            RecordInjectorGenerator<TRecord>.GetRecordInjector(
                cc, fieldComparer, reader, di);
    }
}
