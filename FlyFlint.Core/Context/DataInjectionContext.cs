﻿////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Internal;
using FlyFlint.Internal.Static;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace FlyFlint.Context
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class DataInjectionContext
    {
        internal readonly ConversionContext cc;
        internal readonly IComparer<string> fieldComparer;
        internal readonly DbDataReader reader;
        private protected DataInjectionMetadata[] metadataList = null!;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private protected DataInjectionContext(
            ConversionContext cc, IComparer<string> fieldComparer, DbDataReader reader)
        {
            this.cc = cc;
            this.fieldComparer = fieldComparer;
            this.reader = reader;
        }

        public abstract void RegisterMetadata(
            KeyValuePair<string, Type>[] members, Delegate injector);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool GetBoolean(int metadataIndex)
        {
            var metadata = metadataList[metadataIndex];
            return metadata.StoreDirect ? this.reader.GetBoolean(metadata.DbFieldIndex) :
                this.cc.Convert<bool>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public byte GetByte(int metadataIndex)
        {
            var metadata = metadataList[metadataIndex];
            return metadata.StoreDirect ? this.reader.GetByte(metadata.DbFieldIndex) :
                this.cc.Convert<byte>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public short GetInt16(int metadataIndex)
        {
            var metadata = metadataList[metadataIndex];
            return metadata.StoreDirect ? this.reader.GetInt16(metadata.DbFieldIndex) :
                this.cc.Convert<short>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int GetInt32(int metadataIndex)
        {
            var metadata = metadataList[metadataIndex];
            return metadata.StoreDirect ? this.reader.GetInt32(metadata.DbFieldIndex) :
                this.cc.Convert<int>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public long GetInt64(int metadataIndex)
        {
            var metadata = metadataList[metadataIndex];
            return metadata.StoreDirect ? this.reader.GetInt64(metadata.DbFieldIndex) :
                this.cc.Convert<long>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public float GetSingle(int metadataIndex)
        {
            var metadata = metadataList[metadataIndex];
            return metadata.StoreDirect ? this.reader.GetFloat(metadata.DbFieldIndex) :
                this.cc.Convert<float>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public double GetDouble(int metadataIndex)
        {
            var metadata = metadataList[metadataIndex];
            return metadata.StoreDirect ? this.reader.GetDouble(metadata.DbFieldIndex) :
                this.cc.Convert<double>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public decimal GetDecimal(int metadataIndex)
        {
            var metadata = metadataList[metadataIndex];
            return metadata.StoreDirect ? this.reader.GetDecimal(metadata.DbFieldIndex) :
                this.cc.Convert<decimal>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public char GetChar(int metadataIndex)
        {
            var metadata = metadataList[metadataIndex];
            return metadata.StoreDirect ? this.reader.GetChar(metadata.DbFieldIndex) :
                this.cc.Convert<char>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Guid GetGuid(int metadataIndex)
        {
            var metadata = metadataList[metadataIndex];
            return metadata.StoreDirect ? this.reader.GetGuid(metadata.DbFieldIndex) :
                this.cc.Convert<Guid>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public DateTime GetDateTime(int metadataIndex)
        {
            var metadata = metadataList[metadataIndex];
            return metadata.StoreDirect ? this.reader.GetDateTime(metadata.DbFieldIndex) :
                this.cc.Convert<DateTime>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public TEnum GetEnum<TEnum>(int metadataIndex)
            where TEnum : struct, Enum
        {
            var metadata = metadataList[metadataIndex];
            return metadata.StoreDirect ? (TEnum)this.reader.GetValue(metadata.DbFieldIndex) :
                this.cc.Convert<TEnum>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string GetString(int metadataIndex)
        {
            var metadata = metadataList[metadataIndex];
            return metadata.StoreDirect ? this.reader.GetString(metadata.DbFieldIndex) :
                this.cc.Convert<string>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public byte[] GetBytes(int metadataIndex)
        {
            var metadata = metadataList[metadataIndex];
            return metadata.StoreDirect ? (byte[])this.reader.GetValue(metadata.DbFieldIndex) :
                this.cc.Convert<byte[]>(this.reader.GetValue(metadata.DbFieldIndex));
        }

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool? GetNullableBoolean(int metadataIndex)
        {
            var metadata = metadataList[metadataIndex];
            return this.reader.IsDBNull(metadata.DbFieldIndex) ? null :
                metadata.StoreDirect ? this.reader.GetBoolean(metadata.DbFieldIndex) :
                    this.cc.Convert<bool?>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public byte? GetNullableByte(int metadataIndex)
        {
            var metadata = metadataList[metadataIndex];
            return this.reader.IsDBNull(metadata.DbFieldIndex) ? null :
                metadata.StoreDirect ? this.reader.GetByte(metadata.DbFieldIndex) :
                    this.cc.Convert<byte?>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public short? GetNullableInt16(int metadataIndex)
        {
            var metadata = metadataList[metadataIndex];
            return this.reader.IsDBNull(metadata.DbFieldIndex) ? null :
                metadata.StoreDirect ? this.reader.GetInt16(metadata.DbFieldIndex) :
                    this.cc.Convert<short?>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int? GetNullableInt32(int metadataIndex)
        {
            var metadata = metadataList[metadataIndex];
            return this.reader.IsDBNull(metadata.DbFieldIndex) ? null :
                metadata.StoreDirect ? this.reader.GetInt32(metadata.DbFieldIndex) :
                    this.cc.Convert<int?>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public long? GetNullableInt64(int metadataIndex)
        {
            var metadata = metadataList[metadataIndex];
            return this.reader.IsDBNull(metadata.DbFieldIndex) ? null :
                metadata.StoreDirect ? this.reader.GetInt64(metadata.DbFieldIndex) :
                    this.cc.Convert<long?>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public float? GetNullableSingle(int metadataIndex)
        {
            var metadata = metadataList[metadataIndex];
            return this.reader.IsDBNull(metadata.DbFieldIndex) ? null :
                metadata.StoreDirect ? this.reader.GetFloat(metadata.DbFieldIndex) :
                    this.cc.Convert<float?>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public double? GetNullableDouble(int metadataIndex)
        {
            var metadata = metadataList[metadataIndex];
            return this.reader.IsDBNull(metadata.DbFieldIndex) ? null :
                metadata.StoreDirect ? this.reader.GetDouble(metadata.DbFieldIndex) :
                    this.cc.Convert<double?>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public decimal? GetNullableDecimal(int metadataIndex)
        {
            var metadata = metadataList[metadataIndex];
            return this.reader.IsDBNull(metadata.DbFieldIndex) ? null :
                metadata.StoreDirect ? this.reader.GetDecimal(metadata.DbFieldIndex) :
                    this.cc.Convert<decimal?>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public char? GetNullableChar(int metadataIndex)
        {
            var metadata = metadataList[metadataIndex];
            return this.reader.IsDBNull(metadata.DbFieldIndex) ? null :
                metadata.StoreDirect ? this.reader.GetChar(metadata.DbFieldIndex) :
                    this.cc.Convert<char?>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Guid? GetNullableGuid(int metadataIndex)
        {
            var metadata = metadataList[metadataIndex];
            return this.reader.IsDBNull(metadata.DbFieldIndex) ? null :
                metadata.StoreDirect ? this.reader.GetGuid(metadata.DbFieldIndex) :
                    this.cc.Convert<Guid?>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public DateTime? GetNullableDateTime(int metadataIndex)
        {
            var metadata = metadataList[metadataIndex];
            return this.reader.IsDBNull(metadata.DbFieldIndex) ? null :
                metadata.StoreDirect ? this.reader.GetDateTime(metadata.DbFieldIndex) :
                    this.cc.Convert<DateTime?>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public TEnum? GetNullableEnum<TEnum>(int metadataIndex)
            where TEnum : struct, Enum
        {
            var metadata = metadataList[metadataIndex];
            return this.reader.IsDBNull(metadata.DbFieldIndex) ? default(TEnum?) :
                metadata.StoreDirect ? (TEnum)this.reader.GetValue(metadata.DbFieldIndex) :
                    this.cc.Convert<TEnum?>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string? GetNullableString(int metadataIndex)
        {
            var metadata = metadataList[metadataIndex];
            return this.reader.IsDBNull(metadata.DbFieldIndex) ? null :
                metadata.StoreDirect ? this.reader.GetString(metadata.DbFieldIndex) :
                    this.cc.Convert<string?>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public byte[]? GetNullableBytes(int metadataIndex)
        {
            var metadata = metadataList[metadataIndex];
            return this.reader.IsDBNull(metadata.DbFieldIndex) ? null :
                metadata.StoreDirect ? (byte[])this.reader.GetValue(metadata.DbFieldIndex) :
                    this.cc.Convert<byte[]?>(this.reader.GetValue(metadata.DbFieldIndex));
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class DataInjectionContext<TElement> : DataInjectionContext
    {
        private InjectDelegate<TElement> injector = null!;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal DataInjectionContext(
            ConversionContext cc, IComparer<string> fieldComparer, DbDataReader reader) :
            base(cc, fieldComparer, reader)
        {
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void RegisterMetadata(
            KeyValuePair<string, Type>[] members, Delegate injector)
        {
            Debug.Assert(this.injector == null);    // TODO: combine multiple
            Debug.Assert(this.metadataList == null);

            this.injector = (InjectDelegate<TElement>)injector;

            var metadataMap =
                QueryHelper.CreateSortedMetadataMap(this.reader, this.fieldComparer);

            var candidates = new List<DataInjectionMetadata>(members.Length);
            for (var index = 0; index < members.Length; index++)
            {
                var member = members[index];
                var dbFieldNameIndiciesIndex =
                    Array.BinarySearch(metadataMap.FieldNames, member.Key, this.fieldComparer);
                if (dbFieldNameIndiciesIndex >= 0)
                {
                    var dbFieldMetadata = metadataMap.MetadataList[dbFieldNameIndiciesIndex];

                    var ut = Nullable.GetUnderlyingType(member.Value) ?? member.Value;
                    dbFieldMetadata.StoreDirect = ut == dbFieldMetadata.DbType;

                    candidates.Add(dbFieldMetadata);
                }
            }

            this.metadataList = candidates.ToArray();
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Inject(ref TElement element) =>
            this.injector(this, ref element);
    }
}
