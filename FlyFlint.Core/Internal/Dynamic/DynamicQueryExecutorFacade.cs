////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Internal.Converter;
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

        (string name, object? value)[] GetParameters<TParameters>(
            ref TParameters parameters, string parameterPrefix);

        int ExecuteNonQuery(QueryContext query);
        T ExecuteScalar<T>(QueryContext query);
        IEnumerable<T> Execute<T>(QueryContext<T> query)
            where T : new();

        Task<int> ExecuteNonQueryAsync(QueryContext query);
        Task<T> ExecuteScalarAsync<T>(QueryContext query);
#if !NET40 && !NET45
        IAsyncEnumerable<T> ExecuteAsync<T>(QueryContext<T> query)
            where T : new();
#endif
    }
    
    internal static class DynamicQueryExecutorFacade
    {
        private static volatile IDynamicQueryExecutor? executor;

        public static void SetDynamicQueryExecutor(IDynamicQueryExecutor executor) =>
            DynamicQueryExecutorFacade.executor = executor;
        
        private static IDynamicQueryExecutor GetDynamicQueryExecutor()
        {
            if (executor == null)
            {
                throw new InvalidOperationException(
                    "Dynamic query feature was not enabled. You need to install `FlyFlint.Dynamic` NuGet package and call `DynamicQuery.Enable()`.");
            }
            return executor;
        }

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal static object? Convert(ConversionContext context, object? value, Type targetType) =>
            GetDynamicQueryExecutor().Convert(context, value, targetType);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal static object? UnsafeConvert(ConversionContext context, object value, Type targetType) =>
            GetDynamicQueryExecutor().UnsafeConvert(context, value, targetType);


#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal static (string name, object? value)[] GetParameters<TParameters>(
            ref TParameters parameters, string parameterPrefix) =>
            GetDynamicQueryExecutor().GetParameters(ref parameters, parameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal static int ExecuteNonQuery(QueryContext query) =>
            GetDynamicQueryExecutor().ExecuteNonQuery(query);
        
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal static T ExecuteScalar<T>(QueryContext query) =>
            GetDynamicQueryExecutor().ExecuteScalar<T>(query);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal static IEnumerable<T> Execute<T>(QueryContext<T> query)
            where T : new() =>
            GetDynamicQueryExecutor().Execute(query);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal static Task<int> ExecuteNonQueryAsync(QueryContext query) =>
            GetDynamicQueryExecutor().ExecuteNonQueryAsync(query);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal static Task<T> ExecuteScalarAsync<T>(QueryContext query) =>
            GetDynamicQueryExecutor().ExecuteScalarAsync<T>(query);

#if !NET40 && !NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static IAsyncEnumerable<T> ExecuteAsync<T>(QueryContext<T> query)
            where T : new() =>
            GetDynamicQueryExecutor().ExecuteAsync(query);
#endif
    }
}
