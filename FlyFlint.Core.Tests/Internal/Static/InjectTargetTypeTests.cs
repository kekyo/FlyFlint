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
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;
using static VerifyNUnit.Verifier;

namespace FlyFlint.Internal.Static
{
    public sealed class InjectTargetTypeTests
    {
        public enum EnumValue
        {
            ValueA = 1,
            ValueB = 4,
            ValueC = 7,
            ValueD = 13,
        }

        public struct TargetValueTypes : IDataInjectable
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

            private static readonly KeyValuePair<string, Type>[] members = new[]
            {
                new KeyValuePair<string, Type>(nameof(Value1), typeof(bool)),
                new KeyValuePair<string, Type>(nameof(Value2), typeof(byte)),
                new KeyValuePair<string, Type>(nameof(Value3), typeof(short)),
                new KeyValuePair<string, Type>(nameof(Value4), typeof(int)),
                new KeyValuePair<string, Type>(nameof(Value5), typeof(long)),
                new KeyValuePair<string, Type>(nameof(Value6), typeof(float)),
                new KeyValuePair<string, Type>(nameof(Value7), typeof(double)),
                new KeyValuePair<string, Type>(nameof(Value8), typeof(decimal)),
                new KeyValuePair<string, Type>(nameof(Value9), typeof(Guid)),
                new KeyValuePair<string, Type>(nameof(Value10), typeof(DateTime)),
                new KeyValuePair<string, Type>(nameof(Value11), typeof(EnumValue)),
                new KeyValuePair<string, Type>(nameof(Value12), typeof(EnumValue)),
                new KeyValuePair<string, Type>(nameof(Value13), typeof(EnumValue)),
                new KeyValuePair<string, Type>(nameof(Value14), typeof(EnumValue)),
                new KeyValuePair<string, Type>(nameof(Value15), typeof(string)),
            };

            private static readonly StaticDataInjectorDelegate<TargetValueTypes> injector = Inject;

            public void Prepare(StaticDataInjectionContext context) =>
                context.RegisterMetadata(members, injector);

            private static void Inject(
                StaticDataInjectionContext context, ref TargetValueTypes element)
            {
                element.Value1 = context.GetBoolean(0);
                element.Value2 = context.GetByte(1);
                element.Value3 = context.GetInt16(2);
                element.Value4 = context.GetInt32(3);
                element.Value5 = context.GetInt64(4);
                element.Value6 = context.GetSingle(5);
                element.Value7 = context.GetDouble(6);
                element.Value8 = context.GetDecimal(7);
                element.Value9 = context.GetGuid(8);
                element.Value10 = context.GetDateTime(9);
                element.Value11 = context.GetEnum<EnumValue>(10);
                element.Value12 = context.GetEnum<EnumValue>(11);
                element.Value13 = context.GetEnum<EnumValue>(12);
                element.Value14 = context.GetEnum<EnumValue>(13);
                element.Value15 = context.GetString(14);
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

            var element = new TargetValueTypes();

            var context = new StaticDataInjectionContext<TargetValueTypes>(
                ConversionContext.Default, StringComparer.OrdinalIgnoreCase, reader);
            element.Prepare(context);

            context.Inject(ref element);

            return Verify($"{element.Value1},{element.Value2},{element.Value3},{element.Value4},{element.Value5},{element.Value6},{element.Value7},{element.Value8},{element.Value9},{element.Value10.ToString(CultureInfo.InvariantCulture)},{element.Value11},{element.Value12},{element.Value13},{element.Value14},{element.Value15}");
        }

        public struct TargetNullableValueTypes : IDataInjectable
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

            private static readonly KeyValuePair<string, Type>[] members = new[]
            {
                new KeyValuePair<string, Type>(nameof(Value1), typeof(bool)),
                new KeyValuePair<string, Type>(nameof(Value2), typeof(byte)),
                new KeyValuePair<string, Type>(nameof(Value3), typeof(short)),
                new KeyValuePair<string, Type>(nameof(Value4), typeof(int)),
                new KeyValuePair<string, Type>(nameof(Value5), typeof(long)),
                new KeyValuePair<string, Type>(nameof(Value6), typeof(float)),
                new KeyValuePair<string, Type>(nameof(Value7), typeof(double)),
                new KeyValuePair<string, Type>(nameof(Value8), typeof(decimal)),
                new KeyValuePair<string, Type>(nameof(Value9), typeof(Guid)),
                new KeyValuePair<string, Type>(nameof(Value10), typeof(DateTime)),
                new KeyValuePair<string, Type>(nameof(Value11), typeof(EnumValue)),
                new KeyValuePair<string, Type>(nameof(Value12), typeof(EnumValue)),
                new KeyValuePair<string, Type>(nameof(Value13), typeof(EnumValue)),
                new KeyValuePair<string, Type>(nameof(Value14), typeof(EnumValue)),
                new KeyValuePair<string, Type>(nameof(Value15), typeof(string)),
            };

            private static readonly StaticDataInjectorDelegate<TargetNullableValueTypes> injector = Inject;

            public void Prepare(StaticDataInjectionContext context) =>
                context.RegisterMetadata(members, injector);

            private static void Inject(
                StaticDataInjectionContext context, ref TargetNullableValueTypes element)
            {
                element.Value1 = context.GetNullableBoolean(0);
                element.Value2 = context.GetNullableByte(1);
                element.Value3 = context.GetNullableInt16(2);
                element.Value4 = context.GetNullableInt32(3);
                element.Value5 = context.GetNullableInt64(4);
                element.Value6 = context.GetNullableSingle(5);
                element.Value7 = context.GetNullableDouble(6);
                element.Value8 = context.GetNullableDecimal(7);
                element.Value9 = context.GetNullableGuid(8);
                element.Value10 = context.GetNullableDateTime(9);
                element.Value11 = context.GetNullableEnum<EnumValue>(10);
                element.Value12 = context.GetNullableEnum<EnumValue>(11);
                element.Value13 = context.GetNullableEnum<EnumValue>(12);
                element.Value14 = context.GetNullableEnum<EnumValue>(13);
                element.Value15 = context.GetNullableString(14);
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

            var element = new TargetNullableValueTypes();

            var context = new StaticDataInjectionContext<TargetNullableValueTypes>(
                ConversionContext.Default, StringComparer.OrdinalIgnoreCase, reader);
            element.Prepare(context);

            context.Inject(ref element);

            return Verify($"{element.Value1},{element.Value2},{element.Value3},{element.Value4},{element.Value5},{element.Value6},{element.Value7},{element.Value8},{element.Value9},{element.Value10?.ToString(CultureInfo.InvariantCulture)},{element.Value11},{element.Value12},{element.Value13},{element.Value14},{element.Value15}");
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

            var element = new TargetNullableValueTypes();

            var context = new StaticDataInjectionContext<TargetNullableValueTypes>(
                ConversionContext.Default, StringComparer.OrdinalIgnoreCase, reader);
            element.Prepare(context);

            context.Inject(ref element);

            return Verify($"{element.Value1},{element.Value2},{element.Value3},{element.Value4},{element.Value5},{element.Value6},{element.Value7},{element.Value8},{element.Value9},{element.Value10?.ToString(CultureInfo.InvariantCulture)},{element.Value11},{element.Value12},{element.Value13},{element.Value14},{element.Value15}");
        }
    }
}