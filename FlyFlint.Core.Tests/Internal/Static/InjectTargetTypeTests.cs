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

        public struct TargetValueTypes : IDataInjectable<TargetValueTypes>
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

            private static readonly InjectDelegate<TargetValueTypes> injectDelegate = Inject;

            public PreparingResult<TargetValueTypes> Prepare(DataInjectionContext context) =>
                new PreparingResult<TargetValueTypes>(injectDelegate, context.Prepare(members));

            private static void Inject(
                ref TargetValueTypes element, DataInjectionContext context, DataInjectionMetadata[] metadataList)
            {
                element.Value1 = context.GetBoolean(metadataList[0]);
                element.Value2 = context.GetByte(metadataList[1]);
                element.Value3 = context.GetInt16(metadataList[2]);
                element.Value4 = context.GetInt32(metadataList[3]);
                element.Value5 = context.GetInt64(metadataList[4]);
                element.Value6 = context.GetSingle(metadataList[5]);
                element.Value7 = context.GetDouble(metadataList[6]);
                element.Value8 = context.GetDecimal(metadataList[7]);
                element.Value9 = context.GetGuid(metadataList[8]);
                element.Value10 = context.GetDateTime(metadataList[9]);
                element.Value11 = context.GetEnum<EnumValue>(metadataList[10]);
                element.Value12 = context.GetEnum<EnumValue>(metadataList[11]);
                element.Value13 = context.GetEnum<EnumValue>(metadataList[12]);
                element.Value14 = context.GetEnum<EnumValue>(metadataList[13]);
                element.Value15 = context.GetString(metadataList[14]);
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

            var context = new DataInjectionContext(
                ConversionContext.Default, StringComparer.OrdinalIgnoreCase, reader);
            var pr = element.Prepare(context);

            pr.Injector(ref element, context, pr.MetadataList);

            return Verify($"{element.Value1},{element.Value2},{element.Value3},{element.Value4},{element.Value5},{element.Value6},{element.Value7},{element.Value8},{element.Value9},{element.Value10.ToString(CultureInfo.InvariantCulture)},{element.Value11},{element.Value12},{element.Value13},{element.Value14},{element.Value15}");
        }

        public struct TargetNullableValueTypes : IDataInjectable<TargetNullableValueTypes>
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

            private static readonly InjectDelegate<TargetNullableValueTypes> injectDelegate = Inject;

            public PreparingResult<TargetNullableValueTypes> Prepare(DataInjectionContext context) =>
                new PreparingResult<TargetNullableValueTypes>(injectDelegate, context.Prepare(members));

            private static void Inject(
                ref TargetNullableValueTypes element, DataInjectionContext context, DataInjectionMetadata[] metadataList)
            {
                element.Value1 = context.GetNullableBoolean(metadataList[0]);
                element.Value2 = context.GetNullableByte(metadataList[1]);
                element.Value3 = context.GetNullableInt16(metadataList[2]);
                element.Value4 = context.GetNullableInt32(metadataList[3]);
                element.Value5 = context.GetNullableInt64(metadataList[4]);
                element.Value6 = context.GetNullableSingle(metadataList[5]);
                element.Value7 = context.GetNullableDouble(metadataList[6]);
                element.Value8 = context.GetNullableDecimal(metadataList[7]);
                element.Value9 = context.GetNullableGuid(metadataList[8]);
                element.Value10 = context.GetNullableDateTime(metadataList[9]);
                element.Value11 = context.GetNullableEnum<EnumValue>(metadataList[10]);
                element.Value12 = context.GetNullableEnum<EnumValue>(metadataList[11]);
                element.Value13 = context.GetNullableEnum<EnumValue>(metadataList[12]);
                element.Value14 = context.GetNullableEnum<EnumValue>(metadataList[13]);
                element.Value15 = context.GetNullableString(metadataList[14]);
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

            var context = new DataInjectionContext(
                ConversionContext.Default, StringComparer.OrdinalIgnoreCase, reader);
            var pr = element.Prepare(context);

            pr.Injector(ref element, context, pr.MetadataList);

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

            var context = new DataInjectionContext(
                ConversionContext.Default, StringComparer.OrdinalIgnoreCase, reader);
            var pr = element.Prepare(context);

            pr.Injector(ref element, context, pr.MetadataList);

            return Verify($"{element.Value1},{element.Value2},{element.Value3},{element.Value4},{element.Value5},{element.Value6},{element.Value7},{element.Value8},{element.Value9},{element.Value10?.ToString(CultureInfo.InvariantCulture)},{element.Value11},{element.Value12},{element.Value13},{element.Value14},{element.Value15}");
        }
    }
}