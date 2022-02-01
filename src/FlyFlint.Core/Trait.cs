////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Context;
using FlyFlint.Internal;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace FlyFlint
{
    public sealed class Trait
    {
        internal readonly ConversionContext cc;
        internal readonly IComparer<string> fieldComparer;
        internal readonly string parameterPrefix;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal Trait(
            ConversionContext cc,
            IComparer<string> fieldComparer,
            string parameterPrefix)
        {
            this.cc = cc;
            this.fieldComparer = fieldComparer;
            this.parameterPrefix = parameterPrefix;
        }

        /////////////////////////////////////////////////////////////////////////////

        public ConversionContext ConversionContext =>
            this.cc;

        public IComparer<string> FieldComparer =>
            this.fieldComparer;

        public string ParameterPrefix =>
            this.parameterPrefix;

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedPartialQueryContext Prepare(String sql) =>
            new PreparedPartialQueryContext(
                this,
                () => new QueryParameterBuilderResult(sql.Sql, QueryHelper.DefaultParameters));

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedPartialQueryContext<T> Prepare<T>(String sql)
            where T : new() =>
            new PreparedPartialQueryContext<T>(
                this,
                () => new QueryParameterBuilderResult(sql.Sql, QueryHelper.DefaultParameters));

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedParameterizedQueryContext Prepare(FormattableString sql) =>
            new PreparedParameterizedQueryContext(
                this,
                () => new QueryParameterBuilderResult(
                    QueryHelper.GetFormattedSqlString(sql, this.parameterPrefix),
                    QueryHelper.GetSqlParameters(sql, this.parameterPrefix)));

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedParameterizedQueryContext<T> Prepare<T>(FormattableString sql)
            where T : new() =>
            new PreparedParameterizedQueryContext<T>(
                this,
                () => new QueryParameterBuilderResult(
                    QueryHelper.GetFormattedSqlString(sql, this.parameterPrefix),
                    QueryHelper.GetSqlParameters(sql, this.parameterPrefix)));

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedParameterizedQueryContext Prepare(Func<FormattableString> sqlBuilder) =>
            new PreparedParameterizedQueryContext(
                this,
                () =>
                {
                    var sql = sqlBuilder();
                    return new QueryParameterBuilderResult(
                        QueryHelper.GetFormattedSqlString(sql, this.parameterPrefix),
                        QueryHelper.GetSqlParameters(sql, this.parameterPrefix));
                });

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedParameterizedQueryContext<T> Prepare<T>(Func<FormattableString> sqlBuilder)
            where T : new() =>
            new PreparedParameterizedQueryContext<T>(
                this,
                () =>
                {
                    var sql = sqlBuilder();
                    return new QueryParameterBuilderResult(
                        QueryHelper.GetFormattedSqlString(sql, this.parameterPrefix),
                        QueryHelper.GetSqlParameters(sql, this.parameterPrefix));
                });

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PartialQueryContext Query(
            DbConnection connection,
            String sql) =>
            new PartialQueryContext(
                connection,
                null,
                this,
                sql.Sql);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public ParameterizedQueryContext Query(
            DbConnection connection,
            FormattableString sql) =>
            new ParameterizedQueryContext(
                connection,
                null,
                this,
                QueryHelper.GetFormattedSqlString(sql, this.parameterPrefix),
                QueryHelper.GetSqlParameters(sql, this.parameterPrefix));

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PartialQueryContext Query(
            DbConnection connection,
            DbTransaction? transaction,
            String sql) =>
            new PartialQueryContext(
                connection,
                transaction,
                this,
                sql.Sql);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public ParameterizedQueryContext Query(
            DbConnection connection,
            DbTransaction? transaction,
            FormattableString sql) =>
            new ParameterizedQueryContext(
                connection,
                transaction,
                this,
                QueryHelper.GetFormattedSqlString(sql, this.parameterPrefix),
                QueryHelper.GetSqlParameters(sql, this.parameterPrefix));

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PartialQueryContext<TRecord> Query<TRecord>(
            DbConnection connection,
            String sql)
            where TRecord : new() =>
            new PartialQueryContext<TRecord>(
                connection,
                null,
                this,
                sql.Sql);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public ParameterizedQueryContext<TRecord> Query<TRecord>(
            DbConnection connection,
            FormattableString sql)
            where TRecord : new() =>
            new ParameterizedQueryContext<TRecord>(
                connection,
                null,
                this,
                QueryHelper.GetFormattedSqlString(sql, this.parameterPrefix),
                QueryHelper.GetSqlParameters(sql, this.parameterPrefix));

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PartialQueryContext<TRecord> Query<TRecord>(
            DbConnection connection,
            DbTransaction? transaction,
            String sql)
            where TRecord : new() =>
            new PartialQueryContext<TRecord>(
                connection,
                transaction,
                this,
                sql.Sql);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public ParameterizedQueryContext<TRecord> Query<TRecord>(
            DbConnection connection,
            DbTransaction? transaction,
            FormattableString sql)
            where TRecord : new() =>
            new ParameterizedQueryContext<TRecord>(
                connection,
                transaction,
                this,
                QueryHelper.GetFormattedSqlString(sql, this.parameterPrefix),
                QueryHelper.GetSqlParameters(sql, this.parameterPrefix));
    }
}
