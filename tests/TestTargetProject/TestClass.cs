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
using System.Linq;

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

        public static TargetValueTypes[] InjectExecuteNonQueryWithValueType()
        {
            using var connection = TestTableGenerator.CreateTestTable();

            var query = connection.Query<TargetValueTypes>("SELECT * FROM target");
            var r = query.ExecuteNonParameterized().ToArray();

            return r;
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

        public static TargetReferenceTypes[] InjectExecuteNonQueryWithReferenceType()
        {
            using var connection = TestTableGenerator.CreateTestTable();

            var query = connection.Query<TargetReferenceTypes>("SELECT * FROM target");
            var r = query.ExecuteNonParameterized().ToArray();

            return r;
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
