////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System.Runtime.CompilerServices;

namespace FlyFlint.Internal
{
    internal struct QueryParameterBuilderResult
    {
        public readonly string sql;
        public readonly ExtractedParameter[] parameters;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public QueryParameterBuilderResult(string sql, ExtractedParameter[] parameters)
        {
            this.sql = sql;
            this.parameters = parameters;
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Deconstruct(out string sql, out ExtractedParameter[] parameters)
        {
            sql = this.sql;
            parameters = this.parameters;
        }
    }
}
