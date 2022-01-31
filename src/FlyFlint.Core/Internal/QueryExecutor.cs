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
using FlyFlint.Internal.Dynamic;
using FlyFlint.Internal.Static;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace FlyFlint.Internal
{
    internal static class QueryExecutor
    {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T ConvertTo<T>(
            ConversionContext cc, object? value) =>
            InternalValueConverter<T>.converter.ConvertTo(cc, value);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static object? ConvertTo(
            ConversionContext cc, object? value, Type targetType) =>
            DynamicQueryExecutorFacade.Instance.Convert(cc, value, targetType);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Func<ExtractedParameter[]> GetConstructParameters<TParameters>(
            ConversionContext cc, 
            string parameterPrefix,
            Func<TParameters> getter)
            where TParameters : notnull =>
            () =>
            {
                var parameters = getter();
                return parameters is IParameterExtractable pe ?
                    StaticQueryExecutor.GetParameters(
                        cc, parameterPrefix, ref pe) :
                    DynamicQueryExecutorFacade.Instance.GetParameters(
                        cc, parameterPrefix, ref parameters);
            };

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ExtractedParameter[] GetParameters<TParameters>(
            ConversionContext cc,
            string parameterPrefix,
            ref TParameters parameters)
            where TParameters : notnull =>
            parameters is IParameterExtractable pe ?
                StaticQueryExecutor.GetParameters(
                    cc, parameterPrefix, ref pe) :
                DynamicQueryExecutorFacade.Instance.GetParameters(
                    cc, parameterPrefix, ref parameters);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static DataInjectorDelegate<TElement> GetDataInjector<TElement>(
            ConversionContext cc,
            IComparer<string> fieldComparer,
            DbDataReader reader,
            ref TElement element)
            where TElement : notnull =>
            element is IDataInjectable di ?
                StaticQueryExecutor.GetDataInjector<TElement>(
                    cc, fieldComparer, reader, di) :
                DynamicQueryExecutorFacade.Instance.GetDataInjector<TElement>(
                    cc, fieldComparer, reader);
    }
}
