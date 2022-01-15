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
using System.Threading.Tasks;

namespace FlyFlint.Utilities
{
#if !NET40 && !NET45
    // Simple and fun staff for easy usable async LINQ.
    // If you need for more complex usage, please use `System.Linq.Async` NuGet package instead.
    public static class AsyncEnumerableExtension
    {
        public static async ValueTask<T[]> ToArrayAsync<T>(
            this IAsyncEnumerable<T> enumerable)
        {
            var result = new List<T>();
            await foreach (var item in enumerable)
            {
                result.Add(item);
            }
            return result.ToArray();
        }

        ////////////////////////////////////////////////////////////////////////////

        public static async IAsyncEnumerable<T> Where<T>(
            this IAsyncEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            await foreach (var item in enumerable)
            {
                if (predicate(item))
                {
                    yield return item;
                }
            }
        }

        public static async IAsyncEnumerable<T> Where<T>(
            this IAsyncEnumerable<T> enumerable, Func<T, ValueTask<bool>> predicate)
        {
            await foreach (var item in enumerable)
            {
                if (await predicate(item))
                {
                    yield return item;
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        public static async IAsyncEnumerable<U> Select<T, U>(
            this IAsyncEnumerable<T> enumerable, Func<T, U> mapper)
        {
            await foreach (var item in enumerable)
            {
                yield return mapper(item);
            }
        }

        public static async IAsyncEnumerable<U> Select<T, U>(
            this IAsyncEnumerable<T> enumerable, Func<T, ValueTask<U>> mapper)
        {
            await foreach (var item in enumerable)
            {
                yield return await mapper(item);
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        public static async IAsyncEnumerable<U> SelectMany<T, U>(
            this IAsyncEnumerable<T> enumerable, Func<T, IEnumerable<U>> binder)
        {
            await foreach (var item in enumerable)
            {
                foreach (var inner in binder(item))
                {
                    yield return inner;
                }
            }
        }

        public static async IAsyncEnumerable<U> SelectMany<T, U>(
            this IEnumerable<T> enumerable, Func<T, IAsyncEnumerable<U>> binder)
        {
            foreach (var item in enumerable)
            {
                await foreach (var inner in binder(item))
                {
                    yield return inner;
                }
            }
        }

        public static async IAsyncEnumerable<U> SelectMany<T, U>(
            this IAsyncEnumerable<T> enumerable, Func<T, IAsyncEnumerable<U>> binder)
        {
            await foreach (var item in enumerable)
            {
                await foreach (var inner in binder(item))
                {
                    yield return inner;
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        public static async ValueTask<T> FirstAsync<T>(
            this IAsyncEnumerable<T> enumerable)
        {
            await foreach (var item in enumerable)
            {
                return item;
            }
            throw new InvalidOperationException("The source sequence is empty.");
        }

        public static async ValueTask<T> FirstAsync<T>(
            this IAsyncEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            await foreach (var item in enumerable)
            {
                if (predicate(item))
                {
                    return item;
                }
            }
            throw new InvalidOperationException("The source sequence is empty.");
        }

        public static async ValueTask<T> FirstAsync<T>(
            this IAsyncEnumerable<T> enumerable, Func<T, ValueTask<bool>> predicate)
        {
            await foreach (var item in enumerable)
            {
                if (await predicate(item))
                {
                    return item;
                }
            }
            throw new InvalidOperationException("The source sequence is empty.");
        }

        ////////////////////////////////////////////////////////////////////////////

        public static async ValueTask<T> FirstOrDefaultAsync<T>(
            this IAsyncEnumerable<T> enumerable, T defaultValue = default!)
        {
            await foreach (var item in enumerable)
            {
                return item;
            }
            return defaultValue;
        }

        public static async ValueTask<T> FirstOrDefaultAsync<T>(
            this IAsyncEnumerable<T> enumerable, Func<T, bool> predicate, T defaultValue = default!)
        {
            await foreach (var item in enumerable)
            {
                if (predicate(item))
                {
                    return item;
                }
            }
            return defaultValue;
        }

        public static async ValueTask<T> FirstOrDefaultAsync<T>(
            this IAsyncEnumerable<T> enumerable, Func<T, ValueTask<bool>> predicate, T defaultValue = default!)
        {
            await foreach (var item in enumerable)
            {
                if (await predicate(item))
                {
                    return item;
                }
            }
            return defaultValue;
        }
    }
#endif
}
