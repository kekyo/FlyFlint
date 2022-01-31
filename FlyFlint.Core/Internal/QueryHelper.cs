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
using System.Linq;
using System.Runtime.CompilerServices;

namespace FlyFlint.Internal
{
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

        public static string GetFormattedSqlString(
            FormattableString fs, string parameterPrefix)
        {
            var args = fs.GetArguments();
            var formatArgs = new string[args.Length];
            for (var index = 0; index < formatArgs.Length; index++)
            {
                formatArgs[index] = parameterPrefix + "a" + index;
            }
            return string.Format(fs.Format, formatArgs);
        }

        public static ExtractedParameter[] GetSqlParameters(
            FormattableString fs, string parameterPrefix)
        {
            var args = fs.GetArguments();
            var parameters = new ExtractedParameter[args.Length];
            for (var index = 0; index < parameters.Length; index++)
            {
                parameters[index] = new ExtractedParameter(
                    parameterPrefix + "a" + index, args[index]);
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

        public struct MetadataMap
        {
            public readonly string[] FieldNames;
            public readonly DataInjectionMetadata[] MetadataList;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
            public MetadataMap(string[] fieldNames, DataInjectionMetadata[] metadataList)
            {
                this.FieldNames = fieldNames;
                this.MetadataList = metadataList;
            }
        }

        public static MetadataMap CreateSortedMetadataMap(
            DbDataReader reader, IComparer<string> fieldComparer)
        {
            var dbFieldCount = reader.FieldCount;
            var dbFieldNames = new string[dbFieldCount];
            var dbFieldMetadataList = new DataInjectionMetadata[dbFieldCount];

            for (var dbFieldIndex = 0; dbFieldIndex < dbFieldCount; dbFieldIndex++)
            {
                dbFieldNames[dbFieldIndex] =
                    reader.GetName(dbFieldIndex);
                dbFieldMetadataList[dbFieldIndex] =
                    new DataInjectionMetadata(dbFieldIndex, reader.GetFieldType(dbFieldIndex));
            }

            Array.Sort(dbFieldNames, dbFieldMetadataList, fieldComparer);

            return new MetadataMap(dbFieldNames, dbFieldMetadataList);
        }
    }
}
