////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Context;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;
using static VerifyNUnit.Verifier;

namespace FlyFlint.Internal.Static
{
    public sealed class StaticInjectorTests
    {
        private struct TargetValueType : IDataInjectable
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

        [Test]
        public Task InjectToValueType()
        {
            var data = new DataTable();
            data.Columns.Add("Id", typeof(int));
            data.Columns.Add("Name", typeof(string));
            data.Columns.Add("Birth", typeof(DateTime));
            data.Rows.Add(1, "AAAA", new DateTime(2022, 1, 23, 12, 34, 56, 789));

            using var reader = data.CreateDataReader();
            Assert.IsTrue(reader.Read());

            var element = new TargetValueType();

            var context = new DataInjectionContext(
                ConversionContext.Default, StringComparer.OrdinalIgnoreCase, reader);
            var metadataList = element.Prepare(context);

            element.Inject(context, metadataList);

            return Verify($"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)}");
        }

        private sealed class TargetReferenceType : IDataInjectable
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

        [Test]
        public Task InjectToReferenceType()
        {
            var data = new DataTable();
            data.Columns.Add("Id", typeof(int));
            data.Columns.Add("Name", typeof(string));
            data.Columns.Add("Birth", typeof(DateTime));
            data.Rows.Add(1, "AAAA", new DateTime(2022, 1, 23, 12, 34, 56, 789));

            using var reader = data.CreateDataReader();
            Assert.IsTrue(reader.Read());

            var element = new TargetReferenceType();

            var context = new DataInjectionContext(
                ConversionContext.Default, StringComparer.OrdinalIgnoreCase, reader);
            var metadataList = element.Prepare(context);

            element.Inject(context, metadataList);

            return Verify($"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)}");
        }
    }
}