////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace FlyFlint.Internal
{
    internal static class QueryHelper
    {
        public static DbCommand CreateCommand(
            DbConnection connection, DbTransaction? transaction,
            string sql, (string name, object? value)[] parameters)
        {
            var command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = sql;
            command.Transaction = transaction;

            var pc = command.Parameters;
            foreach (var parameter in parameters)
            {
                var p = command.CreateParameter();
                p.ParameterName = parameter.name;
                p.Value = parameter.value;
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
        
        /////////////////////////////////////////////////////////////////////

#if NET40
        public static Task<int> ExecuteNonQueryAsync(this DbCommand command) =>
            Task.Factory.StartNew(command.ExecuteNonQuery);
        public static Task<object?> ExecuteScalarAsync(this DbCommand command) =>
            Task.Factory.StartNew(command.ExecuteScalar);
#endif
    }
}
