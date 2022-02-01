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
    public sealed class DynamicInjectorWithDataMemberTests
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

            public int GetId() =>
                this.Id;
            public string GetAddress() =>
                this.Address;
        }

        [Test]
        public Task InjectToFieldValueType()
        {
            var data = new DataTable();
            data.Columns.Add("Id", typeof(int));
            data.Columns.Add("Name", typeof(string));
            data.Columns.Add("Birth", typeof(DateTime));
            data.Columns.Add("Address", typeof(string));
            data.Rows.Add(1, "AAAA", new DateTime(2022, 1, 23, 12, 34, 56, 789), "ABC");

            using var reader = data.CreateDataReader();
            Assert.IsTrue(reader.Read());

            var context = new DynamicDataInjectionContext<FieldValueType>(
                ConversionContext.Default, StringComparer.OrdinalIgnoreCase, reader);

            var element = new FieldValueType();

            context.Inject(ref element);

            return Verify($"{element.GetId()},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)},{element.GetAddress()}");
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

            public int GetId() =>
                this.Id;
            public string? GetAddress() =>
                this.Address;
        }

        [Test]
        public Task InjectToFieldReferenceType()
        {
            var data = new DataTable();
            data.Columns.Add("Id", typeof(int));
            data.Columns.Add("Name", typeof(string));
            data.Columns.Add("Birth", typeof(DateTime));
            data.Columns.Add("Address", typeof(string));
            data.Rows.Add(1, "AAAA", new DateTime(2022, 1, 23, 12, 34, 56, 789), "ABC");

            using var reader = data.CreateDataReader();
            Assert.IsTrue(reader.Read());

            var context = new DynamicDataInjectionContext<FieldReferenceType>(
                ConversionContext.Default, StringComparer.OrdinalIgnoreCase, reader);

            var element = new FieldReferenceType();

            context.Inject(ref element);

            return Verify($"{element.GetId()},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)},{element.GetAddress()}");
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

            public int GetId() =>
                this.Id;
            public string? GetAddress() =>
                this.Address;
        }

        [Test]
        public Task InjectToPropertyValueType()
        {
            var data = new DataTable();
            data.Columns.Add("Id", typeof(int));
            data.Columns.Add("Name", typeof(string));
            data.Columns.Add("Birth", typeof(DateTime));
            data.Columns.Add("Address", typeof(string));
            data.Rows.Add(1, "AAAA", new DateTime(2022, 1, 23, 12, 34, 56, 789), "ABC");

            using var reader = data.CreateDataReader();
            Assert.IsTrue(reader.Read());

            var context = new DynamicDataInjectionContext<PropertyValueType>(
                ConversionContext.Default, StringComparer.OrdinalIgnoreCase, reader);

            var element = new PropertyValueType();

            context.Inject(ref element);

            return Verify($"{element.GetId()},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)},{element.GetAddress()}");
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

            public int GetId() =>
                this.Id;
            public string? GetAddress() =>
                this.Address;
        }

        [Test]
        public Task InjectToPropertyReferenceType()
        {
            var data = new DataTable();
            data.Columns.Add("Id", typeof(int));
            data.Columns.Add("Name", typeof(string));
            data.Columns.Add("Birth", typeof(DateTime));
            data.Columns.Add("Address", typeof(string));
            data.Rows.Add(1, "AAAA", new DateTime(2022, 1, 23, 12, 34, 56, 789), "ABC");

            using var reader = data.CreateDataReader();
            Assert.IsTrue(reader.Read());

            var context = new DynamicDataInjectionContext<PropertyReferenceType>(
                ConversionContext.Default, StringComparer.OrdinalIgnoreCase, reader);

            var element = new PropertyReferenceType();

            context.Inject(ref element);

            return Verify($"{element.GetId()},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)},{element.GetAddress()}");
        }
    }
}