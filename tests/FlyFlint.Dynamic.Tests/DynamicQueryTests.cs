////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Collections;
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
    public sealed class DynamicQueryTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp() =>
            DynamicQuery.Enable();

        /////////////////////////////////////////////////////////////////////////////

        private sealed class Target
        {
            public int Id;
            public string? Name;
            public DateTime Birth;
        }

        public sealed class Parameter
        {
            public int idparam { get; set; }
        }

        /////////////////////////////////////////////////////////////////////////////

        private async Task<DbConnection> CreateConnectionAsync()
        {
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
        public async Task Query()
        {
            using var connection = await CreateConnectionAsync();

            var query = connection.Query<Target>("SELECT * FROM target");
            var targets = await query.ExecuteNonParameterizedAsync().ToArrayAsync();

            await Verify(targets.Select(record => $"{record.Id},{record.Name},{record.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }

        [Test]
        public async Task QueryWithAnonymousParameter()
        {
            using var connection = await CreateConnectionAsync();

            var query = connection.Query<Target>(
                    "SELECT * FROM target WHERE Id = @idparam").
                    Parameter(new { idparam = 2 });
            var targets = await query.ExecuteAsync().ToArrayAsync();

            await Verify(targets.Select(record => $"{record.Id},{record.Name},{record.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }

        [Test]
        public async Task QueryWithParameter()
        {
            using var connection = await CreateConnectionAsync();

            var query = connection.Query<Target>(
                    "SELECT * FROM target WHERE Id = @idparam").
                    Parameter(new Parameter { idparam = 2 });
            var targets = await query.ExecuteAsync().ToArrayAsync();

            await Verify(targets.Select(record => $"{record.Id},{record.Name},{record.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }

        [Test]
        public async Task QueryWithInlinedParameter()
        {
            using var connection = await CreateConnectionAsync();

            var idparam = 2;
            var query = connection.Query<Target>(
                $"SELECT * FROM target WHERE Id = {idparam}");
            var targets = await query.ExecuteAsync().ToArrayAsync();

            await Verify(targets.Select(record => $"{record.Id},{record.Name},{record.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }

        /////////////////////////////////////////////////////////////////////////////

        [Test]
        public async Task QueryImmediately()
        {
            using var connection = await CreateConnectionAsync();

            var query = connection.Query<Target>("SELECT * FROM target");
            var targets = await query.ExecuteImmediatelyNonParameterizedAsync();

            await Verify(targets.Select(record => $"{record.Id},{record.Name},{record.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }

        [Test]
        public async Task QueryImmediatelyWithParameter()
        {
            using var connection = await CreateConnectionAsync();

            var query = connection.Query<Target>(
                    "SELECT * FROM target WHERE Id = @idparam").
                    Parameter(new Parameter { idparam = 2 });
            var targets = await query.ExecuteImmediatelyAsync();

            await Verify(targets.Select(record => $"{record.Id},{record.Name},{record.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }

        [Test]
        public async Task QueryImmediatelyWithAnonymousParameter()
        {
            using var connection = await CreateConnectionAsync();

            var query = connection.Query<Target>(
                    "SELECT * FROM target WHERE Id = @idparam").
                    Parameter(new { idparam = 2 });
            var targets = await query.ExecuteImmediatelyAsync();

            await Verify(targets.Select(record => $"{record.Id},{record.Name},{record.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }

        [Test]
        public async Task QueryImmediatelyWithInlinedParameter()
        {
            using var connection = await CreateConnectionAsync();

            var idparam = 2;
            var query = connection.Query<Target>(
                $"SELECT * FROM target WHERE Id = {idparam}");
            var targets = await query.ExecuteImmediatelyAsync();

            await Verify(targets.Select(record => $"{record.Id},{record.Name},{record.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }

        /////////////////////////////////////////////////////////////////////////////

        [Test]
        public async Task ScalarQuery()
        {
            using var connection = await CreateConnectionAsync();

            var query = connection.Query("SELECT Name FROM target WHERE Id = 2");
            var name = await query.ExecuteScalarNonParameterizedAsync<string>();

            await Verify(name);
        }

        [Test]
        public async Task ScalarQueryWithParameter()
        {
            using var connection = await CreateConnectionAsync();

            var query = connection.Query(
                    "SELECT Name FROM target WHERE Id = @idparam").
                    Parameter(new Parameter { idparam = 2 });
            var name = await query.ExecuteScalarAsync<string>();

            await Verify(name);
        }

        [Test]
        public async Task ScalarQueryWithAnonymousParameter()
        {
            using var connection = await CreateConnectionAsync();

            var query = connection.Query(
                    "SELECT Name FROM target WHERE Id = @idparam").
                    Parameter(new { idparam = 2 });
            var name = await query.ExecuteScalarAsync<string>();

            await Verify(name);
        }

        [Test]
        public async Task ScalarQueryWithInlinedParameter()
        {
            using var connection = await CreateConnectionAsync();

            var idparam = 2;
            var query = connection.Query(
                $"SELECT Name FROM target WHERE Id = {idparam}");
            var name = await query.ExecuteScalarAsync<string>();

            await Verify(name);
        }

        [Test]
        public async Task ScalarQuery2()
        {
            using var connection = await CreateConnectionAsync();

            var query = connection.Query("SELECT Birth FROM target WHERE Id = 1");
            var birth = await query.ExecuteScalarNonParameterizedAsync<DateTime>();

            await Verify(birth.ToString(CultureInfo.InvariantCulture));
        }

        [Test]
        public async Task ScalarQueryWithParameter2()
        {
            using var connection = await CreateConnectionAsync();

            var query = connection.Query(
                    "SELECT Birth FROM target WHERE Id = @idparam").
                    Parameter(new Parameter { idparam = 1 });
            var birth = await query.ExecuteScalarAsync<DateTime>();

            await Verify(birth.ToString(CultureInfo.InvariantCulture));
        }

        [Test]
        public async Task ScalarQueryWithAnonymousParameter2()
        {
            using var connection = await CreateConnectionAsync();

            var query = connection.Query(
                    "SELECT Birth FROM target WHERE Id = @idparam").
                    Parameter(new { idparam = 1 });
            var birth = await query.ExecuteScalarAsync<DateTime>();

            await Verify(birth.ToString(CultureInfo.InvariantCulture));
        }

        [Test]
        public async Task ScalarQueryWithInlinedParameter2()
        {
            using var connection = await CreateConnectionAsync();

            var idparam = 1;
            var query = connection.Query(
                $"SELECT Birth FROM target WHERE Id = {idparam}");
            var birth = await query.ExecuteScalarAsync<DateTime>();

            await Verify(birth.ToString(CultureInfo.InvariantCulture));
        }

        /////////////////////////////////////////////////////////////////////////////

        [Test]
        public async Task NonQuery()
        {
            using var connection = await CreateConnectionAsync();

            var query = connection.Query(
                "UPDATE target SET Name='ZZZZZ'");
            var count = await query.ExecuteNonQueryNonParameterizedAsync();

            await Verify(count);
        }

        [Test]
        public async Task NonQueryWithParameter()
        {
            using var connection = await CreateConnectionAsync();

            var query = connection.Query(
                "UPDATE target SET Name='ZZZZZ' WHERE Id = @idparam").
                Parameter(new Parameter { idparam = 2 });
            var count = await query.ExecuteNonQueryAsync();

            await Verify(count);
        }

        [Test]
        public async Task NonQueryWithAnonymousParameter()
        {
            using var connection = await CreateConnectionAsync();

            var query = connection.Query(
                "UPDATE target SET Name='ZZZZZ' WHERE Id = @idparam").
                Parameter(new { idparam = 2 });
            var count = await query.ExecuteNonQueryAsync();

            await Verify(count);
        }

        [Test]
        public async Task NonQueryWithInlinedParameter()
        {
            using var connection = await CreateConnectionAsync();

            var idparam = 2;
            var query = connection.Query(
                $"UPDATE target SET Name='ZZZZZ' WHERE Id = {idparam}");
            var count = await query.ExecuteNonQueryAsync();

            await Verify(count);
        }
    }
}
