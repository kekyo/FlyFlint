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

namespace FlyFlint.Synchronized
{
    public sealed class StaticQueryTests
    {
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

        private DbConnection CreateConnection()
        {
            var connection = new SQLiteConnection("Data Source=:memory:");
            connection.Open();

            var c = connection.CreateCommand();
            c.CommandType = CommandType.Text;
            c.CommandText = "CREATE TABLE target (Id INTEGER PRIMARY KEY,Name TEXT,Birth TEXT)";
            c.ExecuteNonQuery();

            c.CommandText = "INSERT INTO target VALUES (1,'AAAAA','2022/01/23 12:34:56.789')";
            c.ExecuteNonQuery();
            c.CommandText = "INSERT INTO target VALUES (2,'BBBBB','2022/01/23 12:34:57.789')";
            c.ExecuteNonQuery();
            c.CommandText = "INSERT INTO target VALUES (3,'CCCCC','2022/01/23 12:34:58.789')";
            c.ExecuteNonQuery();

            return connection;
        }

        /////////////////////////////////////////////////////////////////////////////

        [Test]
        public async Task Query()
        {
            using var connection = CreateConnection();

            var query = connection.Query<Target>("SELECT * FROM target");
            var targets = query.ExecuteNonParameterized().ToArray();

            await Verify(targets.Select(record => $"{record.Id},{record.Name},{record.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }

        [Test]
        public async Task QueryWithParameter()
        {
            using var connection = CreateConnection();

            var query = connection.Query<Target>(
                    "SELECT * FROM target WHERE Id = @idparam").
                    Parameter(new Parameter { idparam = 2 });
            var targets = query.Execute().ToArray();

            await Verify(targets.Select(record => $"{record.Id},{record.Name},{record.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }

        [Test]
        public async Task QueryWithAnonymousParameter()
        {
            using var connection = CreateConnection();

            var query = connection.Query<Target>(
                    "SELECT * FROM target WHERE Id = @idparam").
                    Parameter(new { idparam = 2 });
            var targets = query.Execute().ToArray();

            await Verify(targets.Select(record => $"{record.Id},{record.Name},{record.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }

        [Test]
        public async Task QueryWithInlinedParameter()
        {
            using var connection = CreateConnection();

            var idparam = 2;
            var query = connection.Query<Target>(
                $"SELECT * FROM target WHERE Id = {idparam}");
            var targets = query.Execute().ToArray();

            await Verify(targets.Select(record => $"{record.Id},{record.Name},{record.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }

        /////////////////////////////////////////////////////////////////////////////

        [Test]
        public async Task QueryImmediately()
        {
            using var connection = CreateConnection();

            var query = connection.Query<Target>("SELECT * FROM target");
            var targets = query.ExecuteImmediatelyNonParameterized();

            await Verify(targets.Select(record => $"{record.Id},{record.Name},{record.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }

        [Test]
        public async Task QueryImmediatelyWithParameter()
        {
            using var connection = CreateConnection();

            var query = connection.Query<Target>(
                    "SELECT * FROM target WHERE Id = @idparam").
                    Parameter(new Parameter { idparam = 2 });
            var targets = query.ExecuteImmediately();

            await Verify(targets.Select(record => $"{record.Id},{record.Name},{record.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }

        [Test]
        public async Task QueryImmediatelyWithAnonymousParameter()
        {
            using var connection = CreateConnection();

            var query = connection.Query<Target>(
                    "SELECT * FROM target WHERE Id = @idparam").
                    Parameter(new { idparam = 2 });
            var targets = query.ExecuteImmediately();

            await Verify(targets.Select(record => $"{record.Id},{record.Name},{record.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }

        [Test]
        public async Task QueryImmediatelyWithInlinedParameter()
        {
            using var connection = CreateConnection();

            var idparam = 2;
            var query = connection.Query<Target>(
                $"SELECT * FROM target WHERE Id = {idparam}");
            var targets = query.ExecuteImmediately();

            await Verify(targets.Select(record => $"{record.Id},{record.Name},{record.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }

        /////////////////////////////////////////////////////////////////////////////

        [Test]
        public async Task ScalarQuery()
        {
            using var connection = CreateConnection();

            var query = connection.Query("SELECT Name FROM target WHERE Id = 2");
            var name = query.ExecuteScalarNonParameterized<string>();

            await Verify(name);
        }

        [Test]
        public async Task ScalarQueryWithParameter()
        {
            using var connection = CreateConnection();

            var query = connection.Query(
                    "SELECT Name FROM target WHERE Id = @idparam").
                    Parameter(new Parameter { idparam = 2 });
            var name = query.ExecuteScalar<string>();

            await Verify(name);
        }

        [Test]
        public async Task ScalarQueryWithAnonymousParameter()
        {
            using var connection = CreateConnection();

            var query = connection.Query(
                    "SELECT Name FROM target WHERE Id = @idparam").
                    Parameter(new { idparam = 2 });
            var name = query.ExecuteScalar<string>();

            await Verify(name);
        }

        [Test]
        public async Task ScalarQueryWithInlinedParameter()
        {
            using var connection = CreateConnection();

            var idparam = 2;
            var query = connection.Query(
                $"SELECT Name FROM target WHERE Id = {idparam}");
            var name = query.ExecuteScalar<string>();

            await Verify(name);
        }

        [Test]
        public async Task ScalarQuery2()
        {
            using var connection = CreateConnection();

            var query = connection.Query("SELECT Birth FROM target WHERE Id = 1");
            var birth = query.ExecuteScalarNonParameterized<DateTime>();

            await Verify(birth.ToString(CultureInfo.InvariantCulture));
        }

        [Test]
        public async Task ScalarQueryWithParameter2()
        {
            using var connection = CreateConnection();

            var query = connection.Query(
                    "SELECT Birth FROM target WHERE Id = @idparam").
                    Parameter(new Parameter { idparam = 1 });
            var birth = query.ExecuteScalar<DateTime>();

            await Verify(birth.ToString(CultureInfo.InvariantCulture));
        }

        [Test]
        public async Task ScalarQueryWithAnonymousParameter2()
        {
            using var connection = CreateConnection();

            var query = connection.Query(
                    "SELECT Birth FROM target WHERE Id = @idparam").
                    Parameter(new { idparam = 1 });
            var birth = query.ExecuteScalar<DateTime>();

            await Verify(birth.ToString(CultureInfo.InvariantCulture));
        }

        [Test]
        public async Task ScalarQueryWithInlinedParameter2()
        {
            using var connection = CreateConnection();

            var idparam = 1;
            var query = connection.Query(
                $"SELECT Birth FROM target WHERE Id = {idparam}");
            var birth = query.ExecuteScalar<DateTime>();

            await Verify(birth.ToString(CultureInfo.InvariantCulture));
        }

        /////////////////////////////////////////////////////////////////////////////

        [Test]
        public async Task NonQuery()
        {
            using var connection = CreateConnection();

            var query = connection.Query(
                "UPDATE target SET Name='ZZZZZ'");
            var count = query.ExecuteNonQueryNonParameterized();

            await Verify(count);
        }

        [Test]
        public async Task NonQueryWithParameter()
        {
            using var connection = CreateConnection();

            var query = connection.Query(
                "UPDATE target SET Name='ZZZZZ' WHERE Id = @idparam").
                Parameter(new Parameter { idparam = 2 });
            var count = query.ExecuteNonQuery();

            await Verify(count);
        }

        [Test]
        public async Task NonQueryWithAnonymousParameter()
        {
            using var connection = CreateConnection();

            var query = connection.Query(
                "UPDATE target SET Name='ZZZZZ' WHERE Id = @idparam").
                Parameter(new { idparam = 2 });
            var count = query.ExecuteNonQuery();

            await Verify(count);
        }

        [Test]
        public async Task NonQueryWithInlinedParameter()
        {
            using var connection = CreateConnection();

            var idparam = 2;
            var query = connection.Query(
                $"UPDATE target SET Name='ZZZZZ' WHERE Id = {idparam}");
            var count = query.ExecuteNonQuery();

            await Verify(count);
        }
    }
}
