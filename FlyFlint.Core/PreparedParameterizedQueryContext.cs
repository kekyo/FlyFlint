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
            string sql,
            Func<KeyValuePair<string, object?>[]> constructParameters) :
            base(cc, sql, constructParameters)
        {
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedParameterizedQueryContext Conversion(ConversionContext cc) =>
            new PreparedParameterizedQueryContext(
                cc,
                this.sql,
                this.constructParameters);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedParameterizedQueryContext<T> Typed<T>()
            where T : new() =>
            new PreparedParameterizedQueryContext<T>(
                this.cc,
                Query.defaultFieldComparer,
                this.sql,
                this.constructParameters);
    }

    public sealed class PreparedParameterizedQueryContext<T> : PreparedQueryContext<T>
        where T : new()
    {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal PreparedParameterizedQueryContext(
            ConversionContext cc,
            IComparer<string> fieldComparer,
            string sql,
            Func<KeyValuePair<string, object?>[]> constructParameters) :
            base(cc, fieldComparer, sql, constructParameters)
        {
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedParameterizedQueryContext<T> Conversion(ConversionContext cc) =>
            new PreparedParameterizedQueryContext<T>(
                cc,
                this.fieldComparer,
                this.sql,
                this.constructParameters);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedParameterizedQueryContext<T> FieldComparer(IComparer<string> fieldComparer) =>
            new PreparedParameterizedQueryContext<T>(
                this.cc,
                fieldComparer,
                this.sql,
                this.constructParameters);
    }
}
