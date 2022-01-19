﻿////////////////////////////////////////////////////////////////////////////
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
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal PartialQueryContext(
            DbConnection connection,
            DbTransaction? transaction,
            DatabaseTrait trait,
            string sql) :
            base(connection, transaction, trait, sql, DatabaseTrait.defaultParameters)
        {
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PartialQueryContext Transaction(DbTransaction transaction) =>
            new PartialQueryContext(
                this.connection,
                transaction,
                this.trait,
                this.sql);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PartialQueryContext<TElement> Typed<TElement>()
            where TElement : new() =>
            new PartialQueryContext<TElement>(
                this.connection,
                this.transaction,
                this.trait,
                this.sql);
    }

    public sealed class PartialQueryContext<TElement> : QueryContext<TElement>
    {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal PartialQueryContext(
            DbConnection connection,
            DbTransaction? transaction,
            DatabaseTrait trait,
            string sql) :
            base(connection, transaction, trait, sql, DatabaseTrait.defaultParameters)
        {
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PartialQueryContext<TElement> Transaction(DbTransaction transaction) =>
            new PartialQueryContext<TElement>(
                this.connection,
                transaction,
                this.trait,
                this.sql);
    }
}
