////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Context;
using FlyFlint.Internal;
using FlyFlint.Internal.Static;
using FlyFlint.Utilities;
using NUnit.Framework;
using System;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static VerifyNUnit.Verifier;

namespace FlyFlint
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

            public DataInjectionMetadata[] Prepare(DataInjectionContext context) =>
                context.Prepare(members);

            public void Inject(DataInjectionContext context, DataInjectionMetadata[] metadataList)
            {
                this.Id = context.GetInt32(metadataList[0]);
                this.Name = context.GetString(metadataList[1]);
                this.Birth = context.GetDateTime(metadataList[2]);
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
            var targets = await StaticQueryFacade.ExecuteAsync(qc).ToArrayAsync();

            await Verify(targets.Select(element => $"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }

        public sealed class Parameter : IParameterExtractable
        {
            public int idparam { get; set; }

            public (string name, object? value)[] Extract() =>
                new[] { ( "idparam", (object?)this.idparam ) };
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

            var query = StaticQueryFacade.Parameter(
                QueryExtension.Query<Target>(
                    connection, "SELECT * FROM target WHERE Id = @idparam"),
                    new Parameter { idparam = 2 });
            var targets = StaticQueryFacade.Execute(query).ToArray();

            await Verify(targets.Select(element => $"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }
    }
}
