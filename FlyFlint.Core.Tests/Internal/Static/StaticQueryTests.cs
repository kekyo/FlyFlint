////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using NUnit.Framework;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static VerifyNUnit.Verifier;

namespace FlyFlint.Internal.Static
{
    public sealed class StaticQueryTests
    {
        private sealed class Target : IDataInjectable
        {
            public int Id;
            public string? Name;
            public DateTime Birth;

            private static readonly (string, Type)[] members = new[]
            {
                (nameof(Id), typeof(int)),
                (nameof(Name), typeof(string)),
                (nameof(Birth), typeof(DateTime)),
            };

            public DataInjectionMetadata[] PrepareAndInject(IFormatProvider fp, DbDataReader reader)
            {
                var metadataList = StaticInjectonHelper<Target>.Prepare(reader, members);
                this.Inject(fp, metadataList, reader);
                return metadataList;
            }

            public void Inject(IFormatProvider fp, DataInjectionMetadata[] metadataList, DbDataReader reader)
            {
                this.Id = StaticDataAccessor.GetInt32(fp, reader, metadataList[0]);
                this.Name = StaticDataAccessor.GetString(fp, reader, metadataList[1]);
                this.Birth = StaticDataAccessor.GetDateTime(fp, reader, metadataList[2]);
            }
        }

        [Test]
        public async Task Query()
        {
            using var connection = new SQLiteConnection("Data Source=:memory:");
            await connection.OpenAsync();

            var c = connection.CreateCommand();
            c.CommandType = CommandType.Text;
            c.CommandText = "CREATE TABLE target (Id INTEGER PRIMARY KEY,Name TEXT,Birth TEXT)";
            await c.ExecuteNonQueryAsync();

            c.CommandText = "INSERT INTO target VALUES (1,'AAAAA','2022/01/23 12:34:56.789')";
            await c.ExecuteNonQueryAsync();
            c.CommandText = "INSERT INTO target VALUES (2,'BBBBB','2022/01/23 12:34:57.789')";
            await c.ExecuteNonQueryAsync();
            c.CommandText = "INSERT INTO target VALUES (3,'CCCCC','2022/01/23 12:34:58.789')";
            await c.ExecuteNonQueryAsync();

            var qc = QueryExtension.Query<Target>(connection, "SELECT * FROM target");
            var targets = StaticQueryFacade.Execute(qc).ToArray();

            await Verify(targets.Select(element => $"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }

        [Test]
        public async Task QueryWithParameter()
        {
            using var connection = new SQLiteConnection("Data Source=:memory:");
            await connection.OpenAsync();

            var c = connection.CreateCommand();
            c.CommandType = CommandType.Text;
            c.CommandText = "CREATE TABLE target (Id INTEGER PRIMARY KEY,Name TEXT,Birth TEXT)";
            await c.ExecuteNonQueryAsync();

            c.CommandText = "INSERT INTO target VALUES (1,'AAAAA','2022/01/23 12:34:56.789')";
            await c.ExecuteNonQueryAsync();
            c.CommandText = "INSERT INTO target VALUES (2,'BBBBB','2022/01/23 12:34:57.789')";
            await c.ExecuteNonQueryAsync();
            c.CommandText = "INSERT INTO target VALUES (3,'CCCCC','2022/01/23 12:34:58.789')";
            await c.ExecuteNonQueryAsync();

            var query = QueryExtension.Query<Target>(
                connection, "SELECT * FROM target WHERE Id = @idparam").
                Parameter(new { idparam = 2 });
            var targets = StaticQueryFacade.Execute(query).ToArray();

            await Verify(targets.Select(element => $"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }
    }
}
