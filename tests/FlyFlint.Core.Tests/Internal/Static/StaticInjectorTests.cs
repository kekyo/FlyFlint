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
        private struct TargetValueType : IDataInjectable
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

            private static readonly StaticDataInjectorDelegate<TargetValueType> injector = Inject;

            public void Prepare(StaticDataInjectionContext context) =>
                context.RegisterMetadata(members, injector);

            private static void Inject(
                StaticDataInjectionContext context, ref TargetValueType element)
            {
                element.Id = context.GetInt32(0);
                element.Name = context.GetString(1);
                element.Birth = context.GetDateTime(2);
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

            var context = new StaticDataInjectionContext<TargetValueType>(
                ConversionContext.Default, StringComparer.OrdinalIgnoreCase, reader);
            element.Prepare(context);

            context.Inject(ref element);

            return Verify($"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)}");
        }

        private sealed class TargetReferenceType : IDataInjectable
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

            private static readonly StaticDataInjectorDelegate<TargetReferenceType> injector = Inject;

            public void Prepare(StaticDataInjectionContext context) =>
                context.RegisterMetadata(members, injector);

            private static void Inject(
                StaticDataInjectionContext context, ref TargetReferenceType element)
            {
                element.Id = context.GetInt32(0);
                element.Name = context.GetString(1);
                element.Birth = context.GetDateTime(2);
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

            var context = new StaticDataInjectionContext<TargetReferenceType>(
                ConversionContext.Default, StringComparer.OrdinalIgnoreCase, reader);
            element.Prepare(context);

            context.Inject(ref element);

            return Verify($"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)}");
        }
    }
}