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
    public sealed class QueryContext
    {
        internal readonly DbConnection connection;
        internal readonly DbTransaction? transaction;
        internal readonly ConversionContext cc;
        internal readonly string sql;
        internal readonly KeyValuePair<string, object?>[] parameters;
        internal readonly string parameterPrefix;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal QueryContext(
            DbConnection connection, DbTransaction? transaction,
            ConversionContext cc,
            string sql, KeyValuePair<string, object?>[] parameters, string parameterPrefix)
        {
            this.connection = connection;
            this.transaction = transaction;
            this.cc = cc;
            this.transaction = transaction;
            this.sql = sql;
            this.parameters = parameters;
            this.parameterPrefix = parameterPrefix;
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public QueryContext Prefix(string parameterPrefix) =>
            new QueryContext(
                this.connection,
                this.transaction,
                this.cc,
                this.sql,
                this.parameters,
                parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public QueryContext Conversion(ConversionContext cc) =>
            new QueryContext(
                this.connection,
                this.transaction,
                cc,
                this.sql,
                this.parameters,
                this.parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public QueryContext Transaction(DbTransaction transaction) =>
            new QueryContext(
                this.connection,
                transaction,
                this.cc,
                this.sql,
                this.parameters,
                this.parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public QueryContext<T> Typed<T>()
            where T : new() =>
            new QueryContext<T>(
                this.connection,
                this.transaction,
                this.cc,
                FlyFlint.Query.defaultFieldComparer,
                this.sql,
                this.parameters,
                this.parameterPrefix);
    }

    public sealed class QueryContext<T>
    {
        internal readonly DbConnection connection;
        internal readonly DbTransaction? transaction;
        internal readonly ConversionContext cc;
        internal readonly IComparer<string> fieldComparer;
        internal readonly string sql;
        internal readonly KeyValuePair<string, object?>[] parameters;
        internal readonly string parameterPrefix;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal QueryContext(
            DbConnection connection, DbTransaction? transaction,
            ConversionContext cc, IComparer<string> fieldComparer,
            string sql, KeyValuePair<string, object?>[] parameters, string parameterPrefix)
        {
            this.connection = connection;
            this.transaction = transaction;
            this.cc = cc;
            this.fieldComparer = fieldComparer;
            this.transaction = transaction;
            this.sql = sql;
            this.parameters = parameters;
            this.parameterPrefix = parameterPrefix;
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public QueryContext<T> Prefix(string parameterPrefix) =>
            new QueryContext<T>(
                this.connection,
                this.transaction,
                this.cc,
                this.fieldComparer,
                this.sql,
                this.parameters,
                parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public QueryContext<T> Conversion(ConversionContext cc) =>
            new QueryContext<T>(
                this.connection,
                this.transaction,
                cc,
                this.fieldComparer,
                this.sql,
                this.parameters,
                this.parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public QueryContext<T> FieldComparer(IComparer<string> fieldComparer) =>
            new QueryContext<T>(
                this.connection,
                this.transaction,
                this.cc,
                fieldComparer,
                this.sql,
                this.parameters,
                this.parameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public QueryContext<T> Transaction(DbTransaction transaction) =>
            new QueryContext<T>(
                this.connection,
                transaction,
                this.cc,
                this.fieldComparer,
                this.sql,
                this.parameters,
                this.parameterPrefix);
    }
}
