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
using System.Text;

namespace FlyFlint
{
    public sealed class PreparedQueryContext
    {
        internal readonly IFormatProvider fp;
        internal readonly Encoding encoding;
        internal readonly string sql;
        internal readonly (string name, object? value)[] parameters;
        internal readonly string parameterPrefix;

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal PreparedQueryContext(
            IFormatProvider fp,
            Encoding encoding,
            string sql,
            (string name, object? value)[] parameters, 
            string parameterPrefix)
        {
            this.fp = fp;
            this.encoding = encoding;
            this.sql = sql;
            this.parameters = parameters;
            this.parameterPrefix = parameterPrefix;
        }

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedQueryContext Prefix(string parameterPrefix) =>
            new PreparedQueryContext(this.fp, this.encoding, this.sql, this.parameters, parameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedQueryContext Formatter(IFormatProvider fp) =>
            new PreparedQueryContext(fp, this.encoding, this.sql, this.parameters, this.parameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedQueryContext Encoding(Encoding encoding) =>
            new PreparedQueryContext(this.fp, encoding, this.sql, this.parameters, this.parameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedQueryContext<T> Typed<T>()
            where T : new() =>
            new PreparedQueryContext<T>(this.fp, this.encoding, this.sql, this.parameters, this.parameterPrefix);
    }

    public sealed class PreparedQueryContext<T>
        where T : new()
    {
        internal readonly IFormatProvider fp;
        internal readonly Encoding encoding;
        internal readonly string sql;
        internal readonly (string name, object? value)[] parameters;
        internal readonly string parameterPrefix;

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal PreparedQueryContext(
            IFormatProvider fp,
            Encoding encoding,
            string sql,
            (string name, object? value)[] parameters, 
            string parameterPrefix)
        {
            this.fp = fp;
            this.encoding = encoding;
            this.sql = sql;
            this.parameters = parameters;
            this.parameterPrefix = parameterPrefix;
        }

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedQueryContext<T> Prefix(string parameterPrefix) =>
            new PreparedQueryContext<T>(this.fp, this.encoding, this.sql, this.parameters, parameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedQueryContext<T> Formatter(IFormatProvider fp) =>
            new PreparedQueryContext<T>(fp, this.encoding, this.sql, this.parameters, this.parameterPrefix);

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PreparedQueryContext<T> Encoding(Encoding encoding) =>
            new PreparedQueryContext<T>(this.fp, encoding, this.sql, this.parameters, this.parameterPrefix);
    }
}
