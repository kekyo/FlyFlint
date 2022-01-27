////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint;
using FlyFlint.Synchronized;
using System;
using System.Data;
using System.Data.SQLite;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace TestTargetProject
{
    public sealed class TestClass
    {
        public enum EnumValue
        {
            ValueA = 1,
            ValueB = 4,
            ValueC = 7,
            ValueD = 13,
        }

        public struct TargetValueTypes
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
            //public EnumValue Value11;
            //public EnumValue Value12;
            public string Value13;
        }

        private async Task<SQLiteConnection> CreateTestTableAsync()
        {
            var connection = new SQLiteConnection("Data Source=:memory:");
            await connection.OpenAsync();

            var c = connection.CreateCommand();
            c.CommandType = CommandType.Text;
            c.CommandText = "CREATE TABLE target (Value1 TEXT,Value2 INTEGER,Value3 INTEGER,Value4 INTEGER,Value5 INTEGER,Value6 REAL,Value7 REAL,Value8 REAL,Value9 TEXT,Value10 TEXT,Value11 INTEGER,Value12 TEXT,Value13 TEXT)";
            await c.ExecuteNonQueryAsync();

            c.CommandText = "INSERT INTO target VALUES ('TRUE',123,12345,12345678,123456789012345,123.45,123.45678901234,123.45678901234,'13B3385A-4D34-42F0-BBAD-6D900A66F191','01/18/2022 12:34:56.789','ValueB',7,'ABC')";
            await c.ExecuteNonQueryAsync();
            c.CommandText = "INSERT INTO target VALUES ('FALSE',124,12346,12345679,123456789012346,123.46,123.45678901235,123.45678901235,'13B3385A-4D34-42F0-BBAD-6D900A66F192','01/18/2022 12:34:56.780','ValueC',8,'ABD')";
            await c.ExecuteNonQueryAsync();
            c.CommandText = "INSERT INTO target VALUES ('TRUE',125,12347,12345670,123456789012347,123.47,123.45678901236,123.45678901236,'13B3385A-4D34-42F0-BBAD-6D900A66F193','01/18/2022 12:34:56.781','ValueD',11,'ABE')";
            await c.ExecuteNonQueryAsync();

            return connection;
        }

        public async Task InjectExecuteNonQueryWithValueType()
        {
            using var connection = await CreateTestTableAsync();

            var query = connection.Query<TargetValueTypes>("SELECT * FROM target");
            var result = query.Execute();
        }

        public class TargetReferenceTypes
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
            //public EnumValue Value11;
            //public EnumValue Value12;
            public string Value13 = null!;
        }

        public async Task InjectExecuteNonQueryWithReferenceType()
        {
            using var connection = await CreateTestTableAsync();

            var query = connection.Query<TargetReferenceTypes>("SELECT * FROM target");
            var result = query.Execute();
        }

        [DataContract]
        public struct TargetValueTypesWithoutUsing
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
            //public EnumValue Value11;
            //public EnumValue Value12;
            public string Value13;
        }

        [DataContract]
        public class TargetReferenceTypesDerived2 : TargetReferenceTypesDerived1
        {
            public bool Value31;
            public byte Value32;
            public short Value33;
            public int Value34;
            public long Value35;

            protected override void foo()
            {
                base.foo();
            }
        }

        [DataContract]
        public class TargetReferenceTypesDerived1 : TargetReferenceTypesBase
        {
            public bool Value21;
            public byte Value22;
            public short Value23;
            public int Value24;
            public long Value25;

            protected override void foo()
            {
                base.foo();
            }
        }

        [DataContract]
        public class TargetReferenceTypesBase
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
            //public EnumValue Value11;
            //public EnumValue Value12;
            public string Value13 = null!;

            protected virtual void foo()
            {
            }
        }


#if false
        public async Task InjectValueTypeVaries()
        {
            using var connection = await CreateTestTableAsync();

            var query = connection.Query<TargetValueTypes>("SELECT * FROM target");
            var targets = await query.ExecuteAsync().ToArrayAsync();
        }

        public struct TargetNullableValueTypes
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
            public string? Value13;
        }

        public async Task InjectValuedNullableValueTypeVaries()
        {
            using var connection = await CreateTestTableAsync();

            var query = connection.Query<TargetNullableValueTypes>("SELECT * FROM target");
            var targets = await query.ExecuteAsync().ToArrayAsync();
        }
#endif
    }
}
