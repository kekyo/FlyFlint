////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace FlyFlint.Internal.Static
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class StaticDataAccessor
    {
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool GetBoolean(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? reader.GetBoolean(metadata.Index) :
                Convert.ToBoolean(reader.GetValue(metadata.Index), fp);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static byte GetByte(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? reader.GetByte(metadata.Index) :
                Convert.ToByte(reader.GetValue(metadata.Index), fp);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static short GetInt16(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? reader.GetInt16(metadata.Index) :
                Convert.ToInt16(reader.GetValue(metadata.Index), fp);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static int GetInt32(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? reader.GetInt32(metadata.Index) :
                Convert.ToInt32(reader.GetValue(metadata.Index), fp);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static long GetInt64(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? reader.GetInt64(metadata.Index) :
                Convert.ToInt64(reader.GetValue(metadata.Index), fp);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static float GetSingle(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? reader.GetFloat(metadata.Index) :
                Convert.ToSingle(reader.GetValue(metadata.Index), fp);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static double GetDouble(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? reader.GetDouble(metadata.Index) :
                Convert.ToDouble(reader.GetValue(metadata.Index), fp);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static decimal GetDecimal(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? reader.GetDecimal(metadata.Index) :
                Convert.ToDecimal(reader.GetValue(metadata.Index), fp);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static char GetChar(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? reader.GetChar(metadata.Index) :
                Convert.ToChar(reader.GetValue(metadata.Index), fp);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Guid GetGuid(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? reader.GetGuid(metadata.Index) :
                new Guid(reader.GetValue(metadata.Index).ToString());

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static DateTime GetDateTime(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            metadata.StoreDirect ? reader.GetDateTime(metadata.Index) :
                Convert.ToDateTime(reader.GetValue(metadata.Index), fp);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static TEnum GetEnum<TEnum>(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata)
            where TEnum : struct, Enum =>
            metadata.StoreDirect ? (TEnum)reader.GetValue(metadata.Index) :
                GetEnumValue<TEnum>(reader.GetValue(metadata.Index), fp);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static string GetString(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            reader.IsDBNull(metadata.Index) ? throw new InvalidCastException("Couldn't assign null string.") :
                metadata.StoreDirect ? reader.GetString(metadata.Index) :
                    Convert.ToString(reader.GetValue(metadata.Index), fp);

        /////////////////////////////////////////////////////////////////////////////

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool? GetNullableBoolean(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? reader.GetBoolean(metadata.Index) :
                    Convert.ToBoolean(reader.GetValue(metadata.Index), fp);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static byte? GetNullableByte(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? reader.GetByte(metadata.Index) :
                    Convert.ToByte(reader.GetValue(metadata.Index), fp);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static short? GetNullableInt16(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? reader.GetInt16(metadata.Index) :
                    Convert.ToInt16(reader.GetValue(metadata.Index), fp);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static int? GetNullableInt32(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? reader.GetInt32(metadata.Index) :
                    Convert.ToInt32(reader.GetValue(metadata.Index), fp);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static long? GetNullableInt64(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? reader.GetInt64(metadata.Index) :
                    Convert.ToInt64(reader.GetValue(metadata.Index), fp);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static float? GetNullableSingle(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? reader.GetFloat(metadata.Index) :
                    Convert.ToSingle(reader.GetValue(metadata.Index), fp);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static double? GetNullableDouble(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? reader.GetDouble(metadata.Index) :
                    Convert.ToDouble(reader.GetValue(metadata.Index), fp);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static decimal? GetNullableDecimal(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? reader.GetDecimal(metadata.Index) :
                    Convert.ToDecimal(reader.GetValue(metadata.Index), fp);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static char? GetNullableChar(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? reader.GetChar(metadata.Index) :
                    Convert.ToChar(reader.GetValue(metadata.Index), fp);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Guid? GetNullableGuid(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? reader.GetGuid(metadata.Index) :
                    new Guid(reader.GetValue(metadata.Index).ToString());

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static DateTime? GetNullableDateTime(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? reader.GetDateTime(metadata.Index) :
                    Convert.ToDateTime(reader.GetValue(metadata.Index), fp);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static TEnum? GetNullableEnum<TEnum>(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata)
            where TEnum : struct, Enum =>
            reader.IsDBNull(metadata.Index) ? default(TEnum?) :
                metadata.StoreDirect ? (TEnum)reader.GetValue(metadata.Index) :
                    GetEnumValue<TEnum>(reader.GetValue(metadata.Index), fp);

        /////////////////////////////////////////////////////////////////////////////

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static string? GetNullableString(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata) =>
            reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? reader.GetString(metadata.Index) :
                    Convert.ToString(reader.GetValue(metadata.Index), fp);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static byte[]? GetBytes(IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata)
        {
            if (reader.IsDBNull(metadata.Index))
            {
                return null;
            }

            var value = reader.GetValue(metadata.Index);
            if (metadata.StoreDirect)
            {
                return (byte[]?)value;
            }
            else if (value is string str)
            {
                return Encoding.UTF8.GetBytes(str);
            }
            else if (value is char c)
            {
                return BitConverter.GetBytes(c);
            }
            else if (value is bool b)
            {
                return BitConverter.GetBytes(b);
            }
            else if (value is byte b8)
            {
                return BitConverter.GetBytes(b8);
            }
            else if (value is short i16)
            {
                return BitConverter.GetBytes(i16);
            }
            else if (value is int i32)
            {
                return BitConverter.GetBytes(i32);
            }
            else if (value is long i64)
            {
                return BitConverter.GetBytes(i64);
            }
            else if (value is float f)
            {
                return BitConverter.GetBytes(f);
            }
            else if (value is double d)
            {
                return BitConverter.GetBytes(d);
            }
            else
            {
                return Encoding.UTF8.GetBytes(value.ToString());
            }
        }

        /////////////////////////////////////////////////////////////////////////////

        private static class EnumMetadata<TEnum>
            where TEnum : struct, Enum
        {
            private static readonly (string[] fieldNames, TEnum[] fieldValues) fields;
            private static readonly Type underlyingType = Enum.GetUnderlyingType(typeof(TEnum));

            static EnumMetadata()
            {
                var (names, values) = QueryHelper.GetSortedEnumValues(typeof(TEnum));
                fields = (names, values.Cast<TEnum>().ToArray());
            }

            private static bool TryToEnumValue(string strValue, out TEnum? value)
            {
                var index = Array.BinarySearch(fields.fieldNames, strValue);
                if (index >= 1)
                {
                    value = fields.fieldValues[index];
                    return true;
                }
                else
                {
                    value = null;
                    return false;
                }
            }

            public static TEnum GetEnumValue(object value, IFormatProvider fp)
            {
                // Makes flexible enum type conversion.
                if (underlyingType == value.GetType())
                {
                    return (TEnum)value;
                }
                else if (value is string sv && EnumMetadata<TEnum>.TryToEnumValue(sv, out var ov))
                {
                    return (TEnum)ov!;
                }
                else
                {
                    return (TEnum)Convert.ChangeType(value, underlyingType, fp);
                }
            }
        }

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static TEnum GetEnumValue<TEnum>(object value, IFormatProvider fp)
            where TEnum : struct, Enum =>
            EnumMetadata<TEnum>.GetEnumValue(value, fp);
    }
}
