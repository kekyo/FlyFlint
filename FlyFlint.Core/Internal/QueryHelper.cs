////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace FlyFlint.Internal
{
    internal static class QueryHelper
    {
        public static readonly (string, object?)[] Empty = { };

        /////////////////////////////////////////////////////////////////////

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

        public static (string[], DataInjectionMetadata[]) GetSortedMetadataMap(DbDataReader reader)
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

            Array.Sort(dbFieldNames, dbFieldMetadataList);

            return (dbFieldNames, dbFieldMetadataList);
        }

        /////////////////////////////////////////////////////////////////////

        public static (string[] fieldNames, Enum[] fieldValues) GetSortedEnumValues(Type enumType)
        {
            var fieldNames = Enum.GetNames(enumType);
            var fieldValues = Enum.GetValues(enumType).Cast<Enum>().ToArray();   // TODO: slow

            Array.Sort(fieldNames, fieldValues);

            return (fieldNames, fieldValues);
        }
    }
}
