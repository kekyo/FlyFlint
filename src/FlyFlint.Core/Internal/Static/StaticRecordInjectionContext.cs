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
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace FlyFlint.Internal.Static
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public delegate void StaticRecordInjectorByRefDelegate<TRecord>(
        StaticRecordInjectionContext context, ref TRecord record)
        where TRecord : notnull;  // struct on the runtime

    [EditorBrowsable(EditorBrowsableState.Never)]
    public delegate void StaticRecordInjectorObjRefDelegate<TRecord>(
        StaticRecordInjectionContext context, TRecord record)
        where TRecord : notnull;  // class on the runtime

    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class StaticRecordInjectionContext :
        RecordInjectionContext
    {
        // HACK: Will contains null RecordInjectionMetadata.
        private protected RecordInjectionMetadata[] metadataList = null!;

        // Don't set outside of context.
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool[] IsAvailable = null!;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private protected StaticRecordInjectionContext(
            ConversionContext cc,
            IComparer<string> fieldComparer,
            DbDataReader reader) :
            base(cc, fieldComparer, reader)
        {
        }

        private protected void RegisterMemberMetadata(
            StaticMemberMetadata[] members)
        {
            Debug.Assert(this.metadataList == null);    // TODO: combine multiple

            var metadataMap =
                QueryHelper.CreateSortedMetadataMap(this.reader, this.fieldComparer);

            this.metadataList = new RecordInjectionMetadata[members.Length];
            this.IsAvailable = new bool[members.Length];
            for (var index = 0; index < members.Length; index++)
            {
                var member = members[index];
                var dbFieldNameIndiciesIndex =
                    Array.BinarySearch(metadataMap.FieldNames, member.Name, this.fieldComparer);
                if (dbFieldNameIndiciesIndex >= 0)
                {
                    var dbFieldMetadata = metadataMap.MetadataList[dbFieldNameIndiciesIndex];

                    var ut = Nullable.GetUnderlyingType(member.Type) ?? member.Type;
                    dbFieldMetadata.StoreDirect = ut == dbFieldMetadata.DbType;

                    this.metadataList[index] = dbFieldMetadata;
                    this.IsAvailable[index] = true;
                }
            }
        }

        public abstract void RegisterMetadata(
            StaticMemberMetadata[] members,
            Delegate injector);   // StaticInjectDelegate<TRecord>

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static object AssertNotDBNull<T>(object value) =>
            value is DBNull ?
                throw new NullReferenceException($"Could not convert from DBNull to {typeof(T).FullName}.") :
                value;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static T AssertNotNull<T>(T value) =>
            value == null ?
                throw new NullReferenceException($"Could not convert from Null to {typeof(T).FullName}.") :
                value;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool GetBoolean(int metadataIndex)
        {
            var metadata = this.metadataList[metadataIndex];
            return metadata.StoreDirect ? this.reader.GetBoolean(metadata.DbFieldIndex) :
                this.cc.ConvertTo<bool>(AssertNotDBNull<bool>(this.reader.GetValue(metadata.DbFieldIndex)));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public byte GetByte(int metadataIndex)
        {
            var metadata = this.metadataList[metadataIndex];
            return metadata.StoreDirect ? this.reader.GetByte(metadata.DbFieldIndex) :
                this.cc.ConvertTo<byte>(AssertNotDBNull<byte>(this.reader.GetValue(metadata.DbFieldIndex)));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public short GetInt16(int metadataIndex)
        {
            var metadata = this.metadataList[metadataIndex];
            return metadata.StoreDirect ? this.reader.GetInt16(metadata.DbFieldIndex) :
                this.cc.ConvertTo<short>(AssertNotDBNull<short>(this.reader.GetValue(metadata.DbFieldIndex)));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int GetInt32(int metadataIndex)
        {
            var metadata = this.metadataList[metadataIndex];
            return metadata.StoreDirect ? this.reader.GetInt32(metadata.DbFieldIndex) :
                this.cc.ConvertTo<int>(AssertNotDBNull<int>(this.reader.GetValue(metadata.DbFieldIndex)));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public long GetInt64(int metadataIndex)
        {
            var metadata = this.metadataList[metadataIndex];
            return metadata.StoreDirect ? this.reader.GetInt64(metadata.DbFieldIndex) :
                this.cc.ConvertTo<long>(AssertNotDBNull<long>(this.reader.GetValue(metadata.DbFieldIndex)));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public float GetSingle(int metadataIndex)
        {
            var metadata = this.metadataList[metadataIndex];
            return metadata.StoreDirect ? this.reader.GetFloat(metadata.DbFieldIndex) :
                this.cc.ConvertTo<float>(AssertNotDBNull<float>(this.reader.GetValue(metadata.DbFieldIndex)));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public double GetDouble(int metadataIndex)
        {
            var metadata = this.metadataList[metadataIndex];
            return metadata.StoreDirect ? this.reader.GetDouble(metadata.DbFieldIndex) :
                this.cc.ConvertTo<double>(AssertNotDBNull<double>(this.reader.GetValue(metadata.DbFieldIndex)));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public decimal GetDecimal(int metadataIndex)
        {
            var metadata = this.metadataList[metadataIndex];
            return metadata.StoreDirect ? this.reader.GetDecimal(metadata.DbFieldIndex) :
                this.cc.ConvertTo<decimal>(AssertNotDBNull<decimal>(this.reader.GetValue(metadata.DbFieldIndex)));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public char GetChar(int metadataIndex)
        {
            var metadata = this.metadataList[metadataIndex];
            return metadata.StoreDirect ? this.reader.GetChar(metadata.DbFieldIndex) :
                this.cc.ConvertTo<char>(AssertNotDBNull<char>(this.reader.GetValue(metadata.DbFieldIndex)));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Guid GetGuid(int metadataIndex)
        {
            var metadata = this.metadataList[metadataIndex];
            return metadata.StoreDirect ? this.reader.GetGuid(metadata.DbFieldIndex) :
                this.cc.ConvertTo<Guid>(AssertNotDBNull<Guid>(this.reader.GetValue(metadata.DbFieldIndex)));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public DateTime GetDateTime(int metadataIndex)
        {
            var metadata = this.metadataList[metadataIndex];
            return metadata.StoreDirect ? this.reader.GetDateTime(metadata.DbFieldIndex) :
                this.cc.ConvertTo<DateTime>(AssertNotDBNull<DateTime>(this.reader.GetValue(metadata.DbFieldIndex)));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string GetString(int metadataIndex)
        {
            var metadata = this.metadataList[metadataIndex];
            return metadata.StoreDirect ? this.reader.GetString(metadata.DbFieldIndex) :
                AssertNotNull(this.cc.ConvertTo<string>(AssertNotDBNull<string>(this.reader.GetValue(metadata.DbFieldIndex))));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public byte[] GetBytes(int metadataIndex)
        {
            var metadata = this.metadataList[metadataIndex];
            return metadata.StoreDirect ? (byte[])this.reader.GetValue(metadata.DbFieldIndex) :
                AssertNotNull(this.cc.ConvertTo<byte[]>(AssertNotDBNull<byte[]>(this.reader.GetValue(metadata.DbFieldIndex))));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public TValue GetValue<TValue>(int metadataIndex)
            where TValue : struct
        {
            var metadata = this.metadataList[metadataIndex];
            return metadata.StoreDirect ? (TValue)this.reader.GetValue(metadata.DbFieldIndex) :
                this.cc.ConvertTo<TValue>(AssertNotDBNull<TValue>(this.reader.GetValue(metadata.DbFieldIndex)));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public TValue GetObjRefValue<TValue>(int metadataIndex)
            where TValue : class
        {
            var metadata = this.metadataList[metadataIndex];
            return metadata.StoreDirect ? (TValue)this.reader.GetValue(metadata.DbFieldIndex) :
                AssertNotNull(this.cc.ConvertTo<TValue>(AssertNotDBNull<TValue>(this.reader.GetValue(metadata.DbFieldIndex))));
        }

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool? GetNullableBoolean(int metadataIndex)
        {
            var metadata = this.metadataList[metadataIndex];
            return this.reader.IsDBNull(metadata.DbFieldIndex) ? null :
                metadata.StoreDirect ? this.reader.GetBoolean(metadata.DbFieldIndex) :
                    this.cc.ConvertTo<bool?>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public byte? GetNullableByte(int metadataIndex)
        {
            var metadata = this.metadataList[metadataIndex];
            return this.reader.IsDBNull(metadata.DbFieldIndex) ? null :
                metadata.StoreDirect ? this.reader.GetByte(metadata.DbFieldIndex) :
                    this.cc.ConvertTo<byte?>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public short? GetNullableInt16(int metadataIndex)
        {
            var metadata = this.metadataList[metadataIndex];
            return this.reader.IsDBNull(metadata.DbFieldIndex) ? null :
                metadata.StoreDirect ? this.reader.GetInt16(metadata.DbFieldIndex) :
                    this.cc.ConvertTo<short?>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int? GetNullableInt32(int metadataIndex)
        {
            var metadata = this.metadataList[metadataIndex];
            return this.reader.IsDBNull(metadata.DbFieldIndex) ? null :
                metadata.StoreDirect ? this.reader.GetInt32(metadata.DbFieldIndex) :
                    this.cc.ConvertTo<int?>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public long? GetNullableInt64(int metadataIndex)
        {
            var metadata = this.metadataList[metadataIndex];
            return this.reader.IsDBNull(metadata.DbFieldIndex) ? null :
                metadata.StoreDirect ? this.reader.GetInt64(metadata.DbFieldIndex) :
                    this.cc.ConvertTo<long?>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public float? GetNullableSingle(int metadataIndex)
        {
            var metadata = this.metadataList[metadataIndex];
            return this.reader.IsDBNull(metadata.DbFieldIndex) ? null :
                metadata.StoreDirect ? this.reader.GetFloat(metadata.DbFieldIndex) :
                    this.cc.ConvertTo<float?>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public double? GetNullableDouble(int metadataIndex)
        {
            var metadata = this.metadataList[metadataIndex];
            return this.reader.IsDBNull(metadata.DbFieldIndex) ? null :
                metadata.StoreDirect ? this.reader.GetDouble(metadata.DbFieldIndex) :
                    this.cc.ConvertTo<double?>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public decimal? GetNullableDecimal(int metadataIndex)
        {
            var metadata = this.metadataList[metadataIndex];
            return this.reader.IsDBNull(metadata.DbFieldIndex) ? null :
                metadata.StoreDirect ? this.reader.GetDecimal(metadata.DbFieldIndex) :
                    this.cc.ConvertTo<decimal?>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public char? GetNullableChar(int metadataIndex)
        {
            var metadata = this.metadataList[metadataIndex];
            return this.reader.IsDBNull(metadata.DbFieldIndex) ? null :
                metadata.StoreDirect ? this.reader.GetChar(metadata.DbFieldIndex) :
                    this.cc.ConvertTo<char?>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Guid? GetNullableGuid(int metadataIndex)
        {
            var metadata = this.metadataList[metadataIndex];
            return this.reader.IsDBNull(metadata.DbFieldIndex) ? null :
                metadata.StoreDirect ? this.reader.GetGuid(metadata.DbFieldIndex) :
                    this.cc.ConvertTo<Guid?>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public DateTime? GetNullableDateTime(int metadataIndex)
        {
            var metadata = this.metadataList[metadataIndex];
            return this.reader.IsDBNull(metadata.DbFieldIndex) ? null :
                metadata.StoreDirect ? this.reader.GetDateTime(metadata.DbFieldIndex) :
                    this.cc.ConvertTo<DateTime?>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string? GetNullableString(int metadataIndex)
        {
            var metadata = this.metadataList[metadataIndex];
            return this.reader.IsDBNull(metadata.DbFieldIndex) ? null :
                metadata.StoreDirect ? this.reader.GetString(metadata.DbFieldIndex) :
                    this.cc.ConvertTo<string?>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public byte[]? GetNullableBytes(int metadataIndex)
        {
            var metadata = this.metadataList[metadataIndex];
            return this.reader.IsDBNull(metadata.DbFieldIndex) ? null :
                metadata.StoreDirect ? (byte[])this.reader.GetValue(metadata.DbFieldIndex) :
                    this.cc.ConvertTo<byte[]?>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public TValue? GetNullableValue<TValue>(int metadataIndex)
            where TValue : struct
        {
            var metadata = this.metadataList[metadataIndex];
            return this.reader.IsDBNull(metadata.DbFieldIndex) ? null :
                metadata.StoreDirect ? (TValue)this.reader.GetValue(metadata.DbFieldIndex) :
                    this.cc.ConvertTo<TValue?>(this.reader.GetValue(metadata.DbFieldIndex));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public TValue? GetNullableObjRefValue<TValue>(int metadataIndex)
            where TValue : class
        {
            var metadata = this.metadataList[metadataIndex];
            return this.reader.IsDBNull(metadata.DbFieldIndex) ? null :
                metadata.StoreDirect ? (TValue)this.reader.GetValue(metadata.DbFieldIndex) :
                    this.cc.ConvertTo<TValue?>(this.reader.GetValue(metadata.DbFieldIndex));
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class StaticRecordInjectionContext<TRecord> :
        StaticRecordInjectionContext
        where TRecord : notnull
    {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private protected StaticRecordInjectionContext(
            ConversionContext cc,
            IComparer<string> fieldComparer,
            DbDataReader reader) :
            base(cc, fieldComparer, reader)
        {
        }

        private protected bool made;  // TODO:

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void MakeInjectable()
        {
            // TODO:
            this.made = true;
        }

        public abstract void Inject(ref TRecord record);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class StaticRecordInjectionByRefContext<TRecord> :
        StaticRecordInjectionContext<TRecord>
        where TRecord : notnull
    {
        private StaticRecordInjectorByRefDelegate<TRecord> injector = null!;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal StaticRecordInjectionByRefContext(
            ConversionContext cc,
            IComparer<string> fieldComparer,
            DbDataReader reader) :
            base(cc, fieldComparer, reader)
        {
            Debug.Assert(typeof(TRecord).IsValueType);
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void RegisterMetadata(
            StaticMemberMetadata[] members,
            Delegate injector)   // StaticRecordInjectorByRefDelegate<TRecord>
        {
            Debug.Assert(this.injector == null);    // TODO: combine multiple

            this.injector = (StaticRecordInjectorByRefDelegate<TRecord>)injector;
            this.RegisterMemberMetadata(members);
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override void Inject(ref TRecord record)
        {
            Debug.Assert(this.made);
            this.injector(this, ref record);
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class StaticRecordInjectionObjRefContext<TRecord> :
        StaticRecordInjectionContext<TRecord>
        where TRecord : notnull
    {
        private StaticRecordInjectorObjRefDelegate<TRecord> injector = null!;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal StaticRecordInjectionObjRefContext(
            ConversionContext cc,
            IComparer<string> fieldComparer,
            DbDataReader reader) :
            base(cc, fieldComparer, reader)
        {
            Debug.Assert(!typeof(TRecord).IsValueType);
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void RegisterMetadata(
            StaticMemberMetadata[] members,
            Delegate injector)   // StaticRecordInjectorObjRefDelegate<TRecord>
        {
            Debug.Assert(this.injector == null);    // TODO: combine multiple

            this.injector = (StaticRecordInjectorObjRefDelegate<TRecord>)injector;
            this.RegisterMemberMetadata(members);
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override void Inject(ref TRecord record)
        {
            Debug.Assert(this.made);
            this.injector(this, record);
        }
    }
}
