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
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace FlyFlint.Internal.Dynamic
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IDynamicQueryExecutor
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        (string name, object? value)[] GetParameters<TParameters>(
            ref TParameters parameters, string parameterPrefix);

        [EditorBrowsable(EditorBrowsableState.Never)]
        IEnumerable<T> Execute<T>(QueryContext<T> query)
            where T : new();

#if !NET40 && !NET45
        [EditorBrowsable(EditorBrowsableState.Never)]
        IAsyncEnumerable<T> ExecuteAsync<T>(QueryContext<T> query)
            where T : new();
#endif
    }
    
    public static class DynamicQueryExecutorFacade
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
        internal static (string name, object? value)[] GetParameters<TParameters>(
            ref TParameters parameters, string parameterPrefix) =>
            GetDynamicQueryExecutor().GetParameters(ref parameters, parameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal static IEnumerable<T> Execute<T>(QueryContext<T> query)
            where T : new() =>
            GetDynamicQueryExecutor().Execute(query);

#if !NET40 && !NET45
        internal static IAsyncEnumerable<T> ExecuteAsync<T>(QueryContext<T> query)
            where T : new() =>
            GetDynamicQueryExecutor().ExecuteAsync(query);
#endif
    }
}
