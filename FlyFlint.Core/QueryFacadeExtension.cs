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
using FlyFlint.Internal.Dynamic;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace FlyFlint
{
    public static class QueryFacadeExtension
    {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext Query<TParameters>(
            this DbConnection connection,
            PartialQueryString sql,
            TParameters parameters)
            where TParameters : notnull =>
            new ParameterizedQueryContext(
                connection,
                null,
                FlyFlint.Query.DefaultTrait,
                sql.Sql,
                DynamicQueryExecutorFacade.GetParameters(
                    ref parameters, FlyFlint.Query.DefaultTrait.parameterPrefix));

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext Query<TParameters>(
            this DbConnection connection,
            DbTransaction transaction,
            PartialQueryString sql,
            TParameters parameters)
            where TParameters : notnull =>
            new ParameterizedQueryContext(
                connection,
                transaction,
                FlyFlint.Query.DefaultTrait,
                sql.Sql,
                DynamicQueryExecutorFacade.GetParameters(
                    ref parameters, FlyFlint.Query.DefaultTrait.parameterPrefix));

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext Query<TParameters>(
            this DbConnection connection,
            PreparedPartialQueryContext prepared,
            TParameters parameters)
            where TParameters : notnull
        {
            var built = prepared.builder();
            Debug.Assert(object.ReferenceEquals(built.parameters, DatabaseTrait.defaultParameters));
            return new ParameterizedQueryContext(
                connection,
                null,
                prepared.trait,
                built.sql,
                DynamicQueryExecutorFacade.GetParameters(
                    ref parameters, prepared.trait.parameterPrefix));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext Query<TParameters>(
            this DbConnection connection,
            DbTransaction transaction,
            PreparedPartialQueryContext prepared,
            TParameters parameters)
            where TParameters : notnull
        {
            var built = prepared.builder();
            Debug.Assert(object.ReferenceEquals(built.parameters, DatabaseTrait.defaultParameters));
            return new ParameterizedQueryContext(
                connection,
                transaction,
                prepared.trait,
                built.sql,
                DynamicQueryExecutorFacade.GetParameters(
                    ref parameters, prepared.trait.parameterPrefix));
        }

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext<TElement> Query<TElement, TParameters>(
            this DbConnection connection,
            PreparedPartialQueryContext<TElement> prepared,
            TParameters parameters)
            where TElement : new()
            where TParameters : notnull
        {
            var built = prepared.builder();
            Debug.Assert(object.ReferenceEquals(built.parameters, DatabaseTrait.defaultParameters));
            return new ParameterizedQueryContext<TElement>(
                connection,
                null,
                prepared.trait,
                built.sql,
                DynamicQueryExecutorFacade.GetParameters(
                    ref parameters, prepared.trait.parameterPrefix));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext<TElement> Query<TElement, TParameters>(
            this DbConnection connection,
            DbTransaction transaction,
            PreparedPartialQueryContext<TElement> prepared,
            TParameters parameters)
            where TElement : new()
            where TParameters : notnull
        {
            var built = prepared.builder();
            Debug.Assert(object.ReferenceEquals(built.parameters, DatabaseTrait.defaultParameters));
            return new ParameterizedQueryContext<TElement>(
                connection,
                transaction,
                prepared.trait,
                built.sql,
                DynamicQueryExecutorFacade.GetParameters(
                    ref parameters, prepared.trait.parameterPrefix));
        }

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PreparedParameterizedQueryContext Parameter<TParameters>(
            this PreparedPartialQueryContext prepared,
            Func<TParameters> getter)
            where TParameters : notnull
        {
            var (sql, parameters) = prepared.builder();
            Debug.Assert(object.ReferenceEquals(parameters, DatabaseTrait.defaultParameters));
            var constructParameters = DynamicQueryExecutorFacade.GetConstructParameters(
                getter, prepared.trait.parameterPrefix);
            return new PreparedParameterizedQueryContext(
                prepared.trait,
                () => new QueryParameterBuilderResult(sql, constructParameters()));
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static PreparedParameterizedQueryContext<TElement> Parameter<TElement, TParameters>(
            this PreparedPartialQueryContext<TElement> prepared,
            Func<TParameters> getter)
            where TElement : new()
            where TParameters : notnull
        {
            var (sql, parameters) = prepared.builder();
            Debug.Assert(object.ReferenceEquals(parameters, DatabaseTrait.defaultParameters));
            var constructParameters = DynamicQueryExecutorFacade.GetConstructParameters(
                getter, prepared.trait.parameterPrefix);
            return new PreparedParameterizedQueryContext<TElement>(
                prepared.trait,
                () => new QueryParameterBuilderResult(sql, constructParameters()));
        }

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext Parameter<TParameters>(
            this PartialQueryContext query,
            TParameters parameters)
            where TParameters : notnull =>
            new ParameterizedQueryContext(
                query.connection,
                query.transaction,
                query.trait,
                query.sql,
                DynamicQueryExecutorFacade.GetParameters(
                    ref parameters, query.trait.parameterPrefix));

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ParameterizedQueryContext<TElement> Parameter<TElement, TParameters>(
            this PartialQueryContext<TElement> query,
            TParameters parameters)
            where TElement : new()
            where TParameters : notnull =>
            new ParameterizedQueryContext<TElement>(
                query.connection,
                query.transaction,
                query.trait,
                query.sql,
                DynamicQueryExecutorFacade.GetParameters(
                    ref parameters, query.trait.parameterPrefix));

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<int> ExecuteNonQueryAsync(this QueryContext query) =>
            DynamicQueryExecutorFacade.ExecuteNonQueryAsync(query);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Task<TResult> ExecuteScalarAsync<TResult>(this QueryContext query) =>
            DynamicQueryExecutorFacade.ExecuteScalarAsync<TResult>(query);

#if NET461_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IAsyncEnumerable<TElement> ExecuteAsync<TElement>(this QueryContext<TElement> query)
            where TElement : new() =>
            DynamicQueryExecutorFacade.ExecuteAsync(query);
#else
        [Obsolete("Before net461 platform, it is not supported async enumeration. Consider upgrades to net461 or upper, or `Execute()` method with `FlyFlint.Synchronized` namespace instead.", true)]
        public static void ExecuteAsync<TElement>(this QueryContext<TElement> query)
            where TElement : new() =>
            throw new InvalidOperationException();
#endif
    }
}
