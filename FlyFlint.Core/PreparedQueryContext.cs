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
    public abstract class PreparedQueryContext
    {
        internal readonly ConversionContext cc;
        internal readonly string sql;
        internal readonly Func<KeyValuePair<string, object?>[]> constructParameters;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private protected PreparedQueryContext(
            ConversionContext cc,
            string sql,
            Func<KeyValuePair<string, object?>[]> constructParameters)
        {
            this.cc = cc;
            this.sql = sql;
            this.constructParameters = constructParameters;
        }
    }

    public abstract class PreparedQueryContext<T>
        where T : new()
    {
        internal readonly ConversionContext cc;
        internal readonly IComparer<string> fieldComparer;
        internal readonly string sql;
        internal readonly Func<KeyValuePair<string, object?>[]> constructParameters;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private protected PreparedQueryContext(
            ConversionContext cc,
            IComparer<string> fieldComparer,
            string sql,
            Func<KeyValuePair<string, object?>[]> constructParameters)
        {
            this.cc = cc;
            this.fieldComparer = fieldComparer;
            this.sql = sql;
            this.constructParameters = constructParameters;
        }
    }
}
