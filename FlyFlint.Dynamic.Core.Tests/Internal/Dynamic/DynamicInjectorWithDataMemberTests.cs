﻿////////////////////////////////////////////////////////////////////////////
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
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using static VerifyNUnit.Verifier;

namespace FlyFlint.Internal.Dynamic
{
    public sealed class DynamicInjectorWithDataMemberTests
    {
        [DataContract]
        public struct FieldValueType
        {
            [DataMember]
            public DateTime Birth;
            public string? Name;
            [DataMember]
            public int Id;
        }

        [Test]
        public Task InjectToFieldValueType()
        {
            var data = new DataTable();
            data.Columns.Add("Id", typeof(int));
            data.Columns.Add("Name", typeof(string));
            data.Columns.Add("Birth", typeof(DateTime));
            data.Rows.Add(1, "AAAA", new DateTime(2022, 1, 23, 12, 34, 56, 789));

            using var reader = data.CreateDataReader();
            Assert.IsTrue(reader.Read());

            var context = new DataInjectionContext(reader, CultureInfo.InvariantCulture, Encoding.UTF8);
            var injector = new DynamicInjector<FieldValueType>(context);

            var element = new FieldValueType();

            injector.Inject(ref element);

            return Verify($"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)}");
        }

        [DataContract]
        public sealed class FieldReferenceType
        {
            [DataMember]
            public DateTime Birth;
            public string? Name;
            [DataMember]
            public int Id;
        }

        [Test]
        public Task InjectToFieldReferenceType()
        {
            var data = new DataTable();
            data.Columns.Add("Id", typeof(int));
            data.Columns.Add("Name", typeof(string));
            data.Columns.Add("Birth", typeof(DateTime));
            data.Rows.Add(1, "AAAA", new DateTime(2022, 1, 23, 12, 34, 56, 789));

            using var reader = data.CreateDataReader();
            Assert.IsTrue(reader.Read());

            var context = new DataInjectionContext(reader, CultureInfo.InvariantCulture, Encoding.UTF8);
            var injector = new DynamicInjector<FieldReferenceType>(context);

            var element = new FieldReferenceType();

            injector.Inject(ref element);

            return Verify($"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)}");
        }

        [DataContract]
        public struct PropertyValueType
        {
            [DataMember]
            public DateTime Birth { get; set; }
            public string? Name { get; set; }
            [DataMember]
            public int Id { get; set; }
        }

        [Test]
        public Task InjectToPropertyValueType()
        {
            var data = new DataTable();
            data.Columns.Add("Id", typeof(int));
            data.Columns.Add("Name", typeof(string));
            data.Columns.Add("Birth", typeof(DateTime));
            data.Rows.Add(1, "AAAA", new DateTime(2022, 1, 23, 12, 34, 56, 789));

            using var reader = data.CreateDataReader();
            Assert.IsTrue(reader.Read());

            var context = new DataInjectionContext(reader, CultureInfo.InvariantCulture, Encoding.UTF8);
            var injector = new DynamicInjector<PropertyValueType>(context);

            var element = new PropertyValueType();

            injector.Inject(ref element);

            return Verify($"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)}");
        }

        [DataContract]
        public sealed class PropertyReferenceType
        {
            [DataMember]
            public DateTime Birth { get; set; }
            public string? Name { get; set; }
            [DataMember]
            public int Id { get; set; }
        }

        [Test]
        public Task InjectToPropertyReferenceType()
        {
            var data = new DataTable();
            data.Columns.Add("Id", typeof(int));
            data.Columns.Add("Name", typeof(string));
            data.Columns.Add("Birth", typeof(DateTime));
            data.Rows.Add(1, "AAAA", new DateTime(2022, 1, 23, 12, 34, 56, 789));

            using var reader = data.CreateDataReader();
            Assert.IsTrue(reader.Read());

            var context = new DataInjectionContext(reader, CultureInfo.InvariantCulture, Encoding.UTF8);
            var injector = new DynamicInjector<PropertyReferenceType>(context);

            var element = new PropertyReferenceType();

            injector.Inject(ref element);

            return Verify($"{element.Id},{element.Name},{element.Birth.ToString(CultureInfo.InvariantCulture)}");
        }
    }
}