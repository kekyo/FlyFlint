////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace FlyFlint.Context
{
    public sealed class PartialQueryContext : QueryContext
    {
        internal readonly string parameterPrefix;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal PartialQueryContext(
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
        public PartialQueryContext Prefix(string parameterPrefix) =>
            new PartialQueryContext(
                this.connection,
                this.transaction,
                this.cc,
                this.sql,
                parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PartialQueryContext Conversion(ConversionContext cc) =>
            new PartialQueryContext(
                this.connection,
                this.transaction,
                cc,
                this.sql,
                this.parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PartialQueryContext Transaction(DbTransaction transaction) =>
            new PartialQueryContext(
                this.connection,
                transaction,
                this.cc,
                this.sql,
                this.parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PartialQueryContext<TElement> Typed<TElement>()
            where TElement : new() =>
            new PartialQueryContext<TElement>(
                this.connection,
                this.transaction,
                this.cc,
                FlyFlint.Query.defaultFieldComparer,
                this.sql,
                this.parameterPrefix);
    }

    public sealed class PartialQueryContext<TElement> : QueryContext<TElement>
    {
        internal readonly string parameterPrefix;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal PartialQueryContext(
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
        public PartialQueryContext<TElement> Prefix(string parameterPrefix) =>
            new PartialQueryContext<TElement>(
                this.connection,
                this.transaction,
                this.cc,
                this.fieldComparer,
                this.sql,
                parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PartialQueryContext<TElement> Conversion(ConversionContext cc) =>
            new PartialQueryContext<TElement>(
                this.connection,
                this.transaction,
                cc,
                this.fieldComparer,
                this.sql,
                this.parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PartialQueryContext<TElement> FieldComparer(IComparer<string> fieldComparer) =>
            new PartialQueryContext<TElement>(
                this.connection,
                this.transaction,
                this.cc,
                fieldComparer,
                this.sql,
                this.parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PartialQueryContext<TElement> Transaction(DbTransaction transaction) =>
            new PartialQueryContext<TElement>(
                this.connection,
                transaction,
                this.cc,
                this.fieldComparer,
                this.sql,
                this.parameterPrefix);
    }
}
