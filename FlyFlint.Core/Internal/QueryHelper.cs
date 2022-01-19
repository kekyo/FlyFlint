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
#if NET40_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
using System.Threading.Tasks;
#endif

namespace FlyFlint.Internal
{
    internal static class QueryHelper
    {
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

        public static DbCommand CreateCommand(
            DbConnection connection, DbTransaction? transaction,
            string sql, KeyValuePair<string, object?>[] parameters)
        {
            var command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = sql;
            command.Transaction = transaction;

            var pc = command.Parameters;
            foreach (var parameter in parameters)
            {
                var p = command.CreateParameter();
                p.ParameterName = parameter.Key;
                p.Value = parameter.Value;
                pc.Add(p);
            }

            return command;
        }

        /////////////////////////////////////////////////////////////////////

        public static (string[], DataInjectionMetadata[]) CreateSortedMetadataMap(
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

            return (dbFieldNames, dbFieldMetadataList);
        }
    }
}
