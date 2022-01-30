////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

#if NET461_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable CS1998

namespace FlyFlint.Collections
{
    // Simple and fun staff for easy usable async LINQ.
    // If you need for more complex usage, please use `System.Linq.Async` NuGet package instead.
    public static class Enumerable
    {
        public static async IAsyncEnumerable<T> AsAsyncEnumerable<T>(
            this IEnumerable<T> enumerable,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            foreach (var item in enumerable)
            {
                ct.ThrowIfCancellationRequested();
                yield return item;
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        public static async ValueTask<T[]> ToArrayAsync<T>(
            this IAsyncEnumerable<T> enumerable,
            CancellationToken ct = default)
        {
            var result = new List<T>();
            await foreach (var item in enumerable.
                WithCancellation(ct).
                ConfigureAwait(false))
            {
                result.Add(item);
            }
            return result.ToArray();
        }

        ////////////////////////////////////////////////////////////////////////////

        public static async ValueTask<ReadOnlyHashSet<T>> ToHashSetAsync<T>(
            this IAsyncEnumerable<T> enumerable,
            IEqualityComparer<T>? comparer = default,
            CancellationToken ct = default)
        {
            var result = new HashSet<T>(comparer ?? EqualityComparer<T>.Default);
            await foreach (var item in enumerable.
                WithCancellation(ct).
                ConfigureAwait(false))
            {
                result.Add(item);
            }
            return new ReadOnlyHashSet<T>(result);
        }

        ////////////////////////////////////////////////////////////////////////////

        public static async ValueTask<ReadOnlyDictionary<TKey, T>> ToDictionaryAsync<T, TKey>(
            this IAsyncEnumerable<T> enumerable,
            Func<T, TKey> keySelector,
            IEqualityComparer<TKey>? comparer = default,
            CancellationToken ct = default)
            where TKey : notnull
        {
            var result = new Dictionary<TKey, T>(comparer ?? EqualityComparer<TKey>.Default);
            await foreach (var item in enumerable.
                WithCancellation(ct).
                ConfigureAwait(false))
            {
                result.Add(
                    keySelector(item),
                    item);
            }
            return new ReadOnlyDictionary<TKey, T>(result);
        }

        public static async ValueTask<ReadOnlyDictionary<TKey, TValue>> ToDictionaryAsync<T, TKey, TValue>(
            this IAsyncEnumerable<T> enumerable,
            Func<T, TKey> keySelector,
            Func<T, TValue> valueSelector,
            IEqualityComparer<TKey>? comparer = default,
            CancellationToken ct = default)
            where TKey : notnull
        {
            var result = new Dictionary<TKey, TValue>(comparer ?? EqualityComparer<TKey>.Default);
            await foreach (var item in enumerable.
                WithCancellation(ct).
                ConfigureAwait(false))
            {
                result.Add(
                    keySelector(item),
                    valueSelector(item));
            }
            return new ReadOnlyDictionary<TKey, TValue>(result);
        }

        public static async ValueTask<ReadOnlyDictionary<TKey, T>> ToDictionaryAsync<T, TKey>(
            this IAsyncEnumerable<T> enumerable,
            Func<T, ValueTask<TKey>> keySelector,
            IEqualityComparer<TKey>? comparer = default,
            CancellationToken ct = default)
            where TKey : notnull
        {
            var result = new Dictionary<TKey, T>(comparer ?? EqualityComparer<TKey>.Default);
            await foreach (var item in enumerable.
                WithCancellation(ct).
                ConfigureAwait(false))
            {
                result.Add(
                    await keySelector(item).ConfigureAwait(false),
                    item);
            }
            return new ReadOnlyDictionary<TKey, T>(result);
        }

        public static async ValueTask<ReadOnlyDictionary<TKey, TValue>> ToDictionaryAsync<T, TKey, TValue>(
            this IAsyncEnumerable<T> enumerable,
            Func<T, ValueTask<TKey>> keySelector,
            Func<T, TValue> valueSelector,
            IEqualityComparer<TKey>? comparer = default,
            CancellationToken ct = default)
            where TKey : notnull
        {
            var result = new Dictionary<TKey, TValue>(comparer ?? EqualityComparer<TKey>.Default);
            await foreach (var item in enumerable.
                WithCancellation(ct).
                ConfigureAwait(false))
            {
                result.Add(
                    await keySelector(item).ConfigureAwait(false),
                    valueSelector(item));
            }
            return new ReadOnlyDictionary<TKey, TValue>(result);
        }

        public static async ValueTask<ReadOnlyDictionary<TKey, TValue>> ToDictionaryAsync<T, TKey, TValue>(
            this IAsyncEnumerable<T> enumerable,
            Func<T, TKey> keySelector,
            Func<T, ValueTask<TValue>> valueSelector,
            IEqualityComparer<TKey>? comparer = default,
            CancellationToken ct = default)
            where TKey : notnull
        {
            var result = new Dictionary<TKey, TValue>(comparer ?? EqualityComparer<TKey>.Default);
            await foreach (var item in enumerable.
                WithCancellation(ct).
                ConfigureAwait(false))
            {
                result.Add(
                    keySelector(item),
                    await valueSelector(item).ConfigureAwait(false));
            }
            return new ReadOnlyDictionary<TKey, TValue>(result);
        }

        public static async ValueTask<ReadOnlyDictionary<TKey, TValue>> ToDictionaryAsync<T, TKey, TValue>(
            this IAsyncEnumerable<T> enumerable,
            Func<T, ValueTask<TKey>> keySelector,
            Func<T, ValueTask<TValue>> valueSelector,
            IEqualityComparer<TKey>? comparer = default,
            CancellationToken ct = default)
            where TKey : notnull
        {
            var result = new Dictionary<TKey, TValue>(comparer ?? EqualityComparer<TKey>.Default);
            await foreach (var item in enumerable.
                WithCancellation(ct).
                ConfigureAwait(false))
            {
                result.Add(
                    await keySelector(item).ConfigureAwait(false),
                    await valueSelector(item).ConfigureAwait(false));
            }
            return new ReadOnlyDictionary<TKey, TValue>(result);
        }

        ////////////////////////////////////////////////////////////////////////////

        public static async IAsyncEnumerable<T> Where<T>(
            this IAsyncEnumerable<T> enumerable,
            Func<T, bool> predicate,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            await foreach (var item in enumerable.
                WithCancellation(ct))
            {
                if (predicate(item))
                {
                    yield return item;
                }
            }
        }

        public static async IAsyncEnumerable<T> Where<T>(
            this IAsyncEnumerable<T> enumerable,
            Func<T, ValueTask<bool>> predicate,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            await foreach (var item in enumerable.
                WithCancellation(ct))
            {
                if (await predicate(item))
                {
                    yield return item;
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        public static async IAsyncEnumerable<U> Select<T, U>(
            this IAsyncEnumerable<T> enumerable,
            Func<T, U> mapper,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            await foreach (var item in enumerable.
                WithCancellation(ct))
            {
                yield return mapper(item);
            }
        }

        public static async IAsyncEnumerable<U> Select<T, U>(
            this IAsyncEnumerable<T> enumerable,
            Func<T, ValueTask<U>> mapper,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            await foreach (var item in enumerable.
                WithCancellation(ct))
            {
                yield return await mapper(item);
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        public static async IAsyncEnumerable<U> SelectMany<T, U>(
            this IAsyncEnumerable<T> enumerable,
            Func<T, IEnumerable<U>> binder,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            await foreach (var item in enumerable.
                WithCancellation(ct))
            {
                foreach (var inner in binder(item))
                {
                    yield return inner;
                }
            }
        }

        public static async IAsyncEnumerable<U> SelectMany<T, U>(
            this IEnumerable<T> enumerable,
            Func<T, IAsyncEnumerable<U>> binder,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            foreach (var item in enumerable)
            {
                await foreach (var inner in binder(item).
                    WithCancellation(ct))
                {
                    yield return inner;
                }
            }
        }

        public static async IAsyncEnumerable<U> SelectMany<T, U>(
            this IAsyncEnumerable<T> enumerable,
            Func<T, IAsyncEnumerable<U>> binder,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            await foreach (var item in enumerable.WithCancellation(ct))
            {
                await foreach (var inner in binder(item).
                    WithCancellation(ct))
                {
                    yield return inner;
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        public static async ValueTask<T> FirstAsync<T>(
            this IAsyncEnumerable<T> enumerable,
            CancellationToken ct = default)
        {
            await foreach (var item in enumerable.
                WithCancellation(ct).
                ConfigureAwait(false))
            {
                return item;
            }
            throw new InvalidOperationException("The source sequence is empty.");
        }

        public static async ValueTask<T> FirstAsync<T>(
            this IAsyncEnumerable<T> enumerable,
            Func<T, bool> predicate,
            CancellationToken ct = default)
        {
            await foreach (var item in enumerable.
                WithCancellation(ct))
            {
                if (predicate(item))
                {
                    return item;
                }
            }
            throw new InvalidOperationException("The source sequence is empty.");
        }

        public static async ValueTask<T> FirstAsync<T>(
            this IAsyncEnumerable<T> enumerable,
            Func<T, ValueTask<bool>> predicate,
            CancellationToken ct = default)
        {
            await foreach (var item in enumerable.
                WithCancellation(ct))
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
            this IAsyncEnumerable<T> enumerable,
            T defaultValue = default!,
            CancellationToken ct = default)
        {
            await foreach (var item in enumerable.
                WithCancellation(ct).
                ConfigureAwait(false))
            {
                return item;
            }
            return defaultValue;
        }

        public static async ValueTask<T> FirstOrDefaultAsync<T>(
            this IAsyncEnumerable<T> enumerable,
            Func<T, bool> predicate,
            T defaultValue = default!,
            CancellationToken ct = default)
        {
            await foreach (var item in
                enumerable.WithCancellation(ct))
            {
                if (predicate(item))
                {
                    return item;
                }
            }
            return defaultValue;
        }

        public static async ValueTask<T> FirstOrDefaultAsync<T>(
            this IAsyncEnumerable<T> enumerable,
            Func<T, ValueTask<bool>> predicate,
            T defaultValue = default!,
            CancellationToken ct = default)
        {
            await foreach (var item in enumerable.
                WithCancellation(ct))
            {
                if (await predicate(item))
                {
                    return item;
                }
            }
            return defaultValue;
        }
    }
}
#endif
