////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Context;
using FlyFlint.Internal.Static;
using System.Runtime.CompilerServices;

namespace FlyFlint.Internal.Dynamic
{
    internal delegate void DynamicParameterExtractorByRefDelegate<TParameters>(
        StaticParameterExtractionContext context, ref TParameters parameters)
        where TParameters : notnull;  // struct on the runtime

    internal delegate void DynamicParameterExtractorObjRefDelegate<in TParameters>(
        StaticParameterExtractionContext context, TParameters parameters)
        where TParameters : notnull;  // class on the runtime

    internal abstract class DynamicParameterExtractionContext<TParameters>
        where TParameters : notnull
    {
        private protected readonly StaticParameterExtractionContext context;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private protected DynamicParameterExtractionContext(
            ConversionContext cc) =>
            this.context = new StaticParameterExtractionContext(cc);

        public abstract ExtractedParameter[] ExtractParameters(
            ref TParameters parameters, string parameterPrefix);
    }

    internal sealed class DynamicParameterExtractionByRefContext<TParameters> :
        DynamicParameterExtractionContext<TParameters>
        where TParameters : notnull
    {
        private static readonly DynamicParameterExtractorByRefDelegate<TParameters> extractor =
            DynamicParameterExtractionHelper.CreateParameterExtractor<DynamicParameterExtractorByRefDelegate<TParameters>, TParameters>();

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal DynamicParameterExtractionByRefContext(
            ConversionContext cc) :
            base(cc)
        {
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override ExtractedParameter[] ExtractParameters(
            ref TParameters parameters, string parameterPrefix)
        {
            extractor(this.context, ref parameters);
            return this.context.ExtractParameters(parameterPrefix);
        }
    }

    internal sealed class DynamicParameterExtractionObjRefContext<TParameters> :
        DynamicParameterExtractionContext<TParameters>
        where TParameters : notnull
    {
        private static readonly DynamicParameterExtractorObjRefDelegate<TParameters> extractor =
            DynamicParameterExtractionHelper.CreateParameterExtractor<DynamicParameterExtractorObjRefDelegate<TParameters>, TParameters>();

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal DynamicParameterExtractionObjRefContext(
            ConversionContext cc) :
            base(cc)
        {
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override ExtractedParameter[] ExtractParameters(
            ref TParameters parameters, string parameterPrefix)
        {
            extractor(this.context, parameters);
            return this.context.ExtractParameters(parameterPrefix);
        }
    }
}
