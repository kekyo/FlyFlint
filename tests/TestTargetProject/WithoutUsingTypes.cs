////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint;
using System;

namespace TestTargetProject
{
    public sealed class WithoutUsingTypes
    {
        public enum EnumValue
        {
            ValueA = 1,
            ValueB = 4,
            ValueC = 7,
            ValueD = 13,
        }

        [QueryRecord]
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

        [QueryRecord]
        public class TargetReferenceTypesDerived2 : TargetReferenceTypesDerived1
        {
            public bool Value31;
            public byte Value32;
            public short Value33;
            public int Value34;
            public long Value35;

            public override void foo()
            {
                base.foo();
            }
        }

        [QueryRecord]
        public class TargetReferenceTypesDerived1 : TargetReferenceTypesBase
        {
            public bool Value21;
            public byte Value22;
            public short Value23;
            public int Value24;
            public long Value25;

            public override void foo()
            {
                base.foo();
            }
        }

        public interface IHoge
        {
            void foo();
        }

        [QueryRecord]
        public class TargetReferenceTypesBase : IHoge
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

            public virtual void foo()
            {
            }
        }
    }
}
