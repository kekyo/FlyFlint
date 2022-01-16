////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Context;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace FlyFlint
{
    public static class QueryExtension
    {
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext Query(
            this DbConnection connection,
            string sql) =>
            new QueryContext(
                connection,
                null,
                ConversionContext.Default,
                sql,
                FlyFlint.Query.defaultParameters,
                FlyFlint.Query.defaultParameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext Query(
            this DbConnection connection,
            DbTransaction transaction,
            string sql) =>
            new QueryContext(
                connection,
                transaction,
                ConversionContext.Default,
                sql,
                FlyFlint.Query.defaultParameters,
                FlyFlint.Query.defaultParameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext<T> Query<T>(
            this DbConnection connection,
            string sql)
            where T : new() =>
            new QueryContext<T>(
                connection,
                null,
                ConversionContext.Default,
                sql,
                FlyFlint.Query.defaultParameters,
                FlyFlint.Query.defaultParameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext<T> Query<T>(
            this DbConnection connection,
            DbTransaction transaction,
            string sql)
            where T : new() =>
            new QueryContext<T>(
                connection,
                transaction,
                ConversionContext.Default,
                sql,
                FlyFlint.Query.defaultParameters,
                FlyFlint.Query.defaultParameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext Query(
            this DbConnection connection,
            PreparedQueryContext prepared) =>
            new QueryContext(
                connection,
                null,
                prepared.cc,
                prepared.sql,
                prepared.parameters,
                prepared.parameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext Query(
            this DbConnection connection, DbTransaction transaction,
            PreparedQueryContext prepared) =>
            new QueryContext(
                connection,
                transaction,
                prepared.cc,
                prepared.sql,
                prepared.parameters,
                prepared.parameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext<T> Query<T>(
            this DbConnection connection,
            PreparedQueryContext<T> prepared)
            where T : new() =>
            new QueryContext<T>(
                connection,
                null,
                prepared.cc,
                prepared.sql,
                prepared.parameters,
                prepared.parameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static QueryContext<T> Query<T>(
            this DbConnection connection, DbTransaction transaction,
            PreparedQueryContext<T> prepared)
            where T : new() =>
            new QueryContext<T>(
                connection,
                transaction,
                prepared.cc,
                prepared.sql,
                prepared.parameters,
                prepared.parameterPrefix);
    }
}
