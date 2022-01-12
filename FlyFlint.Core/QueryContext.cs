////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Text;

namespace FlyFlint
{
    public sealed class QueryContext
    {
        internal readonly DbConnection connection;
        internal readonly DbTransaction? transaction;
        internal readonly IFormatProvider fp;
        internal readonly Encoding encoding;
        internal readonly string sql;
        internal readonly (string name, object? value)[] parameters;
        internal readonly string parameterPrefix;

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal QueryContext(
            DbConnection connection, DbTransaction? transaction,
            IFormatProvider fp, Encoding encoding,
            string sql, (string name, object? value)[] parameters, string parameterPrefix)
        {
            this.connection = connection;
            this.transaction = transaction;
            this.fp = fp;
            this.encoding = encoding;
            this.transaction = transaction;
            this.sql = sql;
            this.parameters = parameters;
            this.parameterPrefix = parameterPrefix;
        }

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public QueryContext Prefix(string parameterPrefix) =>
            new QueryContext(this.connection, this.transaction, fp, this.encoding, this.sql, this.parameters, parameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public QueryContext Formatter(IFormatProvider fp) =>
            new QueryContext(this.connection, this.transaction, fp, this.encoding, this.sql, this.parameters, this.parameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public QueryContext Encoding(Encoding encoding) =>
            new QueryContext(this.connection, this.transaction, this.fp, encoding, this.sql, this.parameters, this.parameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public QueryContext Transaction(DbTransaction transaction) =>
            new QueryContext(this.connection, transaction, this.fp, this.encoding, this.sql, this.parameters, this.parameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public QueryContext<T> Typed<T>()
            where T : new() =>
            new QueryContext<T>(this.connection, this.transaction, this.fp, this.encoding, this.sql, this.parameters, this.parameterPrefix);
    }

    public sealed class QueryContext<T>
    {
        internal readonly DbConnection connection;
        internal readonly DbTransaction? transaction;
        internal readonly IFormatProvider fp;
        internal readonly Encoding encoding;
        internal readonly string sql;
        internal readonly (string name, object? value)[] parameters;
        internal readonly string parameterPrefix;

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal QueryContext(
            DbConnection connection, DbTransaction? transaction,
            IFormatProvider fp, Encoding encoding,
            string sql, (string name, object? value)[] parameters, string parameterPrefix)
        {
            this.connection = connection;
            this.transaction = transaction;
            this.fp = fp;
            this.encoding = encoding;
            this.transaction = transaction;
            this.sql = sql;
            this.parameters = parameters;
            this.parameterPrefix = parameterPrefix;
        }

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public QueryContext<T> Prefix(string parameterPrefix) =>
            new QueryContext<T>(this.connection, this.transaction, fp, this.encoding, this.sql, this.parameters, parameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public QueryContext<T> Formatter(IFormatProvider fp) =>
            new QueryContext<T>(this.connection, this.transaction, fp, this.encoding, this.sql, this.parameters, this.parameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public QueryContext<T> Encoding(Encoding encoding) =>
            new QueryContext<T>(this.connection, this.transaction, this.fp, encoding, this.sql, this.parameters, this.parameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public QueryContext<T> Transaction(DbTransaction transaction) =>
            new QueryContext<T>(this.connection, transaction, fp, this.encoding, this.sql, this.parameters, this.parameterPrefix);
    }
}
