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
    public abstract class QueryContext
    {
        internal readonly DbConnection connection;
        internal readonly DbTransaction? transaction;
        internal readonly Trait trait;
        internal readonly string sql;
        internal readonly ExtractedParameter[] parameters;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private protected QueryContext(
            DbConnection connection,
            DbTransaction? transaction,
            Trait trait,
            string sql,
            ExtractedParameter[] parameters)
        {
            this.connection = connection;
            this.transaction = transaction;
            this.trait = trait;
            this.sql = sql;
            this.parameters = parameters;
        }
    }

    public abstract class QueryContext<TRecord>
    {
        internal readonly DbConnection connection;
        internal readonly DbTransaction? transaction;
        internal readonly Trait trait;
        internal readonly string sql;
        internal readonly ExtractedParameter[] parameters;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private protected QueryContext(
            DbConnection connection,
            DbTransaction? transaction,
            Trait trait,
            string sql,
            ExtractedParameter[] parameters)
        {
            this.connection = connection;
            this.transaction = transaction;
            this.trait = trait;
            this.sql = sql;
            this.parameters = parameters;
        }
    }
}
