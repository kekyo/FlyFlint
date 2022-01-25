////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Internal;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace FlyFlint.Context
{
    public sealed class ParameterizedQueryContext : QueryContext
    {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal ParameterizedQueryContext(
            DbConnection connection,
            DbTransaction? transaction,
            Trait trait,
            string sql,
            ExtractedParameter[] parameters) :
            base(connection, transaction, trait, sql, parameters)
        {
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public ParameterizedQueryContext Transaction(DbTransaction? transaction) =>
            new ParameterizedQueryContext(
                this.connection,
                transaction,
                this.trait,
                this.sql,
                this.parameters);
    }

    public sealed class ParameterizedQueryContext<TElement> : QueryContext<TElement>
    {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal ParameterizedQueryContext(
            DbConnection connection,
            DbTransaction? transaction,
            Trait trait,
            string sql,
            ExtractedParameter[] parameters) :
            base(connection, transaction, trait, sql, parameters)
        {
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public ParameterizedQueryContext<TElement> Transaction(DbTransaction? transaction) =>
            new ParameterizedQueryContext<TElement>(
                this.connection,
                transaction,
                this.trait,
                this.sql,
                this.parameters);
    }
}
