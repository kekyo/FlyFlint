////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace FlyFlint.Context
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class DataInjectionContext
    {
        internal readonly ConversionContext cc;
        internal readonly DbDataReader reader;

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal DataInjectionContext(
            ConversionContext cc, DbDataReader reader)
        {
            this.cc = cc;
            this.reader = reader;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public DataInjectionMetadata[] Prepare(
            (string name, Type type)[] members)
        {
            var (dbFieldNames, dbFieldMetadataList) =
                QueryHelper.GetSortedMetadataMap(this.reader);

            var candidates = new List<DataInjectionMetadata>(members.Length);
            for (var index = 0; index < members.Length; index++)
            {
                var member = members[index];
                var dbFieldNameIndexesIndex = Array.BinarySearch(dbFieldNames, member.name);
                if (dbFieldNameIndexesIndex >= 0)
                {
                    var dbFieldMetadata = dbFieldMetadataList[dbFieldNameIndexesIndex];

                    var ut = Nullable.GetUnderlyingType(member.type) ?? member.type;
                    dbFieldMetadata.StoreDirect = ut == dbFieldMetadata.Type;

                    candidates.Add(dbFieldMetadata);
                }
            }

            return candidates.ToArray();
        }

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool GetBoolean(DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? this.reader.GetBoolean(metadata.Index) :
                this.cc.Convert<bool>(this.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public byte GetByte(DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? this.reader.GetByte(metadata.Index) :
                this.cc.Convert<byte>(this.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public short GetInt16(DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? this.reader.GetInt16(metadata.Index) :
                this.cc.Convert<short>(this.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int GetInt32(DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? this.reader.GetInt32(metadata.Index) :
                this.cc.Convert<int>(this.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public long GetInt64(DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? this.reader.GetInt64(metadata.Index) :
                this.cc.Convert<long>(this.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public float GetSingle(DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? this.reader.GetFloat(metadata.Index) :
                this.cc.Convert<float>(this.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public double GetDouble(DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? this.reader.GetDouble(metadata.Index) :
                this.cc.Convert<double>(this.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public decimal GetDecimal(DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? this.reader.GetDecimal(metadata.Index) :
                this.cc.Convert<decimal>(this.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public char GetChar(DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? this.reader.GetChar(metadata.Index) :
                this.cc.Convert<char>(this.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Guid GetGuid(DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? this.reader.GetGuid(metadata.Index) :
                this.cc.Convert<Guid>(this.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public DateTime GetDateTime(DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? this.reader.GetDateTime(metadata.Index) :
                this.cc.Convert<DateTime>(this.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public TEnum GetEnum<TEnum>(DataInjectionMetadata metadata)
            where TEnum : struct, Enum =>
            metadata.StoreDirect ? (TEnum)this.reader.GetValue(metadata.Index) :
                this.cc.Convert<TEnum>(this.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string GetString(DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? this.reader.GetString(metadata.Index) :
                this.cc.Convert<string>(this.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public byte[] GetBytes(DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? (byte[])this.reader.GetValue(metadata.Index) :
                this.cc.Convert<byte[]>(this.reader.GetValue(metadata.Index));

        /////////////////////////////////////////////////////////////////////////////

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool? GetNullableBoolean(DataInjectionMetadata metadata) =>
            this.reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? this.reader.GetBoolean(metadata.Index) :
                    this.cc.Convert<bool?>(this.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public byte? GetNullableByte(DataInjectionMetadata metadata) =>
            this.reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? this.reader.GetByte(metadata.Index) :
                    this.cc.Convert<byte?>(this.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public short? GetNullableInt16(DataInjectionMetadata metadata) =>
            this.reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? this.reader.GetInt16(metadata.Index) :
                    this.cc.Convert<short?>(this.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int? GetNullableInt32(DataInjectionMetadata metadata) =>
            this.reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? this.reader.GetInt32(metadata.Index) :
                    this.cc.Convert<int?>(this.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public long? GetNullableInt64(DataInjectionMetadata metadata) =>
            this.reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? this.reader.GetInt64(metadata.Index) :
                    this.cc.Convert<long?>(this.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public float? GetNullableSingle(DataInjectionMetadata metadata) =>
            this.reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? this.reader.GetFloat(metadata.Index) :
                    this.cc.Convert<float?>(this.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public double? GetNullableDouble(DataInjectionMetadata metadata) =>
            this.reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? this.reader.GetDouble(metadata.Index) :
                    this.cc.Convert<double?>(this.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public decimal? GetNullableDecimal(DataInjectionMetadata metadata) =>
            this.reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? this.reader.GetDecimal(metadata.Index) :
                    this.cc.Convert<decimal?>(this.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public char? GetNullableChar(DataInjectionMetadata metadata) =>
            this.reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? this.reader.GetChar(metadata.Index) :
                    this.cc.Convert<char?>(this.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Guid? GetNullableGuid(DataInjectionMetadata metadata) =>
            this.reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? this.reader.GetGuid(metadata.Index) :
                    this.cc.Convert<Guid?>(this.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public DateTime? GetNullableDateTime(DataInjectionMetadata metadata) =>
            this.reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? this.reader.GetDateTime(metadata.Index) :
                    this.cc.Convert<DateTime?>(this.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public TEnum? GetNullableEnum<TEnum>(DataInjectionMetadata metadata)
            where TEnum : struct, Enum =>
            this.reader.IsDBNull(metadata.Index) ? default(TEnum?) :
                metadata.StoreDirect ? (TEnum)this.reader.GetValue(metadata.Index) :
                    this.cc.Convert<TEnum?>(this.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string? GetNullableString(DataInjectionMetadata metadata) =>
            this.reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? this.reader.GetString(metadata.Index) :
                    this.cc.Convert<string?>(this.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public byte[]? GetNullableBytes(DataInjectionMetadata metadata) =>
            this.reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? (byte[])this.reader.GetValue(metadata.Index) :
                    this.cc.Convert<byte[]?>(this.reader.GetValue(metadata.Index));

        /////////////////////////////////////////////////////////////////////////////

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object? GetValue(DataInjectionMetadata metadata, Type targetType) =>
            this.reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? this.reader.GetValue(metadata.Index) :
                    this.cc.Convert(this.reader.GetValue(metadata.Index), targetType);
    }
}
