////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Context;
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
            string sql,
            string parameterPrefix) :
            base(cc, sql, Query.constructDefaultParameters) =>
            this.parameterPrefix = parameterPrefix;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedParameterizableQueryContext Prefix(string parameterPrefix) =>
            new PreparedParameterizableQueryContext(
                this.cc,
                this.sql, 
                parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedParameterizableQueryContext Conversion(ConversionContext cc) =>
            new PreparedParameterizableQueryContext(
                cc,
                this.sql,
                this.parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedParameterizableQueryContext<T> Typed<T>()
            where T : new() =>
            new PreparedParameterizableQueryContext<T>(
                this.cc,
                Query.defaultFieldComparer,
                this.sql,
                this.parameterPrefix);
    }

    public sealed class PreparedParameterizableQueryContext<T> : PreparedQueryContext<T>
        where T : new()
    {
        internal readonly string parameterPrefix;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal PreparedParameterizableQueryContext(
            ConversionContext cc,
            IComparer<string> fieldComparer,
            string sql,
            string parameterPrefix) :
            base(cc, fieldComparer, sql, Query.constructDefaultParameters) =>
            this.parameterPrefix = parameterPrefix;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedParameterizableQueryContext<T> Prefix(string parameterPrefix) =>
            new PreparedParameterizableQueryContext<T>(
                this.cc,
                this.fieldComparer,
                this.sql,
                parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedParameterizableQueryContext<T> Conversion(ConversionContext cc) =>
            new PreparedParameterizableQueryContext<T>(
                cc,
                this.fieldComparer,
                this.sql,
                this.parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedParameterizableQueryContext<T> FieldComparer(IComparer<string> fieldComparer) =>
            new PreparedParameterizableQueryContext<T>(
                this.cc,
                fieldComparer,
                this.sql,
                this.parameterPrefix);
    }
}
