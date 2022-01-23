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
using FlyFlint.Internal.Converter;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FlyFlint.Synchronized
{
    public static class QueryFacadeExtension
    {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int ExecuteNonQuery(
            this QueryContext query)
        {
            using var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters);
            return command.ExecuteNonQuery();
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static TElement ExecuteScalar<TElement>(
            this QueryContext<TElement> query)
        {
            using var command = QueryHelper.CreateCommand(
                query.connection, query.transaction, query.sql, query.parameters);
            return InternalValueConverter<TElement>.converter.Convert(
                query.trait.cc, command.ExecuteScalar());
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static IEnumerable<TElement> Execute<TElement>(this QueryContext<TElement> query)
            where TElement : new() =>
            QueryExecutor.Instance.Execute(query);
    }
}
