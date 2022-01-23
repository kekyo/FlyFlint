////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Context;
using FlyFlint.Internal.Static;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace FlyFlint.Internal
{
    internal abstract class QueryExecutor
    {
        private static QueryExecutor executor = StaticQueryExecutor.Instance;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void SetQueryExecutor(QueryExecutor executor) =>
            QueryExecutor.executor = executor;
        
        public static QueryExecutor Instance
        {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
            get => QueryExecutor.executor;
        }

        /////////////////////////////////////////////////////////////////////

        public abstract object? Convert(
            ConversionContext context, object? value, Type targetType);
        public abstract object? UnsafeConvert(
            ConversionContext context, object value, Type targetType);

        public abstract Func<KeyValuePair<string, object?>[]> GetConstructParameters<TParameters>(
            Func<TParameters> getter, string parameterPrefix)
            where TParameters : notnull;
        public abstract KeyValuePair<string, object?>[] GetParameters<TParameters>(
            ref TParameters parameters, string parameterPrefix)
            where TParameters : notnull;

        public abstract InjectorDelegate<TElement> GetInjector<TElement>(
            ConversionContext cc,
            IComparer<string> fieldComparer,
            DbDataReader reader,
            ref TElement element)
            where TElement : notnull;
    }
}
