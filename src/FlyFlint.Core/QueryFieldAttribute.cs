////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.CompilerServices;

namespace FlyFlint
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class QueryFieldAttribute : Attribute
    {
        public readonly string? Name;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public QueryFieldAttribute(string? name = null) =>
            this.Name = name;
    }
}
