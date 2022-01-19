////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace FlyFlint.Internal.Static
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class StaticQueryFacade
    {
#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static QueryContext Query<TParameters>(
            DbConnection connection,
            string sql, TParameters parameters)
            where TParameters : IParameterExtractable =>
            new QueryContext(
                connection,
                null,
                ConversionContext.Default,
                sql,
                StaticQueryExecutor.GetParameters(ref parameters, FlyFlint.Query.defaultParameterPrefix),
                FlyFlint.Query.defaultParameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static QueryContext Query<TParameters>(
            DbConnection connection, DbTransaction transaction,
            string sql, TParameters parameters)
            where TParameters : IParameterExtractable =>
            new QueryContext(
                connection,
                transaction,
                ConversionContext.Default,
                sql,
                StaticQueryExecutor.GetParameters(ref parameters, FlyFlint.Query.defaultParameterPrefix),
                FlyFlint.Query.defaultParameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static QueryContext Query<TParameters>(
            DbConnection connection,
            PreparedQueryContext prepared, TParameters parameters)
            where TParameters : IParameterExtractable =>
            new QueryContext(
                connection,
                null,
                prepared.cc,
                prepared.sql,
                StaticQueryExecutor.GetParameters(ref parameters, prepared.parameterPrefix),
                prepared.parameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static QueryContext Query<TParameters>(
            DbConnection connection, DbTransaction transaction,
            PreparedQueryContext prepared, TParameters parameters)
            where TParameters : IParameterExtractable =>
            new QueryContext(
                connection,
                transaction,
                prepared.cc,
                prepared.sql,
                StaticQueryExecutor.GetParameters(ref parameters, prepared.parameterPrefix),
                prepared.parameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static QueryContext<T> Query<T, TParameters>(
            DbConnection connection,
            PreparedQueryContext<T> prepared, TParameters parameters)
            where T : new()
            where TParameters : IParameterExtractable =>
            new QueryContext<T>(
                connection,
                null,
                prepared.cc,
                prepared.fieldComparer,
                prepared.sql,
                StaticQueryExecutor.GetParameters(ref parameters, prepared.parameterPrefix),
                prepared.parameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static QueryContext<T> Query<T, TParameters>(
            DbConnection connection, DbTransaction transaction,
            PreparedQueryContext<T> prepared, TParameters parameters)
            where T : new()
            where TParameters : IParameterExtractable =>
            new QueryContext<T>(
                connection,
                transaction,
                prepared.cc,
                prepared.fieldComparer,
                prepared.sql,
                StaticQueryExecutor.GetParameters(ref parameters, prepared.parameterPrefix),
                prepared.parameterPrefix);

        /////////////////////////////////////////////////////////////////////////////

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static PreparedQueryContext Parameter<TParameters>(
            PreparedQueryContext prepared, Func<TParameters> getter)
            where TParameters : notnull, IParameterExtractable =>
            new PreparedQueryContext(
                prepared.cc,
                prepared.sql,
                StaticQueryExecutor.GetConstructParameters(getter, prepared.parameterPrefix),
                prepared.parameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static PreparedQueryContext<T> Parameter<T, TParameters>(
            PreparedQueryContext<T> prepared, Func<TParameters> getter)
            where T : new()
            where TParameters : notnull, IParameterExtractable =>
            new PreparedQueryContext<T>(
                prepared.cc,
                prepared.fieldComparer,
                prepared.sql,
                StaticQueryExecutor.GetConstructParameters(getter, prepared.parameterPrefix),
                prepared.parameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static QueryContext Parameter<TParameters>(
            QueryContext query, TParameters parameters)
            where TParameters : notnull, IParameterExtractable =>
            new QueryContext(
                query.connection,
                query.transaction,
                query.cc,
                query.sql,
                StaticQueryExecutor.GetParameters(ref parameters, query.parameterPrefix),
                query.parameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static QueryContext<T> Parameter<T, TParameters>(
            QueryContext<T> query, TParameters parameters)
            where T : new()
            where TParameters : IParameterExtractable =>
            new QueryContext<T>(
                query.connection,
                query.transaction,
                query.cc,
                query.fieldComparer,
                query.sql,
                StaticQueryExecutor.GetParameters(ref parameters, query.parameterPrefix),
                query.parameterPrefix);

        /////////////////////////////////////////////////////////////////////////////

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void ExecuteNonQuery(QueryContext query) =>
            StaticQueryExecutor.ExecuteNonQuery(query);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static T ExecuteScalar<T>(QueryContext query) =>
            StaticQueryExecutor.ExecuteScalar<T>(query);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IEnumerable<T> Execute<T>(QueryContext<T> query)
            where T : IDataInjectable, new() =>
            StaticQueryExecutor.Execute(query);

        /////////////////////////////////////////////////////////////////////

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Task ExecuteNonQueryAsync(QueryContext query) =>
            StaticQueryExecutor.ExecuteNonQueryAsync(query);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Task<T> ExecuteScalarAsync<T>(QueryContext query) =>
            StaticQueryExecutor.ExecuteScalarAsync<T>(query);

#if !NET40 && !NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static IAsyncEnumerable<T> ExecuteAsync<T>(QueryContext<T> query)
            where T : IDataInjectable, new() =>
            StaticQueryExecutor.ExecuteAsync(query);
#endif
    }
}
