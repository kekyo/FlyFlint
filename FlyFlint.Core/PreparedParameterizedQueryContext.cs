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
    public sealed class PreparedParameterizedQueryContext : PreparedQueryContext
    {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal PreparedParameterizedQueryContext(
            ConversionContext cc,
            Func<QueryBuilderResult> builder) :
            base(cc, builder)
        {
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedParameterizedQueryContext Conversion(ConversionContext cc) =>
            new PreparedParameterizedQueryContext(
                cc,
                this.builder);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedParameterizedQueryContext<TElement> Typed<TElement>()
            where TElement : new() =>
            new PreparedParameterizedQueryContext<TElement>(
                this.cc,
                Query.defaultFieldComparer,
                this.builder);
    }

    public sealed class PreparedParameterizedQueryContext<TElement> : PreparedQueryContext<TElement>
        where TElement : new()
    {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal PreparedParameterizedQueryContext(
            ConversionContext cc,
            IComparer<string> fieldComparer,
            Func<QueryBuilderResult> builder) :
            base(cc, fieldComparer, builder)
        {
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedParameterizedQueryContext<TElement> Conversion(ConversionContext cc) =>
            new PreparedParameterizedQueryContext<TElement>(
                cc,
                this.fieldComparer,
                this.builder);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedParameterizedQueryContext<TElement> FieldComparer(IComparer<string> fieldComparer) =>
            new PreparedParameterizedQueryContext<TElement>(
                this.cc,
                fieldComparer,
                this.builder);
    }
}
