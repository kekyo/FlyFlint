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
using System.Data.Common;
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

            private static readonly (string, Type)[] members = new[]
            {
                (nameof(Id), typeof(int)),
                (nameof(Name), typeof(string)),
                (nameof(Birth), typeof(DateTime)),
            };

            public DataInjectionMetadata[] PrepareAndInject(IFormatProvider fp, DbDataReader reader)
            {
                var metadataList = StaticInjectonHelper<TargetValueType>.Prepare(reader, members);
                this.Inject(fp, metadataList, reader);
                return metadataList;
            }

            public void Inject(IFormatProvider fp, DataInjectionMetadata[] metadataList, DbDataReader reader)
            {
                this.Id = StaticDataAccessor.GetInt32(fp, reader, metadataList[0]);
                this.Name = StaticDataAccessor.GetString(fp, reader, metadataList[1]);
                this.Birth = StaticDataAccessor.GetDateTime(fp, reader, metadataList[2]);
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

            var context = element.PrepareAndInject(CultureInfo.InvariantCulture, reader);

            element.Inject(CultureInfo.InvariantCulture, context, reader);

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

            public DataInjectionMetadata[] PrepareAndInject(IFormatProvider fp, DbDataReader reader)
            {
                var metadataList = StaticInjectonHelper<TargetReferenceType>.Prepare(reader, members);
                this.Inject(fp, metadataList, reader);
                return metadataList;
            }

            public void Inject(IFormatProvider fp, DataInjectionMetadata[] metadataList, DbDataReader reader)
            {
                this.Id = StaticDataAccessor.GetInt32(fp, reader, metadataList[0]);
                this.Name = StaticDataAccessor.GetString(fp, reader, metadataList[1]);
                this.Birth = StaticDataAccessor.GetDateTime(fp, reader, metadataList[2]);
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

            var metadataList = element.PrepareAndInject(CultureInfo.InvariantCulture, reader);

            element.Inject(CultureInfo.InvariantCulture, metadataList, reader);

            return Verify($"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)}");
        }
    }
}