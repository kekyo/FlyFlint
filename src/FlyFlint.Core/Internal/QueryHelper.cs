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
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

#pragma warning disable CS8618

namespace FlyFlint.Internal
{
    internal sealed class FormatterParameter : IFormattable
    {
        private readonly string placeholder;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public FormatterParameter(string placeholder) =>
            this.placeholder = placeholder;

        public string? FormatParameter
        {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
            get;
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
            private set;
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override string ToString() =>
            this.placeholder;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            this.FormatParameter = format;
            return this.placeholder;
        }
    }

    internal static class QueryHelper
    {
        public static readonly ExtractedParameter[] DefaultParameters = { };

        public static readonly Trait DefaultTrait =
            new Trait(ConversionContext.Default, StringComparer.OrdinalIgnoreCase, "@");

        public static Trait CurrentDefaultTrait =
            DefaultTrait;

        /////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool IsNullOrWhiteSpace(string? str) =>
#if NET35
            string.IsNullOrEmpty(str) || str.All(char.IsWhiteSpace);
#else
            string.IsNullOrWhiteSpace(str);
#endif

        /////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal static IEnumerable<T> Traverse<T>(
            this T value, Func<T, T?> selector)
        {
            T? current = value;
            while (current != null)
            {
                yield return current;
                current = selector(current);
            }
        }

        internal static IEnumerable<T> DistinctBy<T, U>(
            this IEnumerable<T> enumerable, Func<T, U> keySelector)
        {
            var memoized = new HashSet<U>();
            foreach (var value in enumerable)
            {
                if (memoized.Add(keySelector(value)))
                {
                    yield return value;
                }
            }
        }

        internal static IEnumerable<U> Collect<T, U>(
            this IEnumerable<T> enumerable, Func<T, U?> selector)
        {
            foreach (var value in enumerable)
            {
                if (selector(value) is U sv)
                {
                    yield return sv;
                }
            }
        }

        /////////////////////////////////////////////////////////////////////

        public static KeyValuePair<string, string?[]> GetFormattedSqlString(
            FormattableString fs,
            string parameterPrefix)
        {
            var args = fs.GetArguments();
            var formatArgs = new FormatterParameter[args.Length];
            for (var index = 0; index < formatArgs.Length; index++)
            {
                formatArgs[index] = new FormatterParameter(parameterPrefix + "a" + index);
            }

            var formatted = string.Format(fs.Format, formatArgs);

            var formatParameters = new string?[formatArgs.Length];
            for (var index = 0; index < formatParameters.Length; index++)
            {
                formatParameters[index] = formatArgs[index].FormatParameter;
            }

            return new KeyValuePair<string, string?[]>(formatted, formatParameters);
        }

        public static ExtractedParameter[] GetSqlParameters(
            FormattableString fs,
            string?[] formatParameters,
            string parameterPrefix,
            ConversionContext cc)
        {
            var args = fs.GetArguments();
            var parameters = new ExtractedParameter[args.Length];
            for (var index = 0; index < parameters.Length; index++)
            {
                var formatParameter = formatParameters[index];
                var argument = args[index];

                parameters[index] = new ExtractedParameter(
                    parameterPrefix + "a" + index,
                    cc.ConvertFromByVal(argument, formatParameter));
            }
            return parameters;
        }

        /////////////////////////////////////////////////////////////////////

        public static DbCommand CreateCommand(
            DbConnection connection, DbTransaction? transaction,
            string sql, ExtractedParameter[] parameters)
        {
            var command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = sql;
            command.Transaction = transaction;

            var pc = command.Parameters;
            foreach (var parameter in parameters)
            {
                var p = command.CreateParameter();
                p.ParameterName = parameter.Name;
                p.Value = parameter.Value;
                pc.Add(p);
            }

            return command;
        }

        /////////////////////////////////////////////////////////////////////

        public struct MetadataMap : IEquatable<MetadataMap>
        {
            public readonly string[] FieldNames;
            public readonly RecordInjectionMetadata[] MetadataList;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
            public MetadataMap(string[] fieldNames, RecordInjectionMetadata[] metadataList)
            {
                this.FieldNames = fieldNames;
                this.MetadataList = metadataList;
            }

            public override int GetHashCode()
            {
                var agg = 0;
                for (var index = 0; index < this.FieldNames.Length; index++)
                {
                    agg ^= this.FieldNames[index].GetHashCode();
                }
                for (var index = 0; index < this.MetadataList.Length; index++)
                {
                    agg ^= this.MetadataList[index].GetHashCode();
                }
                return agg;
            }

            public bool Equals(MetadataMap other)
            {
                if (this.FieldNames.Length != other.FieldNames.Length)
                {
                    return false;
                }
                if (this.MetadataList.Length != other.MetadataList.Length)
                {
                    return false;
                }
                for (var index = 0; index < this.FieldNames.Length; index++)
                {
                    if (!this.FieldNames[index].Equals(other.FieldNames[index]))
                    {
                        return false;
                    }
                }
                for (var index = 0; index < this.MetadataList.Length; index++)
                {
                    if (!this.MetadataList[index].Equals(other.MetadataList[index]))
                    {
                        return false;
                    }
                }
                return true;
            }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
            public override bool Equals(object? obj) =>
                obj is MetadataMap mm && this.Equals(mm);
        }

        public static MetadataMap CreateSortedMetadataMap(
            DbDataReader reader, IComparer<string> fieldComparer)
        {
            var dbFieldCount = reader.FieldCount;
            var dbFieldNames = new string[dbFieldCount];
            var dbFieldMetadataList = new RecordInjectionMetadata[dbFieldCount];

            for (var dbFieldIndex = 0; dbFieldIndex < dbFieldCount; dbFieldIndex++)
            {
                dbFieldNames[dbFieldIndex] =
                    reader.GetName(dbFieldIndex);
                dbFieldMetadataList[dbFieldIndex] =
                    new RecordInjectionMetadata(dbFieldIndex, reader.GetFieldType(dbFieldIndex));
            }

            Array.Sort(dbFieldNames, dbFieldMetadataList, fieldComparer);

            return new MetadataMap(dbFieldNames, dbFieldMetadataList);
        }
    }
}
