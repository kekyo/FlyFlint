////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using NUnit.Framework;
using System;
using System.Data;
using System.Globalization;
using System.Text;
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

            private static readonly (string, Type)[] members = new[]
            {
                (nameof(Id), typeof(int)),
                (nameof(Name), typeof(string)),
                (nameof(Birth), typeof(DateTime)),
            };

            public DataInjectionMetadata[] PrepareAndInject(DataInjectionContext context)
            {
                var metadataList = StaticInjectonHelper<TargetValueType>.Prepare(context, members);
                this.Inject(context, metadataList);
                return metadataList;
            }

            public void Inject(DataInjectionContext context, DataInjectionMetadata[] metadataList)
            {
                this.Id = StaticDataAccessor.GetInt32(context, metadataList[0]);
                this.Name = StaticDataAccessor.GetString(context, metadataList[1]);
                this.Birth = StaticDataAccessor.GetDateTime(context, metadataList[2]);
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

            var context = new DataInjectionContext(reader, CultureInfo.InvariantCulture, Encoding.UTF8);
            element.PrepareAndInject(context);

            return Verify($"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)}");
        }

        private sealed class TargetReferenceType : IDataInjectable
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

            public DataInjectionMetadata[] PrepareAndInject(DataInjectionContext context)
            {
                var metadataList = StaticInjectonHelper<TargetReferenceType>.Prepare(context, members);
                this.Inject(context, metadataList);
                return metadataList;
            }

            public void Inject(DataInjectionContext context, DataInjectionMetadata[] metadataList)
            {
                this.Id = StaticDataAccessor.GetInt32(context, metadataList[0]);
                this.Name = StaticDataAccessor.GetString(context, metadataList[1]);
                this.Birth = StaticDataAccessor.GetDateTime(context, metadataList[2]);
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

            var context = new DataInjectionContext(reader, CultureInfo.InvariantCulture, Encoding.UTF8);
            element.PrepareAndInject(context);

            return Verify($"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)}");
        }
    }
}