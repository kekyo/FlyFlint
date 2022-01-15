﻿////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Internal.Converter;
using System;
using System.ComponentModel;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Text;

namespace FlyFlint.Internal.Static
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class StaticDataAccessor
    {
        private static readonly Encoding encoding = Encoding.UTF8;

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool GetBoolean(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? reader.GetBoolean(metadata.Index) :
                ValueConverter.UnsafeConvert<bool>(fp, encoding, reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static byte GetByte(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? reader.GetByte(metadata.Index) :
                ValueConverter.UnsafeConvert<byte>(fp, encoding, reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static short GetInt16(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? reader.GetInt16(metadata.Index) :
                ValueConverter.UnsafeConvert<short>(fp, encoding, reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static int GetInt32(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? reader.GetInt32(metadata.Index) :
                ValueConverter.UnsafeConvert<int>(fp, encoding, reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static long GetInt64(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? reader.GetInt64(metadata.Index) :
                ValueConverter.UnsafeConvert<long>(fp, encoding, reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static float GetSingle(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? reader.GetFloat(metadata.Index) :
                ValueConverter.UnsafeConvert<float>(fp, encoding, reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static double GetDouble(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? reader.GetDouble(metadata.Index) :
                ValueConverter.UnsafeConvert<double>(fp, encoding, reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static decimal GetDecimal(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? reader.GetDecimal(metadata.Index) :
                ValueConverter.UnsafeConvert<decimal>(fp, encoding, reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static char GetChar(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? reader.GetChar(metadata.Index) :
                ValueConverter.UnsafeConvert<char>(fp, encoding, reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Guid GetGuid(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? reader.GetGuid(metadata.Index) :
                ValueConverter.UnsafeConvert<Guid>(fp, encoding, reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static DateTime GetDateTime(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? reader.GetDateTime(metadata.Index) :
                ValueConverter.UnsafeConvert<DateTime>(fp, encoding, reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static TEnum GetEnum<TEnum>(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata)
            where TEnum : struct, Enum =>
            metadata.StoreDirect ? (TEnum)reader.GetValue(metadata.Index) :
                ValueConverter.UnsafeConvert<TEnum>(fp, encoding, reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static string GetString(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? reader.GetString(metadata.Index) :
                ValueConverter.UnsafeConvert<string>(fp, encoding, reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static byte[] GetBytes(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? (byte[])reader.GetValue(metadata.Index) :
                ValueConverter.UnsafeConvert<byte[]>(fp, encoding, reader.GetValue(metadata.Index));

        /////////////////////////////////////////////////////////////////////////////

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool? GetNullableBoolean(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? reader.GetBoolean(metadata.Index) :
                    ValueConverter.UnsafeConvert<bool?>(fp, encoding, reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static byte? GetNullableByte(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? reader.GetByte(metadata.Index) :
                    ValueConverter.UnsafeConvert<byte?>(fp, encoding, reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static short? GetNullableInt16(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? reader.GetInt16(metadata.Index) :
                    ValueConverter.UnsafeConvert<short?>(fp, encoding, reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static int? GetNullableInt32(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? reader.GetInt32(metadata.Index) :
                    ValueConverter.UnsafeConvert<int?>(fp, encoding, reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static long? GetNullableInt64(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? reader.GetInt64(metadata.Index) :
                    ValueConverter.UnsafeConvert<long?>(fp, encoding, reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static float? GetNullableSingle(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? reader.GetFloat(metadata.Index) :
                    ValueConverter.UnsafeConvert<float?>(fp, encoding, reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static double? GetNullableDouble(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? reader.GetDouble(metadata.Index) :
                    ValueConverter.UnsafeConvert<double?>(fp, encoding, reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static decimal? GetNullableDecimal(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? reader.GetDecimal(metadata.Index) :
                    ValueConverter.UnsafeConvert<decimal?>(fp, encoding, reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static char? GetNullableChar(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? reader.GetChar(metadata.Index) :
                    ValueConverter.UnsafeConvert<char?>(fp, encoding, reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Guid? GetNullableGuid(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? reader.GetGuid(metadata.Index) :
                    ValueConverter.UnsafeConvert<Guid?>(fp, encoding, reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static DateTime? GetNullableDateTime(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? reader.GetDateTime(metadata.Index) :
                    ValueConverter.UnsafeConvert<DateTime?>(fp, encoding, reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static TEnum? GetNullableEnum<TEnum>(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata)
            where TEnum : struct, Enum =>
            reader.IsDBNull(metadata.Index) ? default(TEnum?) :
                metadata.StoreDirect ? (TEnum)reader.GetValue(metadata.Index) :
                    ValueConverter.UnsafeConvert<TEnum?>(fp, encoding, reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static string? GetNullableString(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? reader.GetString(metadata.Index) :
                    ValueConverter.UnsafeConvert<string?>(fp, encoding, reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static byte[]? GetNullableBytes(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? (byte[])reader.GetValue(metadata.Index) :
                    ValueConverter.UnsafeConvert<byte[]?>(fp, encoding, reader.GetValue(metadata.Index));
    }
}
