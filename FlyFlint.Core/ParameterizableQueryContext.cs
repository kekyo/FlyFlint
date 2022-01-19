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
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace FlyFlint
{
    public sealed class ParameterizableQueryContext : QueryContext
    {
        internal readonly string parameterPrefix;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal ParameterizableQueryContext(
            DbConnection connection,
            DbTransaction? transaction,
            ConversionContext cc,
            string sql,
            string parameterPrefix) :
            base(connection, transaction, cc, sql, Query.defaultParameters) =>
            this.parameterPrefix = parameterPrefix;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public ParameterizableQueryContext Prefix(string parameterPrefix) =>
            new ParameterizableQueryContext(
                this.connection,
                this.transaction,
                this.cc,
                this.sql,
                parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public ParameterizableQueryContext Conversion(ConversionContext cc) =>
            new ParameterizableQueryContext(
                this.connection,
                this.transaction,
                cc,
                this.sql,
                this.parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public ParameterizableQueryContext Transaction(DbTransaction transaction) =>
            new ParameterizableQueryContext(
                this.connection,
                transaction,
                this.cc,
                this.sql,
                this.parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public ParameterizableQueryContext<T> Typed<T>()
            where T : new() =>
            new ParameterizableQueryContext<T>(
                this.connection,
                this.transaction,
                this.cc,
                FlyFlint.Query.defaultFieldComparer,
                this.sql,
                this.parameterPrefix);
    }

    public sealed class ParameterizableQueryContext<T> : QueryContext<T>
    {
        internal readonly string parameterPrefix;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal ParameterizableQueryContext(
            DbConnection connection,
            DbTransaction? transaction,
            ConversionContext cc,
            IComparer<string> fieldComparer,
            string sql,
            string parameterPrefix) :
            base(connection, transaction, cc, fieldComparer, sql, Query.defaultParameters) =>
            this.parameterPrefix = parameterPrefix;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public ParameterizableQueryContext<T> Prefix(string parameterPrefix) =>
            new ParameterizableQueryContext<T>(
                this.connection,
                this.transaction,
                this.cc,
                this.fieldComparer,
                this.sql,
                parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public ParameterizableQueryContext<T> Conversion(ConversionContext cc) =>
            new ParameterizableQueryContext<T>(
                this.connection,
                this.transaction,
                cc,
                this.fieldComparer,
                this.sql,
                this.parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public ParameterizableQueryContext<T> FieldComparer(IComparer<string> fieldComparer) =>
            new ParameterizableQueryContext<T>(
                this.connection,
                this.transaction,
                this.cc,
                fieldComparer,
                this.sql,
                this.parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public ParameterizableQueryContext<T> Transaction(DbTransaction transaction) =>
            new ParameterizableQueryContext<T>(
                this.connection,
                transaction,
                this.cc,
                this.fieldComparer,
                this.sql,
                this.parameterPrefix);
    }
}
