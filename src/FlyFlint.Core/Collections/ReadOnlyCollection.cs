////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FlyFlint.Collections
{
    public struct ReadOnlyCollection<TValue> :
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        IReadOnlyList<TValue>,
#endif
        IList<TValue>, IList
    {
        private readonly List<TValue> list;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public ReadOnlyCollection(List<TValue> list) =>
            this.list = list;

        public int Count
        {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
            get => this.list.Count;
        }

        public TValue this[int index]
        {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
            get => this.list[index];
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public List<TValue>.Enumerator GetEnumerator() =>
            list.GetEnumerator();

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public bool Contains(TValue item) =>
            this.list.Contains(item);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public int IndexOf(TValue item) =>
            this.list.IndexOf(item);

        ///////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static explicit operator TValue[](ReadOnlyCollection<TValue> collection) =>
            collection.list.ToArray();

        ///////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() =>
            this.GetEnumerator();

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        IEnumerator IEnumerable.GetEnumerator() =>
            this.GetEnumerator();

        int ICollection<TValue>.Count
        {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
            get => this.list.Count;
        }

        bool ICollection<TValue>.IsReadOnly =>
            true;

        bool IList.IsReadOnly =>
            true;

        bool IList.IsFixedSize =>
            true;

        object ICollection.SyncRoot =>
            this;

        bool ICollection.IsSynchronized =>
            true;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        int IReadOnlyCollection<TValue>.Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.list.Count;
        }

        TValue IReadOnlyList<TValue>.this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.list[index];
        }
#endif

        TValue IList<TValue>.this[int index]
        {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
            get => this.list[index];
            set => throw new InvalidOperationException();
        }

        object? IList.this[int index]
        {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
            get => this.list[index];
            set => throw new InvalidOperationException();
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        bool ICollection<TValue>.Contains(TValue item) =>
            this.list.Contains(item);

        public void Add(TValue item) =>
            throw new InvalidOperationException();

        public void Clear() =>
            throw new InvalidOperationException();

        public void CopyTo(TValue[] array, int arrayIndex) =>
            this.list.CopyTo(array, arrayIndex);

        public bool Remove(TValue item) =>
            throw new InvalidOperationException();

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        int IList<TValue>.IndexOf(TValue item) =>
            this.list.IndexOf(item);

        void IList<TValue>.Insert(int index, TValue item) =>
            throw new InvalidOperationException();

        void IList<TValue>.RemoveAt(int index) =>
            throw new InvalidOperationException();

        int IList.Add(object? value) =>
            throw new InvalidOperationException();

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        bool IList.Contains(object? value) =>
            this.list.Contains((TValue)value!);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        int IList.IndexOf(object? value) =>
            this.list.IndexOf((TValue)value!);

        void IList.Insert(int index, object? value) =>
            throw new InvalidOperationException();

        void IList.Remove(object? value) =>
            throw new InvalidOperationException();

        void IList.RemoveAt(int index) =>
            throw new InvalidOperationException();

        void ICollection.CopyTo(Array array, int index) =>
            ((ICollection)this.list).CopyTo(array, index);
    }
}
