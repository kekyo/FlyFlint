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
using System.Globalization;
using System.Text;
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

            private static readonly (string, Type)[] members = new[]
            {
                (nameof(Value1), typeof(bool)),
                (nameof(Value2), typeof(byte)),
                (nameof(Value3), typeof(short)),
                (nameof(Value4), typeof(int)),
                (nameof(Value5), typeof(long)),
                (nameof(Value6), typeof(float)),
                (nameof(Value7), typeof(double)),
                (nameof(Value8), typeof(decimal)),
                (nameof(Value9), typeof(Guid)),
                (nameof(Value10), typeof(DateTime)),
                (nameof(Value11), typeof(EnumValue)),
                (nameof(Value12), typeof(EnumValue)),
                (nameof(Value13), typeof(EnumValue)),
                (nameof(Value14), typeof(EnumValue)),
                (nameof(Value15), typeof(string)),
            };

            public DataInjectionMetadata[] PrepareAndInject(DataInjectionContext context)
            {
                var metadataList = StaticInjectonHelper<TargetValueTypes>.Prepare(context, members);
                this.Inject(context, metadataList);
                return metadataList;
            }

            public void Inject(DataInjectionContext context, DataInjectionMetadata[] metadataList)
            {
                this.Value1 = StaticDataAccessor.GetBoolean(context, metadataList[0]);
                this.Value2 = StaticDataAccessor.GetByte(context, metadataList[1]);
                this.Value3 = StaticDataAccessor.GetInt16(context, metadataList[2]);
                this.Value4 = StaticDataAccessor.GetInt32(context, metadataList[3]);
                this.Value5 = StaticDataAccessor.GetInt64(context, metadataList[4]);
                this.Value6 = StaticDataAccessor.GetSingle(context, metadataList[5]);
                this.Value7 = StaticDataAccessor.GetDouble(context, metadataList[6]);
                this.Value8 = StaticDataAccessor.GetDecimal(context, metadataList[7]);
                this.Value9 = StaticDataAccessor.GetGuid(context, metadataList[8]);
                this.Value10 = StaticDataAccessor.GetDateTime(context, metadataList[9]);
                this.Value11 = StaticDataAccessor.GetEnum<EnumValue>(context, metadataList[10]);
                this.Value12 = StaticDataAccessor.GetEnum<EnumValue>(context, metadataList[11]);
                this.Value13 = StaticDataAccessor.GetEnum<EnumValue>(context, metadataList[12]);
                this.Value14 = StaticDataAccessor.GetEnum<EnumValue>(context, metadataList[13]);
                this.Value15 = StaticDataAccessor.GetString(context, metadataList[14]);
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

            var context = new DataInjectionContext(reader, CultureInfo.InvariantCulture, Encoding.UTF8);
            var metadataList = element.PrepareAndInject(context);

            element.Inject(context, metadataList);

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

            private static readonly (string, Type)[] members = new[]
            {
                (nameof(Value1), typeof(bool)),
                (nameof(Value2), typeof(byte)),
                (nameof(Value3), typeof(short)),
                (nameof(Value4), typeof(int)),
                (nameof(Value5), typeof(long)),
                (nameof(Value6), typeof(float)),
                (nameof(Value7), typeof(double)),
                (nameof(Value8), typeof(decimal)),
                (nameof(Value9), typeof(Guid)),
                (nameof(Value10), typeof(DateTime)),
                (nameof(Value11), typeof(EnumValue)),
                (nameof(Value12), typeof(EnumValue)),
                (nameof(Value13), typeof(EnumValue)),
                (nameof(Value14), typeof(EnumValue)),
                (nameof(Value15), typeof(string)),
            };

            public DataInjectionMetadata[] PrepareAndInject(DataInjectionContext context)
            {
                var metadataList = StaticInjectonHelper<TargetValueTypes>.Prepare(context, members);
                this.Inject(context, metadataList);
                return metadataList;
            }

            public void Inject(DataInjectionContext context, DataInjectionMetadata[] metadataList)
            {
                this.Value1 = StaticDataAccessor.GetNullableBoolean(context, metadataList[0]);
                this.Value2 = StaticDataAccessor.GetNullableByte(context, metadataList[1]);
                this.Value3 = StaticDataAccessor.GetNullableInt16(context, metadataList[2]);
                this.Value4 = StaticDataAccessor.GetNullableInt32(context, metadataList[3]);
                this.Value5 = StaticDataAccessor.GetNullableInt64(context, metadataList[4]);
                this.Value6 = StaticDataAccessor.GetNullableSingle(context, metadataList[5]);
                this.Value7 = StaticDataAccessor.GetNullableDouble(context, metadataList[6]);
                this.Value8 = StaticDataAccessor.GetNullableDecimal(context, metadataList[7]);
                this.Value9 = StaticDataAccessor.GetNullableGuid(context, metadataList[8]);
                this.Value10 = StaticDataAccessor.GetNullableDateTime(context, metadataList[9]);
                this.Value11 = StaticDataAccessor.GetNullableEnum<EnumValue>(context, metadataList[10]);
                this.Value12 = StaticDataAccessor.GetNullableEnum<EnumValue>(context, metadataList[11]);
                this.Value13 = StaticDataAccessor.GetNullableEnum<EnumValue>(context, metadataList[12]);
                this.Value14 = StaticDataAccessor.GetNullableEnum<EnumValue>(context, metadataList[13]);
                this.Value15 = StaticDataAccessor.GetNullableString(context, metadataList[14]);
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

            var context = new DataInjectionContext(reader, CultureInfo.InvariantCulture, Encoding.UTF8);
            var metadataList = element.PrepareAndInject(context);

            element.Inject(context, metadataList);

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

            var context = new DataInjectionContext(reader, CultureInfo.InvariantCulture, Encoding.UTF8);
            var metadataList = element.PrepareAndInject(context);

            element.Inject(context, metadataList);

            return Verify($"{element.Value1},{element.Value2},{element.Value3},{element.Value4},{element.Value5},{element.Value6},{element.Value7},{element.Value8},{element.Value9},{element.Value10?.ToString(CultureInfo.InvariantCulture)},{element.Value11},{element.Value12},{element.Value13},{element.Value14},{element.Value15}");
        }
    }
}