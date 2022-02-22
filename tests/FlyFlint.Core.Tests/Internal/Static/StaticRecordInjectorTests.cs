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
    public sealed class StaticRecordInjectorTests
    {
        /////////////////////////////////////////////////////////////

        public struct FieldValueType : IRecordInjectable
        {
            public DateTime Birth;
            public string? Name;
            public int Id;
            public int Age;
            public double Weight;

            private static readonly StaticMemberMetadata[] members = new[]
            {
                new StaticMemberMetadata(nameof(Birth), typeof(DateTime)),
                new StaticMemberMetadata(nameof(Name), typeof(string)),
                new StaticMemberMetadata(nameof(Id), typeof(int)),
                new StaticMemberMetadata(nameof(Age), typeof(int)),
                new StaticMemberMetadata(nameof(Weight), typeof(double)),
            };

            private static readonly Delegate injector =
                (StaticRecordInjectorByRefDelegate<FieldValueType>)Inject;

            public void Prepare(StaticRecordInjectionContext context) =>
                context.RegisterMetadata(members, injector);

            private static void Inject(
                StaticRecordInjectionContext context, ref FieldValueType record)
            {
                var isAvailable = context.IsAvailable;
                if (isAvailable[0]) record.Birth = context.GetDateTime(0);
                if (isAvailable[1]) record.Name = context.GetNullableString(1);
                if (isAvailable[2]) record.Id = context.GetInt32(2);
                if (isAvailable[3]) record.Age = context.GetInt32(3);
                if (isAvailable[4]) record.Weight = context.GetDouble(4);
            }
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

            var record = new FieldValueType();

            var context = new StaticRecordInjectionByRefContext<FieldValueType>(
                ConversionContext.Default, StringComparer.OrdinalIgnoreCase, reader);
            record.Prepare(context);
            context.MakeInjectable();

            context.Inject(ref record);

            return Verify($"{record.Id},{record.Name},{record.Birth.ToString(CultureInfo.InvariantCulture)},{record.Weight},{record.Age}");
        }

        /////////////////////////////////////////////////////////////

        public sealed class FieldReferenceType : IRecordInjectable
        {
            public DateTime Birth;
            public string? Name;
            public int Id;
            public int Age;
            public double Weight;

            private static readonly StaticMemberMetadata[] members = new[]
            {
                new StaticMemberMetadata(nameof(Birth), typeof(DateTime)),
                new StaticMemberMetadata(nameof(Name), typeof(string)),
                new StaticMemberMetadata(nameof(Id), typeof(int)),
                new StaticMemberMetadata(nameof(Age), typeof(int)),
                new StaticMemberMetadata(nameof(Weight), typeof(double)),
            };

            private static readonly Delegate injector =
                (StaticRecordInjectorObjRefDelegate<FieldReferenceType>)Inject;

            public void Prepare(StaticRecordInjectionContext context) =>
                context.RegisterMetadata(members, injector);

            private static void Inject(
                StaticRecordInjectionContext context, FieldReferenceType record)
            {
                var isAvailable = context.IsAvailable;
                if (isAvailable[0]) record.Birth = context.GetDateTime(0);
                if (isAvailable[1]) record.Name = context.GetNullableString(1);
                if (isAvailable[2]) record.Id = context.GetInt32(2);
                if (isAvailable[3]) record.Age = context.GetInt32(3);
                if (isAvailable[4]) record.Weight = context.GetDouble(4);
            }
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

            var record = new FieldReferenceType();

            var context = new StaticRecordInjectionObjRefContext<FieldReferenceType>(
                ConversionContext.Default, StringComparer.OrdinalIgnoreCase, reader);
            record.Prepare(context);
            context.MakeInjectable();

            context.Inject(ref record);

            return Verify($"{record.Id},{record.Name},{record.Birth.ToString(CultureInfo.InvariantCulture)},{record.Weight},{record.Age}");
        }

        /////////////////////////////////////////////////////////////

        public struct PropertyValueType : IRecordInjectable
        {
            public DateTime Birth { get; set; }
            public string? Name { get; set; }
            public int Id { get; set; }
            public int Age { get; set; }
            public double Weight { get; set; }

            private static readonly StaticMemberMetadata[] members = new[]
            {
                new StaticMemberMetadata(nameof(Birth), typeof(DateTime)),
                new StaticMemberMetadata(nameof(Name), typeof(string)),
                new StaticMemberMetadata(nameof(Id), typeof(int)),
                new StaticMemberMetadata(nameof(Age), typeof(int)),
                new StaticMemberMetadata(nameof(Weight), typeof(double)),
            };

            private static readonly Delegate injector =
                (StaticRecordInjectorByRefDelegate<PropertyValueType>)Inject;

            public void Prepare(StaticRecordInjectionContext context) =>
                context.RegisterMetadata(members, injector);

            private static void Inject(
                StaticRecordInjectionContext context, ref PropertyValueType record)
            {
                var isAvailable = context.IsAvailable;
                if (isAvailable[0]) record.Birth = context.GetDateTime(0);
                if (isAvailable[1]) record.Name = context.GetNullableString(1);
                if (isAvailable[2]) record.Id = context.GetInt32(2);
                if (isAvailable[3]) record.Age = context.GetInt32(3);
                if (isAvailable[4]) record.Weight = context.GetDouble(4);
            }
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

            var record = new PropertyValueType();

            var context = new StaticRecordInjectionByRefContext<PropertyValueType>(
                ConversionContext.Default, StringComparer.OrdinalIgnoreCase, reader);
            record.Prepare(context);
            context.MakeInjectable();

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

            private static readonly StaticMemberMetadata[] members = new[]
            {
                new StaticMemberMetadata(nameof(Birth), typeof(DateTime)),
                new StaticMemberMetadata(nameof(Name), typeof(string)),
                new StaticMemberMetadata(nameof(Id), typeof(int)),
                new StaticMemberMetadata(nameof(Age), typeof(int)),
                new StaticMemberMetadata(nameof(Weight), typeof(double)),
            };

            private static readonly Delegate injector =
                (StaticRecordInjectorObjRefDelegate<PropertyReferenceType>)Inject;

            public void Prepare(StaticRecordInjectionContext context) =>
                context.RegisterMetadata(members, injector);

            private static void Inject(
                StaticRecordInjectionContext context, PropertyReferenceType record)
            {
                var isAvailable = context.IsAvailable;
                if (isAvailable[0]) record.Birth = context.GetDateTime(0);
                if (isAvailable[1]) record.Name = context.GetNullableString(1);
                if (isAvailable[2]) record.Id = context.GetInt32(2);
                if (isAvailable[3]) record.Age = context.GetInt32(3);
                if (isAvailable[4]) record.Weight = context.GetDouble(4);
            }
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

            var record = new PropertyReferenceType();

            var context = new StaticRecordInjectionObjRefContext<PropertyReferenceType>(
                ConversionContext.Default, StringComparer.OrdinalIgnoreCase, reader);
            record.Prepare(context);
            context.MakeInjectable();

            context.Inject(ref record);

            return Verify($"{record.Id},{record.Name},{record.Birth.ToString(CultureInfo.InvariantCulture)},{record.Weight},{record.Age}");
        }
    }
}