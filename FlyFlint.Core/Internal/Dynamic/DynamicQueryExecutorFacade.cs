////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlyFlint.Internal.Dynamic
{
    internal interface IDynamicQueryExecutor
    {
        object? Convert(IFormatProvider fp, Encoding encoding, object? value, Type targetType);
        object? UnsafeConvert(IFormatProvider fp, Encoding encoding, object value, Type targetType);

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
#if NETFRAMEWORK
                var thisPath = new Uri(typeof(DynamicQueryExecutorFacade).Assembly.CodeBase!).LocalPath;
#else
                var thisPath = typeof(DynamicQueryExecutorFacade).Assembly.Location;
#endif
                var basePath = Path.GetDirectoryName(thisPath)!;
                var path = Path.Combine(basePath, "FlyFlint.Dynamic.Core.dll");
                var assembly = Assembly.LoadFrom(path);
                var type = assembly.GetType("FlyFlint.Internal.Dynamic.DynamicQueryExecutor")!;
                if (executor == null)
                {
                    var newExecutor = (IDynamicQueryExecutor)Activator.CreateInstance(type)!;
                    Interlocked.CompareExchange(ref executor, newExecutor, null);
                }
            }
            return executor!;
        }

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal static object? Convert(IFormatProvider fp, Encoding encoding, object? value, Type targetType) =>
            GetDynamicQueryExecutor().Convert(fp, encoding, value, targetType);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal static object? UnsafeConvert(IFormatProvider fp, Encoding encoding, object value, Type targetType) =>
            GetDynamicQueryExecutor().UnsafeConvert(fp, encoding, value, targetType);


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
