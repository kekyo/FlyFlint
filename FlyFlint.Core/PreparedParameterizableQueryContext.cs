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
using System.Runtime.CompilerServices;

namespace FlyFlint
{
    public sealed class PreparedParameterizableQueryContext : PreparedQueryContext
    {
        internal readonly string parameterPrefix;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal PreparedParameterizableQueryContext(
            ConversionContext cc,
            Func<QueryBuilderResult> builder,
            string parameterPrefix) :
            base(cc, builder) =>
            this.parameterPrefix = parameterPrefix;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedParameterizableQueryContext Prefix(string parameterPrefix) =>
            new PreparedParameterizableQueryContext(
                this.cc,
                this.builder, 
                parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedParameterizableQueryContext Conversion(ConversionContext cc) =>
            new PreparedParameterizableQueryContext(
                cc,
                this.builder,
                this.parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedParameterizableQueryContext<TElement> Typed<TElement>()
            where TElement : new() =>
            new PreparedParameterizableQueryContext<TElement>(
                this.cc,
                Query.defaultFieldComparer,
                this.builder,
                this.parameterPrefix);
    }

    public sealed class PreparedParameterizableQueryContext<TElement> : PreparedQueryContext<TElement>
        where TElement : new()
    {
        internal readonly string parameterPrefix;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal PreparedParameterizableQueryContext(
            ConversionContext cc,
            IComparer<string> fieldComparer,
            Func<QueryBuilderResult> builder,
            string parameterPrefix) :
            base(cc, fieldComparer, builder) =>
            this.parameterPrefix = parameterPrefix;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedParameterizableQueryContext<TElement> Prefix(string parameterPrefix) =>
            new PreparedParameterizableQueryContext<TElement>(
                this.cc,
                this.fieldComparer,
                this.builder,
                parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedParameterizableQueryContext<TElement> Conversion(ConversionContext cc) =>
            new PreparedParameterizableQueryContext<TElement>(
                cc,
                this.fieldComparer,
                this.builder,
                this.parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedParameterizableQueryContext<TElement> FieldComparer(IComparer<string> fieldComparer) =>
            new PreparedParameterizableQueryContext<TElement>(
                this.cc,
                fieldComparer,
                this.builder,
                this.parameterPrefix);
    }
}
