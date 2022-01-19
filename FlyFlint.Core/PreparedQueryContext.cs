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
    public sealed class PreparedQueryContext
    {
        internal readonly ConversionContext cc;
        internal readonly string sql;
        internal readonly Func<KeyValuePair<string, object?>[]> constructParameters;
        internal readonly string parameterPrefix;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal PreparedQueryContext(
            ConversionContext cc,
            string sql,
            Func<KeyValuePair<string, object?>[]> constructParameters, 
            string parameterPrefix)
        {
            this.cc = cc;
            this.sql = sql;
            this.constructParameters = constructParameters;
            this.parameterPrefix = parameterPrefix;
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedQueryContext Prefix(string parameterPrefix) =>
            new PreparedQueryContext(
                this.cc,
                this.sql, 
                this.constructParameters,
                parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedQueryContext Conversion(ConversionContext cc) =>
            new PreparedQueryContext(
                cc,
                this.sql,
                this.constructParameters,
                this.parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedQueryContext<T> Typed<T>()
            where T : new() =>
            new PreparedQueryContext<T>(
                this.cc,
                Query.defaultFieldComparer,
                this.sql,
                this.constructParameters,
                this.parameterPrefix);
    }

    public sealed class PreparedQueryContext<T>
        where T : new()
    {
        internal readonly ConversionContext cc;
        internal readonly IComparer<string> fieldComparer;
        internal readonly string sql;
        internal readonly Func<KeyValuePair<string, object?>[]> constructParameters;
        internal readonly string parameterPrefix;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal PreparedQueryContext(
            ConversionContext cc,
            IComparer<string> fieldComparer,
            string sql,
            Func<KeyValuePair<string, object?>[]> constructParameters, 
            string parameterPrefix)
        {
            this.cc = cc;
            this.fieldComparer = fieldComparer;
            this.sql = sql;
            this.constructParameters = constructParameters;
            this.parameterPrefix = parameterPrefix;
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedQueryContext<T> Prefix(string parameterPrefix) =>
            new PreparedQueryContext<T>(
                this.cc,
                this.fieldComparer,
                this.sql,
                this.constructParameters,
                parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedQueryContext<T> Conversion(ConversionContext cc) =>
            new PreparedQueryContext<T>(
                cc,
                this.fieldComparer,
                this.sql,
                this.constructParameters, 
                this.parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedQueryContext<T> FieldComparer(IComparer<string> fieldComparer) =>
            new PreparedQueryContext<T>(
                this.cc,
                fieldComparer,
                this.sql,
                this.constructParameters,
                this.parameterPrefix);
    }
}
