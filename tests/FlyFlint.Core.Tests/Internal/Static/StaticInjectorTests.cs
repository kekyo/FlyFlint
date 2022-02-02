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
using System.Data;
using System.Globalization;
using System.Threading.Tasks;
using static VerifyNUnit.Verifier;

namespace FlyFlint.Internal.Static
{
    public sealed class StaticInjectorTests
    {
        private struct TargetValueType : IRecordInjectable
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

            private static readonly StaticRecordInjectorByRefDelegate<TargetValueType> injector = Inject;

            public void Prepare(StaticRecordInjectionContext context) =>
                context.RegisterMetadata(members, injector);

            private static void Inject(
                StaticRecordInjectionContext context, ref TargetValueType record)
            {
                record.Id = context.GetInt32(0);
                record.Name = context.GetString(1);
                record.Birth = context.GetDateTime(2);
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

            var record = new TargetValueType();

            var context = new StaticRecordInjectionByRefContext<TargetValueType>(
                ConversionContext.Default, StringComparer.OrdinalIgnoreCase, reader);
            record.Prepare(context);

            context.Inject(ref record);

            return Verify($"{record.Id},{record.Name},{record.Birth.ToString(CultureInfo.InvariantCulture)}");
        }

        private sealed class TargetReferenceType : IRecordInjectable
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

            private static readonly StaticRecordInjectorByRefDelegate<TargetReferenceType> injector = Inject;

            public void Prepare(StaticRecordInjectionContext context) =>
                context.RegisterMetadata(members, injector);

            private static void Inject(
                StaticRecordInjectionContext context, ref TargetReferenceType record)
            {
                record.Id = context.GetInt32(0);
                record.Name = context.GetString(1);
                record.Birth = context.GetDateTime(2);
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

            var record = new TargetReferenceType();

            var context = new StaticRecordInjectionByRefContext<TargetReferenceType>(
                ConversionContext.Default, StringComparer.OrdinalIgnoreCase, reader);
            record.Prepare(context);

            context.Inject(ref record);

            return Verify($"{record.Id},{record.Name},{record.Birth.ToString(CultureInfo.InvariantCulture)}");
        }
    }
}