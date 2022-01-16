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
using System.Text;
using System.Threading.Tasks;
using static VerifyNUnit.Verifier;

namespace FlyFlint.Internal.Dynamic
{
    public sealed class DynamicInjectorTests
    {
        public struct FieldValueType
        {
            public DateTime Birth;
            public string? Name;
            public int Id;
            public double Weight;
            public int Age;
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

            var context = new DataInjectionContext(reader, CultureInfo.InvariantCulture, Encoding.UTF8);
            var injector = new DynamicInjector<FieldValueType>(context);

            var element = new FieldValueType();

            injector.Inject(ref element);

            return Verify($"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)},{element.Weight},{element.Age}");
        }

        public sealed class FieldReferenceType
        {
            public DateTime Birth;
            public string? Name;
            public int Id;
            public double Weight;
            public int Age;
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

            var context = new DataInjectionContext(reader, CultureInfo.InvariantCulture, Encoding.UTF8);
            var injector = new DynamicInjector<FieldReferenceType>(context);

            var element = new FieldReferenceType();

            injector.Inject(ref element);

            return Verify($"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)},{element.Weight},{element.Age}");
        }

        public struct PropertyValueType
        {
            public DateTime Birth { get; set; }
            public string? Name { get; set; }
            public int Id { get; set; }
            public double Weight { get; set; }
            public int Age { get; set; }
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

            var context = new DataInjectionContext(reader, CultureInfo.InvariantCulture, Encoding.UTF8);
            var injector = new DynamicInjector<PropertyValueType>(context);

            var element = new PropertyValueType();

            injector.Inject(ref element);

            return Verify($"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)},{element.Weight},{element.Age}");
        }

        public sealed class PropertyReferenceType
        {
            public DateTime Birth { get; set; }
            public string? Name { get; set; }
            public int Id { get; set; }
            public double Weight { get; set; }
            public int Age { get; set; }
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

            var context = new DataInjectionContext(reader, CultureInfo.InvariantCulture, Encoding.UTF8);
            var injector = new DynamicInjector<PropertyReferenceType>(context);

            var element = new PropertyReferenceType();

            injector.Inject(ref element);

            return Verify($"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)},{element.Weight},{element.Age}");
        }
    }
}