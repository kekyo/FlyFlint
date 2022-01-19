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
    public sealed class DatabaseTrait
    {
        internal static readonly KeyValuePair<string, object?>[] defaultParameters = { };

        internal readonly ConversionContext cc;
        internal readonly IComparer<string> fieldComparer;
        internal readonly string parameterPrefix;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private DatabaseTrait(ConversionContext cc, IComparer<string> fieldComparer, string parameterPrefix)
        {
            this.cc = cc;
            this.fieldComparer = fieldComparer;
            this.parameterPrefix = parameterPrefix;
        }

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedPartialQueryContext Prepare(PartialQueryString sql) =>
            new PreparedPartialQueryContext(
                this,
                () => new QueryParameterBuilderResult(sql.Sql, defaultParameters));

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedPartialQueryContext<T> Prepare<T>(PartialQueryString sql)
            where T : new() =>
            new PreparedPartialQueryContext<T>(
                this,
                () => new QueryParameterBuilderResult(sql.Sql, defaultParameters));

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
            PartialQueryString sql) =>
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
            DbTransaction transaction,
            PartialQueryString sql) =>
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
            DbTransaction transaction,
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
        public PartialQueryContext<TElement> Query<TElement>(
            DbConnection connection,
            PartialQueryString sql)
            where TElement : new() =>
            new PartialQueryContext<TElement>(
                connection,
                null,
                this,
                sql.Sql);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public ParameterizedQueryContext<TElement> Query<TElement>(
            DbConnection connection,
            FormattableString sql)
            where TElement : new() =>
            new ParameterizedQueryContext<TElement>(
                connection,
                null,
                this,
                QueryHelper.GetFormattedSqlString(sql, this.parameterPrefix),
                QueryHelper.GetSqlParameters(sql, this.parameterPrefix));

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PartialQueryContext<TElement> Query<TElement>(
            DbConnection connection,
            DbTransaction transaction,
            PartialQueryString sql)
            where TElement : new() =>
            new PartialQueryContext<TElement>(
                connection,
                transaction,
                this,
                sql.Sql);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public ParameterizedQueryContext<TElement> Query<TElement>(
            DbConnection connection,
            DbTransaction transaction,
            FormattableString sql)
            where TElement : new() =>
            new ParameterizedQueryContext<TElement>(
                connection,
                transaction,
                this,
                QueryHelper.GetFormattedSqlString(sql, this.parameterPrefix),
                QueryHelper.GetSqlParameters(sql, this.parameterPrefix));

        /////////////////////////////////////////////////////////////////////////////

        public static readonly DatabaseTrait Default =
            new DatabaseTrait(ConversionContext.Default, StringComparer.OrdinalIgnoreCase, "@");

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static DatabaseTrait Create(ConversionContext cc) =>
            new DatabaseTrait(cc, StringComparer.OrdinalIgnoreCase, "@");
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static DatabaseTrait Create(IComparer<string> fieldComparer) =>
            new DatabaseTrait(ConversionContext.Default, fieldComparer, "@");
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static DatabaseTrait Create(string parameterPrefix) =>
            new DatabaseTrait(ConversionContext.Default, StringComparer.OrdinalIgnoreCase, parameterPrefix);
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static DatabaseTrait Create(ConversionContext cc, IComparer<string> fieldComparer) =>
            new DatabaseTrait(cc, fieldComparer, "@");
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static DatabaseTrait Create(IComparer<string> fieldComparer, string parameterPrefix) =>
            new DatabaseTrait(ConversionContext.Default, fieldComparer, parameterPrefix);
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static DatabaseTrait Create(ConversionContext cc, IComparer<string> fieldComparer, string parameterPrefix) =>
            new DatabaseTrait(cc, fieldComparer, parameterPrefix);
    }
}
