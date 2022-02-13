////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FlyFlint.Internal.Static
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class StaticParameterExtractionContext
    {
        private readonly ConversionContext cc;
        private readonly List<ExtractedParameter> parameters = new();

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal StaticParameterExtractionContext(
            ConversionContext cc) =>
            this.cc = cc;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void SetParameter<TValue>(string fieldName, ref TValue parameterValue) =>
            this.parameters.Add(new ExtractedParameter(fieldName, cc.ConvertFrom(in parameterValue, null)));

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void SetParameter<TValue>(string fieldName, TValue parameterValue) =>
            this.parameters.Add(new ExtractedParameter(fieldName, cc.ConvertFrom(in parameterValue, null)));

        internal ExtractedParameter[] ExtractParameters(string parameterPrefix)
        {
            var extracted = new ExtractedParameter[this.parameters.Count];
            for (var index = 0; index < extracted.Length; index++)
            {
                var parameter = this.parameters[index];

                var value = parameter.Value;
                var dbValue = value ?? DBNull.Value;

                extracted[index] = new ExtractedParameter(
                    parameterPrefix + parameter.Name, dbValue);
            }
            return extracted;
        }
    }
}
