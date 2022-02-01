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
using System.Runtime.CompilerServices;

namespace FlyFlint.Synchronized
{
    public static class QueryExtension
    {
        private static int InternalExecuteNonQuery(
            this QueryContext query)
        {
            using var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters);
            return command.ExecuteNonQuery();
        }

        private static TRecord InternalExecuteScalar<TRecord>(
            this QueryContext<TRecord> query)
        {
            using var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters);
            return QueryExecutor.ConvertTo<TRecord>(
                query.trait.cc, command.ExecuteScalar());
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int ExecuteNonQuery(
            this ParameterizedQueryContext query) =>
            InternalExecuteNonQuery(query);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static TRecord ExecuteScalar<TRecord>(
            this ParameterizedQueryContext<TRecord> query) =>
            InternalExecuteScalar(query);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int ExecuteNonQueryNonParameterized(
            this PartialQueryContext query) =>
            InternalExecuteNonQuery(query);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static TRecord ExecuteScalarNonParameterized<TRecord>(
            this PartialQueryContext<TRecord> query) =>
            InternalExecuteScalar(query);
    }
}
