////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Utilities;
using NUnit.Framework;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static VerifyNUnit.Verifier;

namespace FlyFlint
{
    public sealed class ConstructVariationTests
    {
        private struct Target
        {
            public int Id;
            public string? Name;
            public DateTime Birth;
        }

        /////////////////////////////////////////////////////////////////////////////

        private async Task<DbConnection> CreateConnectionAsync()
        {
            DynamicQuery.Enable();

            var connection = new SQLiteConnection("Data Source=:memory:");
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

            return connection;
        }

        /////////////////////////////////////////////////////////////////////////////

        [Test]
        public async Task QueryWithTypedParameter()
        {
            using var connection = await CreateConnectionAsync();

            var targets = await connection.Query<Target>("SELECT * FROM target WHERE Id = @idparam").
                Parameter(new { idparam = 2 }).
                ExecuteAsync().
                ToArrayAsync();

            await Verify(targets.Select(element => $"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }

        [Test]
        public async Task QueryWithInlineParameter()
        {
            using var connection = await CreateConnectionAsync();

            var idparam = 2;
            var targets = await connection.Query<Target>($"SELECT * FROM target WHERE Id = {idparam}").
                ExecuteAsync().
                ToArrayAsync();

            await Verify(targets.Select(element => $"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }

        /////////////////////////////////////////////////////////////////////////////

        [Test]
        public async Task QueryFromPreparedWithTypedParameter()
        {
            using var connection = await CreateConnectionAsync();

            var prepared = Query.Prepare<Target>("SELECT * FROM target WHERE Id = @idparam").
                Parameter(() => new { idparam = 2 });

            var targets = await connection.Query(prepared).
                ExecuteAsync().
                ToArrayAsync();

            await Verify(targets.Select(element => $"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }

        [Test]
        public async Task QueryFromPreparedWithInlineParameter()
        {
            using var connection = await CreateConnectionAsync();

            var idparam = 2;
            var prepared = Query.Prepare<Target>($"SELECT * FROM target WHERE Id = {idparam}");

            var targets = await connection.Query(prepared).
                ExecuteAsync().
                ToArrayAsync();

            await Verify(targets.Select(element => $"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }

        /////////////////////////////////////////////////////////////////////////////

        [Test]
        public async Task QueryAfterTypedParameter()
        {
            using var connection = await CreateConnectionAsync();

            var prepared = Query.Prepare<Target>("SELECT * FROM target WHERE Id = @idparam");

            var targets = await connection.Query(prepared).
                Parameter(new { idparam = 2 }).
                ExecuteAsync().
                ToArrayAsync();

            await Verify(targets.Select(element => $"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }

        /////////////////////////////////////////////////////////////////////////////

        [Test]
        public async Task QueryFromPreparedWithTypedParameterWithTypedElement()
        {
            using var connection = await CreateConnectionAsync();

            var prepared = Query.Prepare<Target>("SELECT * FROM target WHERE Id = @idparam").
                Parameter(() => new { idparam = 2 });

            var targets = await connection.Query(prepared).
                ExecuteAsync().
                ToArrayAsync();

            await Verify(targets.Select(element => $"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }

        [Test]
        public async Task QueryFromPreparedWithInlineParameterWithTypedElement()
        {
            using var connection = await CreateConnectionAsync();

            var idparam = 2;
            var prepared = Query.Prepare<Target>($"SELECT * FROM target WHERE Id = {idparam}");

            var targets = await connection.Query(prepared).
                ExecuteAsync().
                ToArrayAsync();

            await Verify(targets.Select(element => $"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }
    }
}
