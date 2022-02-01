﻿////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Collections;
using FlyFlint.Internal;
using FlyFlint.Internal.Static;
using NUnit.Framework;
using System;
using System.Collections.Generic;
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

            private static readonly StaticMemberMetadata[] members = new[]
            {
                 new StaticMemberMetadata(nameof(Id), typeof(int)),
                 new StaticMemberMetadata(nameof(Name), typeof(string)),
                 new StaticMemberMetadata(nameof(Birth), typeof(DateTime)),
            };

            private static readonly StaticDataInjectorDelegate<Target> injector = Inject;

            public void Prepare(StaticDataInjectionContext context) =>
                context.RegisterMetadata(members, injector);

            private static void Inject(
                StaticDataInjectionContext context, ref Target element)
            {
                element.Id = context.GetInt32(0);
                element.Name = context.GetString(1);
                element.Birth = context.GetDateTime(2);
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

            var query = QueryExtension.Query<Target>(connection, "SELECT * FROM target");
            var targets = await QueryFacadeExtension.ExecuteAsync(query).ToArrayAsync();

            await Verify(targets.Select(element => $"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }

        public sealed class Parameter : IParameterExtractable
        {
            public int idparam { get; set; }

            public void Extract(
                StaticParameterExtractionContext context)
            {
                //base.Extract(context);
                context.RegisterParameter<int>(nameof(idparam), this.idparam);
            }
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

            //var query = QueryFacadeExtension.Parameter(
            //    QueryExtension.Query<Target>(
            //        connection, "SELECT * FROM target WHERE Id = @idparam"),
            //        new Parameter { idparam = 2 });
            var query = QueryFacadeExtension.Parameter(
                QueryExtension.Query<Target>(
                    connection, "SELECT * FROM target WHERE Id = @idparam"),
                    new Parameter { idparam = 2 });
            var targets = await QueryFacadeExtension.ExecuteAsync(query).ToArrayAsync();

            await Verify(targets.Select(element => $"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }

        [Test]
        public async Task QueryWithInlinedParameter()
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

            var idparam = 2;
            var query = QueryExtension.Query<Target>(
                connection, $"SELECT * FROM target WHERE Id = {idparam}");
            var targets = await QueryFacadeExtension.ExecuteAsync(query).ToArrayAsync();

            await Verify(targets.Select(element => $"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }
    }
}