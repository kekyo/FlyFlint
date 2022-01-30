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
        public void RegisterParameter<T>(string fieldName, ref T parameterValue) =>
            this.parameters.Add(new ExtractedParameter(fieldName, cc.ConvertFrom(ref parameterValue)));

        public void RegisterParameter<T>(string fieldName, T parameterValue) =>
            this.parameters.Add(new ExtractedParameter(fieldName, parameterValue));

        internal ExtractedParameter[] ExtractParameters(string parameterPrefix)
        {
            var extracted = new ExtractedParameter[this.parameters.Count];
            for (var index = 0; index < extracted.Length; index++)
            {
                var value = extracted[index].Value;
                var dbValue = (value == null) ? DBNull.Value : value;

                extracted[index] = new ExtractedParameter(
                    parameterPrefix + extracted[index].Name, dbValue);
            }
            return extracted;
        }
    }
}
