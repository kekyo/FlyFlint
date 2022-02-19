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
    public sealed class StaticExtractorTests
    {
        private struct ParameterValueType : IParameterExtractable
        {
            public int Id;
            public string? Name;
            public DateTime Birth;

            public void Extract(StaticParameterExtractionContext context)
            {
                context.SetByRefParameter(nameof(Id), ref this.Id);
                context.SetByValParameter(nameof(Name), this.Name);
                context.SetByRefParameter(nameof(Birth), ref this.Birth);
            }
        }

        [Test]
        public Task ExtractFromValueType()
        {
            var parameter = new ParameterValueType
            {
                Id = 123,
                Name = "ABC",
                Birth = new DateTime(2022, 1, 23, 12, 34, 56, 789),
            };

            var context = new StaticParameterExtractionContext(
                ConversionContext.Default);

            parameter.Extract(context);

            var extracted = context.ExtractParameters("A");

            return Verify(string.Join(
                Environment.NewLine,
                extracted.Select(pair => $"{pair.Name}={Convert.ToString(pair.Value, CultureInfo.InvariantCulture)}")));
        }

        [QueryParameter]
        private sealed class ParametertReferenceType : IParameterExtractable
        {
            public int Id;
            public string? Name;
            public DateTime Birth;

            public void Extract(StaticParameterExtractionContext context)
            {
                context.SetByRefParameter(nameof(Id), ref this.Id);
                context.SetByValParameter(nameof(Name), this.Name);
                context.SetByRefParameter(nameof(Birth), ref this.Birth);
            }
        }

        [Test]
        public Task ExtractFromReferenceType()
        {
            var parameter = new ParametertReferenceType
            {
                Id = 123,
                Name = "ABC",
                Birth = new DateTime(2022, 1, 23, 12, 34, 56, 789),
            };

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