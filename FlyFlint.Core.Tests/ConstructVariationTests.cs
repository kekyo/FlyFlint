﻿////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Context;
using FlyFlint.Internal.Static;
using FlyFlint.Utilities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
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
        private struct Target : IDataInjectable
        {
            public int Id;
            public string? Name;
            public DateTime Birth;

            private static readonly KeyValuePair<string, Type>[] members = new[]
            {
                 new KeyValuePair<string, Type>(nameof(Id), typeof(int)),
                 new KeyValuePair<string, Type>(nameof(Name), typeof(string)),
                 new KeyValuePair<string, Type>(nameof(Birth), typeof(DateTime)),
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

        public sealed class Parameter : IParameterExtractable
        {
            public int idparam { get; set; }

            public KeyValuePair<string, object?>[] Extract() =>
                new[] { new KeyValuePair<string, object?>("idparam", this.idparam) };
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
        public async Task QueryWithTypedParameter()
        {
            using var connection = await CreateConnectionAsync();

            var targets = await connection.Query<Target>("SELECT * FROM target WHERE Id = @idparam").
                Parameter__(new Parameter { idparam = 2 }).
                ExecuteAsync__().
                ToArrayAsync();

            await Verify(targets.Select(element => $"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }

        [Test]
        public async Task QueryWithInlineParameter()
        {
            using var connection = await CreateConnectionAsync();

            var idparam = 2;
            var targets = await connection.Query<Target>($"SELECT * FROM target WHERE Id = {idparam}").
                ExecuteAsync__().
                ToArrayAsync();

            await Verify(targets.Select(element => $"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }

        /////////////////////////////////////////////////////////////////////////////

        [Test]
        public async Task QueryFromPreparedWithTypedParameter()
        {
            using var connection = await CreateConnectionAsync();

            var prepared = Query.Prepare<Target>("SELECT * FROM target WHERE Id = @idparam").
                Parameter__(() => new Parameter { idparam = 2 });

            var targets = await connection.Query(prepared).
                ExecuteAsync__().
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
                ExecuteAsync__().
                ToArrayAsync();

            await Verify(targets.Select(element => $"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }

        /////////////////////////////////////////////////////////////////////////////

        [Test]
        public async Task QueryFromPreparedWithTypedParameterAfterTypedElement()
        {
            using var connection = await CreateConnectionAsync();

            var prepared = Query.Prepare("SELECT * FROM target WHERE Id = @idparam").
                Parameter__(() => new Parameter { idparam = 2 }).
                Typed<Target>();

            var targets = await connection.Query(prepared).
                ExecuteAsync__().
                ToArrayAsync();

            await Verify(targets.Select(element => $"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }

        [Test]
        public async Task QueryFromPreparedWithInlineParameterAfterTypedElement()
        {
            using var connection = await CreateConnectionAsync();

            var idparam = 2;
            var prepared = Query.Prepare($"SELECT * FROM target WHERE Id = {idparam}").
                Typed<Target>();

            var targets = await connection.Query(prepared).
                ExecuteAsync__().
                ToArrayAsync();

            await Verify(targets.Select(element => $"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }

        /////////////////////////////////////////////////////////////////////////////

        [Test]
        public async Task QueryFromPreparedWithTypedParameterBeforeTypedElement()
        {
            using var connection = await CreateConnectionAsync();

            var prepared = Query.Prepare("SELECT * FROM target WHERE Id = @idparam").
                Typed<Target>().
                Parameter__(() => new Parameter { idparam = 2 });

            var targets = await connection.Query(prepared).
                ExecuteAsync__().
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
                Parameter__(new Parameter { idparam = 2 }).
                ExecuteAsync__().
                ToArrayAsync();

            await Verify(targets.Select(element => $"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }

        /////////////////////////////////////////////////////////////////////////////

        [Test]
        public async Task QueryAfterTypedParameterAfterTypedElement()
        {
            using var connection = await CreateConnectionAsync();

            var prepared = Query.Prepare("SELECT * FROM target WHERE Id = @idparam");

            var targets = await connection.Query(prepared).
                Parameter__(new Parameter { idparam = 2 }).
                Typed<Target>().
                ExecuteAsync__().
                ToArrayAsync();

            await Verify(targets.Select(element => $"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }

        [Test]
        public async Task QueryAfterTypedParameterBeforeTypedElement()
        {
            using var connection = await CreateConnectionAsync();

            var prepared = Query.Prepare("SELECT * FROM target WHERE Id = @idparam");

            var targets = await connection.Query(prepared).
                Typed<Target>().
                Parameter__(new Parameter { idparam = 2 }).
                ExecuteAsync__().
                ToArrayAsync();

            await Verify(targets.Select(element => $"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }

        /////////////////////////////////////////////////////////////////////////////

        [Test]
        public async Task QueryWithTypedElementAfterTypedParameter()
        {
            using var connection = await CreateConnectionAsync();

            var prepared = Query.Prepare("SELECT * FROM target WHERE Id = @idparam");

            var targets = await connection.Query<Target>(prepared).
                Parameter__(new Parameter { idparam = 2 }).
                ExecuteAsync__().
                ToArrayAsync();

            await Verify(targets.Select(element => $"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }

        /////////////////////////////////////////////////////////////////////////////

        [Test]
        public async Task QueryFromPreparedWithTypedParameterWithTypedElement()
        {
            using var connection = await CreateConnectionAsync();

            var prepared = Query.Prepare("SELECT * FROM target WHERE Id = @idparam").
                Parameter__(() => new Parameter { idparam = 2 });

            var targets = await connection.Query<Target>(prepared).
                ExecuteAsync__().
                ToArrayAsync();

            await Verify(targets.Select(element => $"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }

        [Test]
        public async Task QueryFromPreparedWithInlineParameterWithTypedElement()
        {
            using var connection = await CreateConnectionAsync();

            var idparam = 2;
            var prepared = Query.Prepare($"SELECT * FROM target WHERE Id = {idparam}");

            var targets = await connection.Query<Target>(prepared).
                ExecuteAsync__().
                ToArrayAsync();

            await Verify(targets.Select(element => $"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)}"));
        }
    }
}
