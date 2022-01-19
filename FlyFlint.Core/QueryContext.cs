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
    public abstract class QueryContext
    {
        internal readonly DbConnection connection;
        internal readonly DbTransaction? transaction;
        internal readonly ConversionContext cc;
        internal readonly string sql;
        internal readonly KeyValuePair<string, object?>[] parameters;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private protected QueryContext(
            DbConnection connection,
            DbTransaction? transaction,
            ConversionContext cc,
            string sql,
            KeyValuePair<string, object?>[] parameters)
        {
            this.connection = connection;
            this.transaction = transaction;
            this.cc = cc;
            this.sql = sql;
            this.parameters = parameters;
        }
    }

    public abstract class QueryContext<T>
    {
        internal readonly DbConnection connection;
        internal readonly DbTransaction? transaction;
        internal readonly ConversionContext cc;
        internal readonly IComparer<string> fieldComparer;
        internal readonly string sql;
        internal readonly KeyValuePair<string, object?>[] parameters;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private protected QueryContext(
            DbConnection connection,
            DbTransaction? transaction,
            ConversionContext cc,
            IComparer<string> fieldComparer,
            string sql,
            KeyValuePair<string, object?>[] parameters)
        {
            this.connection = connection;
            this.transaction = transaction;
            this.cc = cc;
            this.fieldComparer = fieldComparer;
            this.sql = sql;
            this.parameters = parameters;
        }
    }
}
