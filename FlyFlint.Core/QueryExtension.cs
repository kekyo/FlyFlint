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
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace FlyFlint
{
    public static class QueryExtension
    {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizableQueryContext Query(
            this DbConnection connection,
            ParameterizableQueryString sql) =>
            new ParameterizableQueryContext(
                connection,
                null,
                ConversionContext.Default,
                sql.Sql,
                FlyFlint.Query.defaultParameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext Query(
            this DbConnection connection,
            FormattableString sql) =>
            new ParameterizedQueryContext(
                connection,
                null,
                ConversionContext.Default,
                QueryHelper.GetFormattedSqlString(sql, FlyFlint.Query.defaultParameterPrefix),
                QueryHelper.GetSqlParameters(sql, FlyFlint.Query.defaultParameterPrefix));

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizableQueryContext Query(
            this DbConnection connection,
            DbTransaction transaction,
            ParameterizableQueryString sql) =>
            new ParameterizableQueryContext(
                connection,
                transaction,
                ConversionContext.Default,
                sql.Sql,
                FlyFlint.Query.defaultParameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext Query(
            this DbConnection connection,
            DbTransaction transaction,
            FormattableString sql) =>
            new ParameterizedQueryContext(
                connection,
                transaction,
                ConversionContext.Default,
                QueryHelper.GetFormattedSqlString(sql, FlyFlint.Query.defaultParameterPrefix),
                QueryHelper.GetSqlParameters(sql, FlyFlint.Query.defaultParameterPrefix));

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizableQueryContext<TElement> Query<TElement>(
            this DbConnection connection,
            ParameterizableQueryString sql)
            where TElement : new() =>
            new ParameterizableQueryContext<TElement>(
                connection,
                null,
                ConversionContext.Default,
                FlyFlint.Query.defaultFieldComparer,
                sql.Sql,
                FlyFlint.Query.defaultParameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext<TElement> Query<TElement>(
            this DbConnection connection,
            FormattableString sql)
            where TElement : new() =>
            new ParameterizedQueryContext<TElement>(
                connection,
                null,
                ConversionContext.Default,
                FlyFlint.Query.defaultFieldComparer,
                QueryHelper.GetFormattedSqlString(sql, FlyFlint.Query.defaultParameterPrefix),
                QueryHelper.GetSqlParameters(sql, FlyFlint.Query.defaultParameterPrefix));

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizableQueryContext<TElement> Query<TElement>(
            this DbConnection connection,
            DbTransaction transaction,
            ParameterizableQueryString sql)
            where TElement : new() =>
            new ParameterizableQueryContext<TElement>(
                connection,
                transaction,
                ConversionContext.Default,
                FlyFlint.Query.defaultFieldComparer,
                sql.Sql,
                FlyFlint.Query.defaultParameterPrefix);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext<TElement> Query<TElement>(
            this DbConnection connection,
            DbTransaction transaction,
            FormattableString sql)
            where TElement : new() =>
            new ParameterizedQueryContext<TElement>(
                connection,
                transaction,
                ConversionContext.Default,
                FlyFlint.Query.defaultFieldComparer,
                QueryHelper.GetFormattedSqlString(sql, FlyFlint.Query.defaultParameterPrefix),
                QueryHelper.GetSqlParameters(sql, FlyFlint.Query.defaultParameterPrefix));

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext Query(
            this DbConnection connection,
            PreparedParameterizedQueryContext prepared)
        {
            var built = prepared.builder();
            return new ParameterizedQueryContext(
                connection,
                null,
                prepared.cc,
                built.sql,
                built.parameters);
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizableQueryContext Query(
            this DbConnection connection,
            PreparedParameterizableQueryContext prepared)
        {
            var built = prepared.builder();
            Debug.Assert(object.ReferenceEquals(built.parameters, FlyFlint.Query.defaultParameters));
            return new ParameterizableQueryContext(
                connection,
                null,
                prepared.cc,
                built.sql,
                prepared.parameterPrefix);
        }

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext Query(
            this DbConnection connection,
            DbTransaction transaction,
            PreparedParameterizedQueryContext prepared)
        {
            var built = prepared.builder();
            return new ParameterizedQueryContext(
                connection,
                transaction,
                prepared.cc,
                built.sql,
                built.parameters);
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizableQueryContext Query(
            this DbConnection connection,
            DbTransaction transaction,
            PreparedParameterizableQueryContext prepared)
        {
            var built = prepared.builder();
            Debug.Assert(object.ReferenceEquals(built.parameters, FlyFlint.Query.defaultParameters));
            return new ParameterizableQueryContext(
                connection,
                transaction,
                prepared.cc,
                built.sql,
                prepared.parameterPrefix);
        }

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext<TElement> Query<TElement>(
            this DbConnection connection,
            PreparedParameterizedQueryContext<TElement> prepared)
            where TElement : new()
        {
            var built = prepared.builder();
            return new ParameterizedQueryContext<TElement>(
                connection,
                null,
                prepared.cc,
                prepared.fieldComparer,
                built.sql,
                built.parameters);
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizableQueryContext<TElement> Query<TElement>(
            this DbConnection connection,
            PreparedParameterizableQueryContext<TElement> prepared)
            where TElement : new()
        {
            var built = prepared.builder();
            Debug.Assert(object.ReferenceEquals(built.parameters, FlyFlint.Query.defaultParameters));
            return new ParameterizableQueryContext<TElement>(
                connection,
                null,
                prepared.cc,
                prepared.fieldComparer,
                built.sql,
                prepared.parameterPrefix);
        }

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext<TElement> Query<TElement>(
            this DbConnection connection,
            DbTransaction transaction,
            PreparedParameterizedQueryContext<TElement> prepared)
            where TElement : new()
        {
            var built = prepared.builder();
            return new ParameterizedQueryContext<TElement>(
                connection,
                transaction,
                prepared.cc,
                prepared.fieldComparer,
                built.sql,
                built.parameters);
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizableQueryContext<TElement> Query<TElement>(
            this DbConnection connection,
            DbTransaction transaction,
            PreparedParameterizableQueryContext<TElement> prepared)
            where TElement : new()
        {
            var built = prepared.builder();
            Debug.Assert(object.ReferenceEquals(built.parameters, FlyFlint.Query.defaultParameters));
            return new ParameterizableQueryContext<TElement>(
                connection,
                transaction,
                prepared.cc,
                prepared.fieldComparer,
                built.sql,
                prepared.parameterPrefix);
        }
    }
}
