////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Context;
using System.Runtime.CompilerServices;

namespace FlyFlint
{
    public sealed class PreparedQueryContext
    {
        internal readonly ConversionContext cc;
        internal readonly string sql;
        internal readonly (string name, object? value)[] parameters;
        internal readonly string parameterPrefix;

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal PreparedQueryContext(
            ConversionContext cc,
            string sql,
            (string name, object? value)[] parameters, 
            string parameterPrefix)
        {
            this.cc = cc;
            this.sql = sql;
            this.parameters = parameters;
            this.parameterPrefix = parameterPrefix;
        }

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedQueryContext Prefix(string parameterPrefix) =>
            new PreparedQueryContext(this.cc, this.sql, this.parameters, parameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedQueryContext Conversion(ConversionContext cc) =>
            new PreparedQueryContext(cc, this.sql, this.parameters, this.parameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedQueryContext<T> Typed<T>()
            where T : new() =>
            new PreparedQueryContext<T>(this.cc, this.sql, this.parameters, this.parameterPrefix);
    }

    public sealed class PreparedQueryContext<T>
        where T : new()
    {
        internal readonly ConversionContext cc;
        internal readonly string sql;
        internal readonly (string name, object? value)[] parameters;
        internal readonly string parameterPrefix;

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal PreparedQueryContext(
            ConversionContext cc,
            string sql,
            (string name, object? value)[] parameters, 
            string parameterPrefix)
        {
            this.cc = cc;
            this.sql = sql;
            this.parameters = parameters;
            this.parameterPrefix = parameterPrefix;
        }

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedQueryContext<T> Prefix(string parameterPrefix) =>
            new PreparedQueryContext<T>(this.cc, this.sql, this.parameters, parameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedQueryContext<T> Conversion(ConversionContext cc) =>
            new PreparedQueryContext<T>(cc, this.sql, this.parameters, this.parameterPrefix);
    }
}
