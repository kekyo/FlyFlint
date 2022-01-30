////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FlyFlint.Collections
{
    public static class CollectionsExtension
    {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(
            this Dictionary<TKey, TValue> dictionary)
            where TKey : notnull =>
            new ReadOnlyDictionary<TKey, TValue>(dictionary);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ReadOnlyHashSet<T> AsReadOnly<T>(
            this HashSet<T> hashSet) =>
            new ReadOnlyHashSet<T>(hashSet);
    }

    public sealed class ReadOnlyDictionary<TKey, TValue>
#if NET45_OR_GREATER
        : IReadOnlyDictionary<TKey, TValue>
#else
        : IEnumerable<KeyValuePair<TKey, TValue>>
#endif
        where TKey : notnull
    {
        private readonly Dictionary<TKey, TValue> dictionary;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public ReadOnlyDictionary(Dictionary<TKey, TValue> dictionary) =>
            this.dictionary = dictionary;

        public TValue this[TKey key] =>
            this.dictionary[key];

        public IEnumerable<TKey> Keys =>
            this.dictionary.Keys;

        public IEnumerable<TValue> Values =>
            this.dictionary.Values;

        public int Count =>
            this.dictionary.Count;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public bool ContainsKey(TKey key) =>
            this.dictionary.ContainsKey(key);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() =>
            this.dictionary.GetEnumerator();

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public bool TryGetValue(TKey key, out TValue value) =>
            this.dictionary.TryGetValue(key, out value!);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        IEnumerator IEnumerable.GetEnumerator() =>
            this.GetEnumerator();
    }

    public sealed class ReadOnlyHashSet<T>
#if NET5_0_OR_GREATER
        : IReadOnlySet<T>
#elif NET45_OR_GREATER
        : IReadOnlyCollection<T>
#else
        : IEnumerable<T>
#endif
    {
        private readonly HashSet<T> hashSet;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public ReadOnlyHashSet(HashSet<T> hashSet) =>
            this.hashSet = hashSet;

        public int Count =>
            this.hashSet.Count;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public bool Contains(T item) =>
            this.hashSet.Contains(item);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public IEnumerator<T> GetEnumerator() =>
            this.hashSet.GetEnumerator();

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public bool IsProperSubsetOf(IEnumerable<T> other) =>
            this.hashSet.IsProperSubsetOf(other);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public bool IsProperSupersetOf(IEnumerable<T> other) =>
            this.hashSet.IsProperSupersetOf(other);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public bool IsSubsetOf(IEnumerable<T> other) =>
            this.hashSet.IsSubsetOf(other);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public bool IsSupersetOf(IEnumerable<T> other) =>
            this.hashSet.IsSupersetOf(other);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public bool Overlaps(IEnumerable<T> other) =>
            this.hashSet.Overlaps(other);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public bool SetEquals(IEnumerable<T> other) =>
            this.hashSet.SetEquals(other);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        IEnumerator IEnumerable.GetEnumerator() =>
            this.GetEnumerator();
    }
}
