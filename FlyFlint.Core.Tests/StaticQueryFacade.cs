////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Context;
using FlyFlint.Internal.Static;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace FlyFlint
{
    public static class StaticQueryFacadeExtension
    {
        public static ParameterizedQueryContext Query__<TParameters>(
            this DbConnection connection,
            String sql,
            TParameters parameters)
            where TParameters : notnull, IParameterExtractable =>
            StaticQueryFacade.Query(connection, sql, parameters);

        public static ParameterizedQueryContext Query__<TParameters>(
            this DbConnection connection,
            DbTransaction transaction,
            String sql,
            TParameters parameters)
            where TParameters : notnull, IParameterExtractable =>
            StaticQueryFacade.Query(connection, transaction, sql, parameters);

        /////////////////////////////////////////////////////////////////////////////

        public static ParameterizedQueryContext Query__<TParameters>(
            this DbConnection connection,
            PreparedPartialQueryContext prepared,
            TParameters parameters)
            where TParameters : notnull, IParameterExtractable =>
            StaticQueryFacade.Query(connection, prepared, parameters);

        public static ParameterizedQueryContext Query__<TParameters>(
            this DbConnection connection,
            DbTransaction transaction,
            PreparedPartialQueryContext prepared,
            TParameters parameters)
            where TParameters : notnull, IParameterExtractable =>
            StaticQueryFacade.Query(connection, transaction, prepared, parameters);

        /////////////////////////////////////////////////////////////////////////////

        public static ParameterizedQueryContext<TElement> Query__<TElement, TParameters>(
            this DbConnection connection,
            PreparedPartialQueryContext<TElement> prepared,
            TParameters parameters)
            where TElement : IDataInjectable, new()
            where TParameters : notnull, IParameterExtractable =>
            StaticQueryFacade.Query(connection, prepared, parameters);

        public static ParameterizedQueryContext<TElement> Query__<TElement, TParameters>(
            this DbConnection connection,
            DbTransaction transaction,
            PreparedPartialQueryContext<TElement> prepared,
            TParameters parameters)
            where TElement : IDataInjectable, new()
            where TParameters : notnull, IParameterExtractable =>
            StaticQueryFacade.Query(connection, transaction, prepared, parameters);

        /////////////////////////////////////////////////////////////////////////////

        public static PreparedParameterizedQueryContext Parameter__<TParameters>(
            this PreparedPartialQueryContext prepared,
            Func<TParameters> getter)
            where TParameters : notnull, IParameterExtractable =>
            StaticQueryFacade.Parameter(prepared, getter);

        public static PreparedParameterizedQueryContext<TElement> Parameter__<TElement, TParameters>(
            this PreparedPartialQueryContext<TElement> prepared,
            Func<TParameters> getter)
            where TElement : IDataInjectable, new()
            where TParameters : notnull, IParameterExtractable =>
            StaticQueryFacade.Parameter(prepared, getter);

        /////////////////////////////////////////////////////////////////////////////

        public static ParameterizedQueryContext Parameter__<TParameters>(
            this PartialQueryContext query,
            TParameters parameters)
            where TParameters : notnull, IParameterExtractable =>
            StaticQueryFacade.Parameter(query, parameters);

        public static ParameterizedQueryContext<TElement> Parameter__<TElement, TParameters>(
            this PartialQueryContext<TElement> query,
            TParameters parameters)
            where TElement : IDataInjectable, new()
            where TParameters : notnull, IParameterExtractable =>
            StaticQueryFacade.Parameter(query, parameters);

        /////////////////////////////////////////////////////////////////////////////

        public static void ExecuteNonQuery__(this QueryContext query) =>
            StaticQueryFacade.ExecuteNonQuery(query);

        public static TElement ExecuteScalar__<TElement>(this QueryContext<TElement> query) =>
            StaticQueryFacade.ExecuteScalar(query);

        public static IEnumerable<TElement> Execute__<TElement>(this QueryContext<TElement> query)
            where TElement : IDataInjectable, new() =>
            StaticQueryFacade.Execute(query);

        /////////////////////////////////////////////////////////////////////

        public static Task ExecuteNonQueryAsync__(this QueryContext query) =>
            StaticQueryFacade.ExecuteNonQueryAsync(query);

        public static Task<TElement> ExecuteScalarAsync__<TElement>(this QueryContext<TElement> query) =>
            StaticQueryFacade.ExecuteScalarAsync(query);

        public static IAsyncEnumerable<TElement> ExecuteAsync__<TElement>(this QueryContext<TElement> query)
            where TElement : IDataInjectable, new() =>
            StaticQueryFacade.ExecuteAsync(query);
    }
}
