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
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static VerifyNUnit.Verifier;

namespace FlyFlint.Internal.Static
{
    public sealed class ExtractParameterTypeTests
    {
        public enum EnumValue
        {
            ValueA = 1,
            ValueB = 4,
            ValueC = 7,
            ValueD = 13,
        }

        public struct ParameterValueTypes : IParameterExtractable
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
            public string Value12;

            public void Extract(StaticParameterExtractionContext context)
            {
                context.SetParameter(nameof(Value1), ref this.Value1);
                context.SetParameter(nameof(Value2), ref this.Value2);
                context.SetParameter(nameof(Value3), ref this.Value3);
                context.SetParameter(nameof(Value4), ref this.Value4);
                context.SetParameter(nameof(Value5), ref this.Value5);
                context.SetParameter(nameof(Value6), ref this.Value6);
                context.SetParameter(nameof(Value7), ref this.Value7);
                context.SetParameter(nameof(Value8), ref this.Value8);
                context.SetParameter(nameof(Value9), ref this.Value9);
                context.SetParameter(nameof(Value10), ref this.Value10);
                context.SetParameter(nameof(Value11), ref this.Value11);
                context.SetParameter(nameof(Value12), this.Value12);
            }
        }

        [Test]
        public Task ExtractValueTypeVaries()
        {
            var guid = new Guid("fd752796-8c8e-4f87-8efd-b982d3d28bcb");
            var date = new DateTime(2022, 1, 23, 12, 34, 56, 789);
            var parameter = new ParameterValueTypes
            {
                Value1 = true,
                Value2 = 111,
                Value3 = 222,
                Value4 = 333,
                Value5 = 444,
                Value6 = 555.55f,
                Value7 = 666.66,
                Value8 = 777.77m,
                Value9 = guid,
                Value10 = date,
                Value11 = EnumValue.ValueB,
                Value12 = "ABCD",
            };

            var context = new StaticParameterExtractionContext(
                ConversionContext.Default);

            parameter.Extract(context);

            var extracted = context.ExtractParameters("A");

            return Verify(string.Join(
                Environment.NewLine,
                extracted.Select(pair => $"{pair.Name}={Convert.ToString(pair.Value, CultureInfo.InvariantCulture)}")));
        }

        public struct ParameterNullableValueTypes : IParameterExtractable
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
            public string? Value12;

            public void Extract(StaticParameterExtractionContext context)
            {
                context.SetParameter(nameof(Value1), ref this.Value1);
                context.SetParameter(nameof(Value2), ref this.Value2);
                context.SetParameter(nameof(Value3), ref this.Value3);
                context.SetParameter(nameof(Value4), ref this.Value4);
                context.SetParameter(nameof(Value5), ref this.Value5);
                context.SetParameter(nameof(Value6), ref this.Value6);
                context.SetParameter(nameof(Value7), ref this.Value7);
                context.SetParameter(nameof(Value8), ref this.Value8);
                context.SetParameter(nameof(Value9), ref this.Value9);
                context.SetParameter(nameof(Value10), ref this.Value10);
                context.SetParameter(nameof(Value11), ref this.Value11);
                context.SetParameter(nameof(Value12), this.Value12);
            }
        }

        [Test]
        public Task ExtractValuedNullableValueTypeVaries()
        {
            var guid = new Guid("fd752796-8c8e-4f87-8efd-b982d3d28bcb");
            var date = new DateTime(2022, 1, 23, 12, 34, 56, 789);
            var parameter = new ParameterNullableValueTypes
            {
                Value1 = true,
                Value2 = 111,
                Value3 = 222,
                Value4 = 333,
                Value5 = 444,
                Value6 = 555.55f,
                Value7 = 666.66,
                Value8 = 777.77m,
                Value9 = guid,
                Value10 = date,
                Value11 = EnumValue.ValueB,
                Value12 = "ABCD",
            };

            var context = new StaticParameterExtractionContext(
                ConversionContext.Default);

            parameter.Extract(context);

            var extracted = context.ExtractParameters("A");

            return Verify(string.Join(
                Environment.NewLine,
                extracted.Select(pair => $"{pair.Name}={Convert.ToString(pair.Value, CultureInfo.InvariantCulture)}")));
        }

        [Test]
        public Task ExtractNulledNullableValueTypeVaries()
        {
            var parameter = new ParameterNullableValueTypes();

            var context = new StaticParameterExtractionContext(
                ConversionContext.Default);

            parameter.Extract(context);

            var extracted = context.ExtractParameters("A");

            return Verify(string.Join(
                Environment.NewLine,
                extracted.Select(pair => $"{pair.Name}={Convert.ToString(pair.Value, CultureInfo.InvariantCulture)}")));
        }
    }
}