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
    internal struct QueryBuilderResult
    {
        public readonly string sql;
        public readonly KeyValuePair<string, object?>[] parameters;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public QueryBuilderResult(string sql, KeyValuePair<string, object?>[] parameters)
        {
            this.sql = sql;
            this.parameters = parameters;
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Deconstruct(out string sql, out KeyValuePair<string, object?>[] parameters)
        {
            sql = this.sql;
            parameters = this.parameters;
        }
    }

    public abstract class PreparedQueryContext
    {
        internal readonly ConversionContext cc;
        internal readonly Func<QueryBuilderResult> builder;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private protected PreparedQueryContext(
            ConversionContext cc,
            Func<QueryBuilderResult> builder)
        {
            this.cc = cc;
            this.builder = builder;
        }
    }

    public abstract class PreparedQueryContext<TElement>
        where TElement : new()
    {
        internal readonly ConversionContext cc;
        internal readonly IComparer<string> fieldComparer;
        internal readonly Func<QueryBuilderResult> builder;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private protected PreparedQueryContext(
            ConversionContext cc,
            IComparer<string> fieldComparer,
            Func<QueryBuilderResult> builder)
        {
            this.cc = cc;
            this.fieldComparer = fieldComparer;
            this.builder = builder;
        }
    }
}
