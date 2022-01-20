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
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace FlyFlint.Internal.Dynamic
{
    internal interface IDynamicQueryExecutor
    {
        object? Convert(ConversionContext context, object? value, Type targetType);
        object? UnsafeConvert(ConversionContext context, object value, Type targetType);

        /////////////////////////////////////////////////////////////////////

        Func<KeyValuePair<string, object?>[]> GetConstructParameters<TParameters>(Func<TParameters> getter, string parameterPrefix)
            where TParameters : notnull;
        KeyValuePair<string, object?>[] GetParameters<TParameters>(ref TParameters parameters, string parameterPrefix)
            where TParameters : notnull;

        /////////////////////////////////////////////////////////////////////

        int ExecuteNonQuery(QueryContext query);
        TElement ExecuteScalar<TElement>(QueryContext<TElement> query);
        IEnumerable<TElement> Execute<TElement>(QueryContext<TElement> query)
            where TElement : new();

        /////////////////////////////////////////////////////////////////////

        Task<int> ExecuteNonQueryAsync(QueryContext query);
        Task<TElement> ExecuteScalarAsync<TElement>(QueryContext<TElement> query);
#if NET461_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        IAsyncEnumerable<TElement> ExecuteAsync<TElement>(QueryContext<TElement> query)
            where TElement : new();
#endif
    }

    internal static class DynamicQueryExecutorFacade
    {
        private static volatile IDynamicQueryExecutor? executor;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static void SetDynamicQueryExecutor(IDynamicQueryExecutor executor) =>
            DynamicQueryExecutorFacade.executor = executor;
        
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static IDynamicQueryExecutor GetDynamicQueryExecutor()
        {
            if (executor == null)
            {
                throw new InvalidOperationException(
                    "Dynamic query feature was not enabled. You need to install `FlyFlint.Dynamic` NuGet package and call `DynamicQuery.Enable()`.");
            }
            return executor;
        }

        /////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal static object? Convert(ConversionContext context, object? value, Type targetType) =>
            GetDynamicQueryExecutor().Convert(context, value, targetType);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal static object? UnsafeConvert(ConversionContext context, object value, Type targetType) =>
            GetDynamicQueryExecutor().UnsafeConvert(context, value, targetType);

        /////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal static Func<KeyValuePair<string, object?>[]> GetConstructParameters<TParameters>(Func<TParameters> getter, string parameterPrefix)
            where TParameters : notnull =>
            GetDynamicQueryExecutor().GetConstructParameters(getter, parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal static KeyValuePair<string, object?>[] GetParameters<TParameters>(ref TParameters parameters, string parameterPrefix)
            where TParameters : notnull =>
            GetDynamicQueryExecutor().GetParameters(ref parameters, parameterPrefix);

        /////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal static int ExecuteNonQuery(QueryContext query) =>
            GetDynamicQueryExecutor().ExecuteNonQuery(query);
        
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal static TElement ExecuteScalar<TElement>(QueryContext<TElement> query) =>
            GetDynamicQueryExecutor().ExecuteScalar(query);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal static IEnumerable<TElement> Execute<TElement>(QueryContext<TElement> query)
            where TElement : new() =>
            GetDynamicQueryExecutor().Execute(query);

        /////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal static Task<int> ExecuteNonQueryAsync(QueryContext query) =>
            GetDynamicQueryExecutor().ExecuteNonQueryAsync(query);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal static Task<TElement> ExecuteScalarAsync<TElement>(QueryContext<TElement> query) =>
            GetDynamicQueryExecutor().ExecuteScalarAsync<TElement>(query);

#if NET461_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static IAsyncEnumerable<TElement> ExecuteAsync<TElement>(QueryContext<TElement> query)
            where TElement : new() =>
            GetDynamicQueryExecutor().ExecuteAsync(query);
#endif
    }
}
