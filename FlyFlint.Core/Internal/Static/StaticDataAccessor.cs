////////////////////////////////////////////////////////////////////////////
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
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace FlyFlint.Internal.Static
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class StaticDataAccessor
    {
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static T UnsafeConvert<T>(DataInjectionContext context, object value)
        {
            Debug.Assert(value != null);
            Debug.Assert(value is not DBNull);
            return InternalValueConverter<T>.converter.UnsafeConvert(context, value!);
        }

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool GetBoolean(DataInjectionContext context, DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? context.reader.GetBoolean(metadata.Index) :
                UnsafeConvert<bool>(context, context.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static byte GetByte(DataInjectionContext context, DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? context.reader.GetByte(metadata.Index) :
                UnsafeConvert<byte>(context, context.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static short GetInt16(DataInjectionContext context, DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? context.reader.GetInt16(metadata.Index) :
                UnsafeConvert<short>(context, context.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static int GetInt32(DataInjectionContext context, DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? context.reader.GetInt32(metadata.Index) :
                UnsafeConvert<int>(context, context.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static long GetInt64(DataInjectionContext context, DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? context.reader.GetInt64(metadata.Index) :
                UnsafeConvert<long>(context, context.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static float GetSingle(DataInjectionContext context, DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? context.reader.GetFloat(metadata.Index) :
                UnsafeConvert<float>(context, context.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static double GetDouble(DataInjectionContext context, DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? context.reader.GetDouble(metadata.Index) :
                UnsafeConvert<double>(context, context.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static decimal GetDecimal(DataInjectionContext context, DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? context.reader.GetDecimal(metadata.Index) :
                UnsafeConvert<decimal>(context, context.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static char GetChar(DataInjectionContext context, DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? context.reader.GetChar(metadata.Index) :
                UnsafeConvert<char>(context, context.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Guid GetGuid(DataInjectionContext context, DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? context.reader.GetGuid(metadata.Index) :
                UnsafeConvert<Guid>(context, context.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static DateTime GetDateTime(DataInjectionContext context, DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? context.reader.GetDateTime(metadata.Index) :
                UnsafeConvert<DateTime>(context, context.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static TEnum GetEnum<TEnum>(DataInjectionContext context, DataInjectionMetadata metadata)
            where TEnum : struct, Enum =>
            metadata.StoreDirect ? (TEnum)context.reader.GetValue(metadata.Index) :
                UnsafeConvert<TEnum>(context, context.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static string GetString(DataInjectionContext context, DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? context.reader.GetString(metadata.Index) :
                UnsafeConvert<string>(context, context.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static byte[] GetBytes(DataInjectionContext context, DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? (byte[])context.reader.GetValue(metadata.Index) :
                UnsafeConvert<byte[]>(context, context.reader.GetValue(metadata.Index));

        /////////////////////////////////////////////////////////////////////////////

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool? GetNullableBoolean(DataInjectionContext context, DataInjectionMetadata metadata) =>
            context.reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? context.reader.GetBoolean(metadata.Index) :
                    UnsafeConvert<bool?>(context, context.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static byte? GetNullableByte(DataInjectionContext context, DataInjectionMetadata metadata) =>
            context.reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? context.reader.GetByte(metadata.Index) :
                    UnsafeConvert<byte?>(context, context.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static short? GetNullableInt16(DataInjectionContext context, DataInjectionMetadata metadata) =>
            context.reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? context.reader.GetInt16(metadata.Index) :
                    UnsafeConvert<short?>(context, context.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static int? GetNullableInt32(DataInjectionContext context, DataInjectionMetadata metadata) =>
            context.reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? context.reader.GetInt32(metadata.Index) :
                    UnsafeConvert<int?>(context, context.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static long? GetNullableInt64(DataInjectionContext context, DataInjectionMetadata metadata) =>
            context.reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? context.reader.GetInt64(metadata.Index) :
                    UnsafeConvert<long?>(context, context.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static float? GetNullableSingle(DataInjectionContext context, DataInjectionMetadata metadata) =>
            context.reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? context.reader.GetFloat(metadata.Index) :
                    UnsafeConvert<float?>(context, context.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static double? GetNullableDouble(DataInjectionContext context, DataInjectionMetadata metadata) =>
            context.reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? context.reader.GetDouble(metadata.Index) :
                    UnsafeConvert<double?>(context, context.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static decimal? GetNullableDecimal(DataInjectionContext context, DataInjectionMetadata metadata) =>
            context.reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? context.reader.GetDecimal(metadata.Index) :
                    UnsafeConvert<decimal?>(context, context.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static char? GetNullableChar(DataInjectionContext context, DataInjectionMetadata metadata) =>
            context.reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? context.reader.GetChar(metadata.Index) :
                    UnsafeConvert<char?>(context, context.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Guid? GetNullableGuid(DataInjectionContext context, DataInjectionMetadata metadata) =>
            context.reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? context.reader.GetGuid(metadata.Index) :
                    UnsafeConvert<Guid?>(context, context.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static DateTime? GetNullableDateTime(DataInjectionContext context, DataInjectionMetadata metadata) =>
            context.reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? context.reader.GetDateTime(metadata.Index) :
                    UnsafeConvert<DateTime?>(context, context.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static TEnum? GetNullableEnum<TEnum>(DataInjectionContext context, DataInjectionMetadata metadata)
            where TEnum : struct, Enum =>
            context.reader.IsDBNull(metadata.Index) ? default(TEnum?) :
                metadata.StoreDirect ? (TEnum)context.reader.GetValue(metadata.Index) :
                    UnsafeConvert<TEnum?>(context, context.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static string? GetNullableString(DataInjectionContext context, DataInjectionMetadata metadata) =>
            context.reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? context.reader.GetString(metadata.Index) :
                    UnsafeConvert<string?>(context, context.reader.GetValue(metadata.Index));

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static byte[]? GetNullableBytes(DataInjectionContext context, DataInjectionMetadata metadata) =>
            context.reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? (byte[])context.reader.GetValue(metadata.Index) :
                    UnsafeConvert<byte[]?>(context, context.reader.GetValue(metadata.Index));
    }
}
