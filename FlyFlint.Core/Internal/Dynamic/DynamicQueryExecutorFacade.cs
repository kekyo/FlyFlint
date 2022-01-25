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

namespace FlyFlint.Internal.Dynamic
{
    internal class DynamicQueryExecutorFacade
    {
        private static DynamicQueryExecutorFacade instance =
            new DynamicQueryExecutorFacade();

        public static DynamicQueryExecutorFacade Instance
        {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
            get => instance;
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void SetQueryExecutor(DynamicQueryExecutorFacade instance) =>
            DynamicQueryExecutorFacade.instance = instance;

        /////////////////////////////////////////////////////////////////////
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private protected DynamicQueryExecutorFacade()
        {
        }

        public virtual object? Convert(
            ConversionContext context, object? value, Type targetType) =>
            throw new InvalidOperationException(
                $"Dynamic query feature is not enabled: Type={targetType.FullName}");

        public virtual Func<ExtractedParameter[]> GetConstructParameters<TParameters>(
            ConversionContext cc,
            string parameterPrefix,
            Func<TParameters> getter)
            where TParameters : notnull =>
            throw new InvalidOperationException(
                $"Dynamic query feature is not enabled: Type={typeof(TParameters).FullName}");

        public virtual ExtractedParameter[] GetParameters<TParameters>(
            ConversionContext cc,
            string parameterPrefix,
            ref TParameters parameters)
            where TParameters : notnull =>
            throw new InvalidOperationException(
                $"Dynamic query feature is not enabled: Type={typeof(TParameters).FullName}");

        public virtual DataInjectorDelegate<TElement> GetDataInjector<TElement>(
            ConversionContext cc,
            IComparer<string> fieldComparer,
            DbDataReader reader)
            where TElement : notnull =>
            throw new InvalidOperationException(
                $"Dynamic query feature is not enabled: Type={typeof(TElement).FullName}");
    }
}
