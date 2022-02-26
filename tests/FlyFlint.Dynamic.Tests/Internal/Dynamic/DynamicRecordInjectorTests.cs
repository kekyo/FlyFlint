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
    public sealed class DynamicRecordInjectorTests
    {
        public struct FieldValueType
        {
            public DateTime Birth;
            public string? Name;
            public int Id;
            public int Age;
            public double Weight;
        }

        [Test]
        public Task InjectToFieldValueType()
        {
            var data = new DataTable();
            data.Columns.Add("Id", typeof(int));
            data.Columns.Add("Name", typeof(string));
            data.Columns.Add("Birth", typeof(DateTime));
            data.Columns.Add("Height", typeof(double));
            data.Columns.Add("Weight", typeof(double));
            data.Rows.Add(1, "AAAA", new DateTime(2022, 1, 23, 12, 34, 56, 789), 123.4, 234.5);

            using var reader = data.CreateDataReader();
            Assert.IsTrue(reader.Read());

            var context = new DynamicRecordInjectionContext<FieldValueType>(
                ConversionContext.Default, StringComparer.OrdinalIgnoreCase, reader);

            var record = new FieldValueType();

            context.Inject(ref record);

            return Verify($"{record.Id},{record.Name},{record.Birth.ToString(CultureInfo.InvariantCulture)},{record.Weight},{record.Age}");
        }

        /////////////////////////////////////////////////////////////

        public sealed class FieldReferenceType
        {
            public DateTime Birth;
            public string? Name;
            public int Id;
            public int Age;
            public double Weight;
        }

        [Test]
        public Task InjectToFieldReferenceType()
        {
            var data = new DataTable();
            data.Columns.Add("Id", typeof(int));
            data.Columns.Add("Name", typeof(string));
            data.Columns.Add("Birth", typeof(DateTime));
            data.Columns.Add("Height", typeof(double)).AllowDBNull = true;
            data.Columns.Add("Weight", typeof(double)).AllowDBNull = true;
            data.Rows.Add(1, "AAAA", new DateTime(2022, 1, 23, 12, 34, 56, 789), 123.4, 234.5);

            using var reader = data.CreateDataReader();
            Assert.IsTrue(reader.Read());

            var context = new DynamicRecordInjectionContext<FieldReferenceType>(
                ConversionContext.Default, StringComparer.OrdinalIgnoreCase, reader);

            var record = new FieldReferenceType();

            context.Inject(ref record);

            return Verify($"{record.Id},{record.Name},{record.Birth.ToString(CultureInfo.InvariantCulture)},{record.Weight},{record.Age}");
        }

        /////////////////////////////////////////////////////////////

        public struct PropertyValueType
        {
            public DateTime Birth { get; set; }
            public string? Name { get; set; }
            public int Id { get; set; }
            public int Age { get; set; }
            public double Weight { get; set; }
        }

        [Test]
        public Task InjectToPropertyValueType()
        {
            var data = new DataTable();
            data.Columns.Add("Id", typeof(int));
            data.Columns.Add("Name", typeof(string));
            data.Columns.Add("Birth", typeof(DateTime));
            data.Columns.Add("Height", typeof(double)).AllowDBNull = true;
            data.Columns.Add("Weight", typeof(double)).AllowDBNull = true;
            data.Rows.Add(1, "AAAA", new DateTime(2022, 1, 23, 12, 34, 56, 789), 123.4, 234.5);

            using var reader = data.CreateDataReader();
            Assert.IsTrue(reader.Read());

            var context = new DynamicRecordInjectionContext<PropertyValueType>(
                ConversionContext.Default, StringComparer.OrdinalIgnoreCase, reader);

            var record = new PropertyValueType();

            context.Inject(ref record);

            return Verify($"{record.Id},{record.Name},{record.Birth.ToString(CultureInfo.InvariantCulture)},{record.Weight},{record.Age}");
        }

        /////////////////////////////////////////////////////////////

        public sealed class PropertyReferenceType
        {
            public DateTime Birth { get; set; }
            public string? Name { get; set; }
            public int Id { get; set; }
            public int Age { get; set; }
            public double Weight { get; set; }
        }

        [Test]
        public Task InjectToPropertyReferenceType()
        {
            var data = new DataTable();
            data.Columns.Add("Id", typeof(int));
            data.Columns.Add("Name", typeof(string));
            data.Columns.Add("Birth", typeof(DateTime));
            data.Columns.Add("Height", typeof(double)).AllowDBNull = true;
            data.Columns.Add("Weight", typeof(double)).AllowDBNull = true;
            data.Rows.Add(1, "AAAA", new DateTime(2022, 1, 23, 12, 34, 56, 789), 123.4, 234.5);

            using var reader = data.CreateDataReader();
            Assert.IsTrue(reader.Read());

            var context = new DynamicRecordInjectionContext<PropertyReferenceType>(
                ConversionContext.Default, StringComparer.OrdinalIgnoreCase, reader);

            var record = new PropertyReferenceType();

            context.Inject(ref record);

            return Verify($"{record.Id},{record.Name},{record.Birth.ToString(CultureInfo.InvariantCulture)},{record.Weight},{record.Age}");
        }

        /////////////////////////////////////////////////////////////

        public class FieldBaseReferenceType
        {
            public DateTime Birth;
            public string? Name;
            public int Id;
            public int Age;
            public double Weight;
        }

        public class FieldDerivedReferenceType : FieldBaseReferenceType
        {
            public string? Address;
            public string? Telephone;
            public Guid Guid;
        }

        [Test]
        public Task InjectToFieldDerivedReferenceType()
        {
            var data = new DataTable();
            data.Columns.Add("Id", typeof(int));
            data.Columns.Add("Guid", typeof(Guid));
            data.Columns.Add("Name", typeof(string));
            data.Columns.Add("Birth", typeof(DateTime));
            data.Columns.Add("Telephone", typeof(string)).AllowDBNull = true;
            data.Columns.Add("Address", typeof(string));
            data.Columns.Add("Height", typeof(double)).AllowDBNull = true;
            data.Columns.Add("Weight", typeof(double)).AllowDBNull = true;
            var guid = new Guid("fd752796-8c8e-4f87-8efd-b982d3d28bcb");
            var date = new DateTime(2022, 1, 23, 12, 34, 56, 789);
            data.Rows.Add(1, guid, "AAAA", date, "81-12-345-6789", "Uminokuchi. Taira. Omachi. Nagano 398-0001", 123.4, 234.5);

            using var reader = data.CreateDataReader();
            Assert.IsTrue(reader.Read());

            var record = new FieldDerivedReferenceType();

            var context = new DynamicRecordInjectionContext<FieldDerivedReferenceType>(
                ConversionContext.Default, StringComparer.OrdinalIgnoreCase, reader);

            context.Inject(ref record);

            return Verify($"{record.Id},{record.Guid},{record.Name},{record.Birth.ToString(CultureInfo.InvariantCulture)},{record.Telephone},{record.Address},{record.Weight},{record.Age}");
        }
    }
}