////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Internal;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FlyFlint.Context
{
    public sealed class PreparedPartialQueryContext : PreparedQueryContext
    {
        internal readonly string parameterPrefix;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal PreparedPartialQueryContext(
            ConversionContext cc,
            Func<QueryParameterBuilderResult> builder,
            string parameterPrefix) :
            base(cc, builder) =>
            this.parameterPrefix = parameterPrefix;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedPartialQueryContext Prefix(string parameterPrefix) =>
            new PreparedPartialQueryContext(
                this.cc,
                this.builder, 
                parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedPartialQueryContext Conversion(ConversionContext cc) =>
            new PreparedPartialQueryContext(
                cc,
                this.builder,
                this.parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedPartialQueryContext<TElement> Typed<TElement>()
            where TElement : new() =>
            new PreparedPartialQueryContext<TElement>(
                this.cc,
                Query.defaultFieldComparer,
                this.builder,
                this.parameterPrefix);
    }

    public sealed class PreparedPartialQueryContext<TElement> : PreparedQueryContext<TElement>
        where TElement : new()
    {
        internal readonly string parameterPrefix;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal PreparedPartialQueryContext(
            ConversionContext cc,
            IComparer<string> fieldComparer,
            Func<QueryParameterBuilderResult> builder,
            string parameterPrefix) :
            base(cc, fieldComparer, builder) =>
            this.parameterPrefix = parameterPrefix;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedPartialQueryContext<TElement> Prefix(string parameterPrefix) =>
            new PreparedPartialQueryContext<TElement>(
                this.cc,
                this.fieldComparer,
                this.builder,
                parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedPartialQueryContext<TElement> Conversion(ConversionContext cc) =>
            new PreparedPartialQueryContext<TElement>(
                cc,
                this.fieldComparer,
                this.builder,
                this.parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedPartialQueryContext<TElement> FieldComparer(IComparer<string> fieldComparer) =>
            new PreparedPartialQueryContext<TElement>(
                this.cc,
                fieldComparer,
                this.builder,
                this.parameterPrefix);
    }
}
