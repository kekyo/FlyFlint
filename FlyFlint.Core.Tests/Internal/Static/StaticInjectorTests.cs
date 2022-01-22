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
        private struct TargetValueType : IDataInjectable<TargetValueType>
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

            private static readonly InjectDelegate<TargetValueType> injectDelegate = Inject;

            public PreparingResult<TargetValueType> Prepare(DataInjectionContext context) =>
                new PreparingResult<TargetValueType>(injectDelegate, context.Prepare(members));

            private static void Inject(
                ref TargetValueType element, DataInjectionContext context, DataInjectionMetadata[] metadataList)
            {
                element.Id = context.GetInt32(metadataList[0]);
                element.Name = context.GetString(metadataList[1]);
                element.Birth = context.GetDateTime(metadataList[2]);
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
            var pr = element.Prepare(context);

            pr.Injector(ref element, context, pr.MetadataList);

            return Verify($"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)}");
        }

        private sealed class TargetReferenceType : IDataInjectable<TargetReferenceType>
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

            private static readonly InjectDelegate<TargetReferenceType> injectDelegate = Inject;

            public PreparingResult<TargetReferenceType> Prepare(DataInjectionContext context) =>
                new PreparingResult<TargetReferenceType>(injectDelegate, context.Prepare(members));

            private static void Inject(
                ref TargetReferenceType element, DataInjectionContext context, DataInjectionMetadata[] metadataList)
            {
                element.Id = context.GetInt32(metadataList[0]);
                element.Name = context.GetString(metadataList[1]);
                element.Birth = context.GetDateTime(metadataList[2]);
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
            var pr = element.Prepare(context);

            pr.Injector(ref element, context, pr.MetadataList);

            return Verify($"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)}");
        }
    }
}