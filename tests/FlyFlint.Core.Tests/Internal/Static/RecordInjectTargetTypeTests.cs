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
    public sealed class RecordInjectTargetTypeTests
    {
        public enum EnumValue
        {
            ValueA = 1,
            ValueB = 4,
            ValueC = 7,
            ValueD = 13,
        }

        public struct TargetValueTypes : IRecordInjectable
        {
            public bool Value1;
            public byte Value2;
            public short Value3;
            public int Value4;
            public long Value5;
            public float Value6;
            public double Value7;
            public decimal Value8;
            public Guid Value9;
            public DateTime Value10;
            public EnumValue Value11;
            public EnumValue Value12;
            public EnumValue Value13;
            public EnumValue Value14;
            public string Value15;

            private static readonly StaticMemberMetadata[] members = new[]
            {
                new StaticMemberMetadata(nameof(Value1), typeof(bool)),
                new StaticMemberMetadata(nameof(Value2), typeof(byte)),
                new StaticMemberMetadata(nameof(Value3), typeof(short)),
                new StaticMemberMetadata(nameof(Value4), typeof(int)),
                new StaticMemberMetadata(nameof(Value5), typeof(long)),
                new StaticMemberMetadata(nameof(Value6), typeof(float)),
                new StaticMemberMetadata(nameof(Value7), typeof(double)),
                new StaticMemberMetadata(nameof(Value8), typeof(decimal)),
                new StaticMemberMetadata(nameof(Value9), typeof(Guid)),
                new StaticMemberMetadata(nameof(Value10), typeof(DateTime)),
                new StaticMemberMetadata(nameof(Value11), typeof(EnumValue)),
                new StaticMemberMetadata(nameof(Value12), typeof(EnumValue)),
                new StaticMemberMetadata(nameof(Value13), typeof(EnumValue)),
                new StaticMemberMetadata(nameof(Value14), typeof(EnumValue)),
                new StaticMemberMetadata(nameof(Value15), typeof(string)),
            };

            private static readonly Delegate injector =
                (StaticRecordInjectorByRefDelegate<TargetValueTypes>)Inject;

            public void Prepare(StaticRecordInjectionContext context) =>
                context.RegisterMetadata(members, injector);

            private static void Inject(
                StaticRecordInjectionContext context, ref TargetValueTypes record)
            {
                var isAvailable = context.IsAvailable;
                if (isAvailable[0]) record.Value1 = context.GetBoolean(0);
                if (isAvailable[1]) record.Value2 = context.GetByte(1);
                if (isAvailable[2]) record.Value3 = context.GetInt16(2);
                if (isAvailable[3]) record.Value4 = context.GetInt32(3);
                if (isAvailable[4]) record.Value5 = context.GetInt64(4);
                if (isAvailable[5]) record.Value6 = context.GetSingle(5);
                if (isAvailable[6]) record.Value7 = context.GetDouble(6);
                if (isAvailable[7]) record.Value8 = context.GetDecimal(7);
                if (isAvailable[8]) record.Value9 = context.GetGuid(8);
                if (isAvailable[9]) record.Value10 = context.GetDateTime(9);
                if (isAvailable[10]) record.Value11 = context.GetValue<EnumValue>(10);
                if (isAvailable[11]) record.Value12 = context.GetValue<EnumValue>(11);
                if (isAvailable[12]) record.Value13 = context.GetValue<EnumValue>(12);
                if (isAvailable[13]) record.Value14 = context.GetValue<EnumValue>(13);
                if (isAvailable[14]) record.Value15 = context.GetString(14);
            }
        }

        [Test]
        public Task InjectValueTypeVaries()
        {
            var data = new DataTable();
            data.Columns.Add("Value1", typeof(bool));
            data.Columns.Add("Value2", typeof(byte));
            data.Columns.Add("Value3", typeof(short));
            data.Columns.Add("Value4", typeof(int));
            data.Columns.Add("Value5", typeof(long));
            data.Columns.Add("Value6", typeof(float));
            data.Columns.Add("Value7", typeof(double));
            data.Columns.Add("Value8", typeof(decimal));
            data.Columns.Add("Value9", typeof(Guid));
            data.Columns.Add("Value10", typeof(DateTime));
            data.Columns.Add("Value11", typeof(int));
            data.Columns.Add("Value12", typeof(string));
            data.Columns.Add("Value13", typeof(string));
            data.Columns.Add("Value14", typeof(int));
            data.Columns.Add("Value15", typeof(string));
            var guid = new Guid("fd752796-8c8e-4f87-8efd-b982d3d28bcb");
            var date = new DateTime(2022, 1, 23, 12, 34, 56, 789);
            data.Rows.Add(true, 111, 222, 333, 444, 555.55f, 666.66, 777.77m, guid, date, 4, "7", "ValueD", 3, "ABCD");

            using var reader = data.CreateDataReader();
            Assert.IsTrue(reader.Read());

            var record = new TargetValueTypes();

            var context = new StaticRecordInjectionByRefContext<TargetValueTypes>(
                ConversionContext.Default, StringComparer.OrdinalIgnoreCase, reader);
            record.Prepare(context);
            context.MakeInjectable();

            context.Inject(ref record);

            return Verify($"{record.Value1},{record.Value2},{record.Value3},{record.Value4},{record.Value5},{record.Value6},{record.Value7},{record.Value8},{record.Value9},{record.Value10.ToString(CultureInfo.InvariantCulture)},{record.Value11},{record.Value12},{record.Value13},{record.Value14},{record.Value15}");
        }

        public struct TargetNullableValueTypes : IRecordInjectable
        {
            public bool? Value1;
            public byte? Value2;
            public short? Value3;
            public int? Value4;
            public long? Value5;
            public float? Value6;
            public double? Value7;
            public decimal? Value8;
            public Guid? Value9;
            public DateTime? Value10;
            public EnumValue? Value11;
            public EnumValue? Value12;
            public EnumValue? Value13;
            public EnumValue? Value14;
            public string? Value15;

            private static readonly StaticMemberMetadata[] members = new[]
            {
                new StaticMemberMetadata(nameof(Value1), typeof(bool)),
                new StaticMemberMetadata(nameof(Value2), typeof(byte)),
                new StaticMemberMetadata(nameof(Value3), typeof(short)),
                new StaticMemberMetadata(nameof(Value4), typeof(int)),
                new StaticMemberMetadata(nameof(Value5), typeof(long)),
                new StaticMemberMetadata(nameof(Value6), typeof(float)),
                new StaticMemberMetadata(nameof(Value7), typeof(double)),
                new StaticMemberMetadata(nameof(Value8), typeof(decimal)),
                new StaticMemberMetadata(nameof(Value9), typeof(Guid)),
                new StaticMemberMetadata(nameof(Value10), typeof(DateTime)),
                new StaticMemberMetadata(nameof(Value11), typeof(EnumValue)),
                new StaticMemberMetadata(nameof(Value12), typeof(EnumValue)),
                new StaticMemberMetadata(nameof(Value13), typeof(EnumValue)),
                new StaticMemberMetadata(nameof(Value14), typeof(EnumValue)),
                new StaticMemberMetadata(nameof(Value15), typeof(string)),
            };

            private static readonly Delegate injector =
                (StaticRecordInjectorByRefDelegate<TargetNullableValueTypes>)Inject;

            public void Prepare(StaticRecordInjectionContext context) =>
                context.RegisterMetadata(members, injector);

            private static void Inject(
                StaticRecordInjectionContext context, ref TargetNullableValueTypes record)
            {
                var isAvailable = context.IsAvailable;
                if (isAvailable[0]) record.Value1 = context.GetNullableBoolean(0);
                if (isAvailable[1]) record.Value2 = context.GetNullableByte(1);
                if (isAvailable[2]) record.Value3 = context.GetNullableInt16(2);
                if (isAvailable[3]) record.Value4 = context.GetNullableInt32(3);
                if (isAvailable[4]) record.Value5 = context.GetNullableInt64(4);
                if (isAvailable[5]) record.Value6 = context.GetNullableSingle(5);
                if (isAvailable[6]) record.Value7 = context.GetNullableDouble(6);
                if (isAvailable[7]) record.Value8 = context.GetNullableDecimal(7);
                if (isAvailable[8]) record.Value9 = context.GetNullableGuid(8);
                if (isAvailable[9]) record.Value10 = context.GetNullableDateTime(9);
                if (isAvailable[10]) record.Value11 = context.GetNullableValue<EnumValue>(10);
                if (isAvailable[11]) record.Value12 = context.GetNullableValue<EnumValue>(11);
                if (isAvailable[12]) record.Value13 = context.GetNullableValue<EnumValue>(12);
                if (isAvailable[13]) record.Value14 = context.GetNullableValue<EnumValue>(13);
                if (isAvailable[14]) record.Value15 = context.GetNullableString(14);
            }
        }

        [Test]
        public Task InjectValuedNullableValueTypeVaries()
        {
            var data = new DataTable();
            data.Columns.Add("Value1", typeof(bool)).AllowDBNull = true;
            data.Columns.Add("Value2", typeof(byte)).AllowDBNull = true;
            data.Columns.Add("Value3", typeof(short)).AllowDBNull = true;
            data.Columns.Add("Value4", typeof(int)).AllowDBNull = true;
            data.Columns.Add("Value5", typeof(long)).AllowDBNull = true;
            data.Columns.Add("Value6", typeof(float)).AllowDBNull = true;
            data.Columns.Add("Value7", typeof(double)).AllowDBNull = true;
            data.Columns.Add("Value8", typeof(decimal)).AllowDBNull = true;
            data.Columns.Add("Value9", typeof(Guid)).AllowDBNull = true;
            data.Columns.Add("Value10", typeof(DateTime)).AllowDBNull = true;
            data.Columns.Add("Value11", typeof(int)).AllowDBNull = true;
            data.Columns.Add("Value12", typeof(string)).AllowDBNull = true;
            data.Columns.Add("Value13", typeof(string)).AllowDBNull = true;
            data.Columns.Add("Value14", typeof(int)).AllowDBNull = true;
            data.Columns.Add("Value15", typeof(string)).AllowDBNull = true;
            var guid = new Guid("fd752796-8c8e-4f87-8efd-b982d3d28bcb");
            var date = new DateTime(2022, 1, 23, 12, 34, 56, 789);
            data.Rows.Add(true, 111, 222, 333, 444, 555.55f, 666.66, 777.77m, guid, date, 4, "7", "ValueD", 3, "ABCD");

            using var reader = data.CreateDataReader();
            Assert.IsTrue(reader.Read());

            var record = new TargetNullableValueTypes();

            var context = new StaticRecordInjectionByRefContext<TargetNullableValueTypes>(
                ConversionContext.Default, StringComparer.OrdinalIgnoreCase, reader);
            record.Prepare(context);
            context.MakeInjectable();

            context.Inject(ref record);

            return Verify($"{record.Value1},{record.Value2},{record.Value3},{record.Value4},{record.Value5},{record.Value6},{record.Value7},{record.Value8},{record.Value9},{record.Value10?.ToString(CultureInfo.InvariantCulture)},{record.Value11},{record.Value12},{record.Value13},{record.Value14},{record.Value15}");
        }

        [Test]
        public Task InjectNulledNullableValueTypeVaries()
        {
            var data = new DataTable();
            data.Columns.Add("Value1", typeof(bool)).AllowDBNull = true;
            data.Columns.Add("Value2", typeof(byte)).AllowDBNull = true;
            data.Columns.Add("Value3", typeof(short)).AllowDBNull = true;
            data.Columns.Add("Value4", typeof(int)).AllowDBNull = true;
            data.Columns.Add("Value5", typeof(long)).AllowDBNull = true;
            data.Columns.Add("Value6", typeof(float)).AllowDBNull = true;
            data.Columns.Add("Value7", typeof(double)).AllowDBNull = true;
            data.Columns.Add("Value8", typeof(decimal)).AllowDBNull = true;
            data.Columns.Add("Value9", typeof(Guid)).AllowDBNull = true;
            data.Columns.Add("Value10", typeof(DateTime)).AllowDBNull = true;
            data.Columns.Add("Value11", typeof(int)).AllowDBNull = true;
            data.Columns.Add("Value12", typeof(string)).AllowDBNull = true;
            data.Columns.Add("Value13", typeof(string)).AllowDBNull = true;
            data.Columns.Add("Value14", typeof(int)).AllowDBNull = true;
            data.Columns.Add("Value15", typeof(string)).AllowDBNull = true;
            data.Rows.Add(DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value);

            using var reader = data.CreateDataReader();
            Assert.IsTrue(reader.Read());

            var record = new TargetNullableValueTypes();

            var context = new StaticRecordInjectionByRefContext<TargetNullableValueTypes>(
                ConversionContext.Default, StringComparer.OrdinalIgnoreCase, reader);
            record.Prepare(context);
            context.MakeInjectable();

            context.Inject(ref record);

            return Verify($"{record.Value1},{record.Value2},{record.Value3},{record.Value4},{record.Value5},{record.Value6},{record.Value7},{record.Value8},{record.Value9},{record.Value10?.ToString(CultureInfo.InvariantCulture)},{record.Value11},{record.Value12},{record.Value13},{record.Value14},{record.Value15}");
        }
    }
}