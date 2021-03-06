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

namespace FlyFlint.Internal.Dynamic
{
    public sealed class DynamicRecordInjectorWithQueryFieldTests
    {
        public struct FieldValueType
        {
            [QueryField]
            public DateTime Birth;
            [QueryIgnore]
            public string? Name;
            [QueryField]
            private int Id;
            private string Address;
            [QueryField("Data1")]
            public int Foo1;
            [QueryField("Data2")]
            private int Foo2;

            public int GetId() =>
                this.Id;
            public string GetAddress() =>
                this.Address;
            public int GetFoo2() =>
                this.Foo2;
        }

        [Test]
        public Task InjectToFieldValueType()
        {
            var data = new DataTable();
            data.Columns.Add("Id", typeof(int));
            data.Columns.Add("Name", typeof(string));
            data.Columns.Add("Birth", typeof(DateTime));
            data.Columns.Add("Address", typeof(string));
            data.Columns.Add("Data1", typeof(int));
            data.Columns.Add("Data2", typeof(int));
            data.Rows.Add(1, "AAAA", new DateTime(2022, 1, 23, 12, 34, 56, 789), "ABC", 111, 222);

            using var reader = data.CreateDataReader();
            Assert.IsTrue(reader.Read());

            var context = new DynamicRecordInjectionContext<FieldValueType>(
                ConversionContext.Default, StringComparer.OrdinalIgnoreCase, reader);

            var record = new FieldValueType();

            context.Inject(ref record);

            return Verify($"{record.GetId()},{record.Name},{record.Birth.ToString(CultureInfo.InvariantCulture)},{record.GetAddress()},{record.Foo1},{record.GetFoo2()}");
        }

        public sealed class FieldReferenceType
        {
            [QueryField]
            public DateTime Birth;
            [QueryIgnore]
            public string? Name;
            [QueryField]
            private int Id;
            private string? Address;
            [QueryField("Data1")]
            public int Foo1;
            [QueryField("Data2")]
            private int Foo2;

            public int GetId() =>
                this.Id;
            public string? GetAddress() =>
                this.Address;
            public int GetFoo2() =>
                this.Foo2;
        }

        [Test]
        public Task InjectToFieldReferenceType()
        {
            var data = new DataTable();
            data.Columns.Add("Id", typeof(int));
            data.Columns.Add("Name", typeof(string));
            data.Columns.Add("Birth", typeof(DateTime));
            data.Columns.Add("Address", typeof(string));
            data.Columns.Add("Data1", typeof(int));
            data.Columns.Add("Data2", typeof(int));
            data.Rows.Add(1, "AAAA", new DateTime(2022, 1, 23, 12, 34, 56, 789), "ABC", 111, 222);

            using var reader = data.CreateDataReader();
            Assert.IsTrue(reader.Read());

            var context = new DynamicRecordInjectionContext<FieldReferenceType>(
                ConversionContext.Default, StringComparer.OrdinalIgnoreCase, reader);

            var record = new FieldReferenceType();

            context.Inject(ref record);

            return Verify($"{record.GetId()},{record.Name},{record.Birth.ToString(CultureInfo.InvariantCulture)},{record.GetAddress()},{record.Foo1},{record.GetFoo2()}");
        }

        public struct PropertyValueType
        {
            [QueryField]
            public DateTime Birth { get; set; }
            [QueryIgnore]
            public string? Name { get; set; }
            [QueryField]
            private int Id { get; set; }
            private string? Address { get; set; }
            [QueryField("Data1")]
            public int Foo1 { get; set; }
            [QueryField("Data2")]
            private int Foo2 { get; set; }

            public int GetId() =>
                this.Id;
            public string? GetAddress() =>
                this.Address;
            public int GetFoo2() =>
                this.Foo2;
        }

        [Test]
        public Task InjectToPropertyValueType()
        {
            var data = new DataTable();
            data.Columns.Add("Id", typeof(int));
            data.Columns.Add("Name", typeof(string));
            data.Columns.Add("Birth", typeof(DateTime));
            data.Columns.Add("Address", typeof(string));
            data.Columns.Add("Data1", typeof(int));
            data.Columns.Add("Data2", typeof(int));
            data.Rows.Add(1, "AAAA", new DateTime(2022, 1, 23, 12, 34, 56, 789), "ABC", 111, 222);

            using var reader = data.CreateDataReader();
            Assert.IsTrue(reader.Read());

            var context = new DynamicRecordInjectionContext<PropertyValueType>(
                ConversionContext.Default, StringComparer.OrdinalIgnoreCase, reader);

            var record = new PropertyValueType();

            context.Inject(ref record);

            return Verify($"{record.GetId()},{record.Name},{record.Birth.ToString(CultureInfo.InvariantCulture)},{record.GetAddress()},{record.Foo1},{record.GetFoo2()}");
        }

        public sealed class PropertyReferenceType
        {
            [QueryField]
            public DateTime Birth { get; set; }
            [QueryIgnore]
            public string? Name { get; set; }
            [QueryField]
            private int Id { get; set; }
            private string? Address { get; set; }
            [QueryField("Data1")]
            public int Foo1 { get; set; }
            [QueryField("Data2")]
            private int Foo2 { get; set; }

            public int GetId() =>
                this.Id;
            public string? GetAddress() =>
                this.Address;
            public int GetFoo2() =>
                this.Foo2;
        }

        [Test]
        public Task InjectToPropertyReferenceType()
        {
            var data = new DataTable();
            data.Columns.Add("Id", typeof(int));
            data.Columns.Add("Name", typeof(string));
            data.Columns.Add("Birth", typeof(DateTime));
            data.Columns.Add("Address", typeof(string));
            data.Columns.Add("Data1", typeof(int));
            data.Columns.Add("Data2", typeof(int));
            data.Rows.Add(1, "AAAA", new DateTime(2022, 1, 23, 12, 34, 56, 789), "ABC", 111, 222);

            using var reader = data.CreateDataReader();
            Assert.IsTrue(reader.Read());

            var context = new DynamicRecordInjectionContext<PropertyReferenceType>(
                ConversionContext.Default, StringComparer.OrdinalIgnoreCase, reader);

            var record = new PropertyReferenceType();

            context.Inject(ref record);

            return Verify($"{record.GetId()},{record.Name},{record.Birth.ToString(CultureInfo.InvariantCulture)},{record.GetAddress()},{record.Foo1},{record.GetFoo2()}");
        }
    }
}