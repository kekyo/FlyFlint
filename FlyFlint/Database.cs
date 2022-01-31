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
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FlyFlint
{
    public static class Database
    {
        public static readonly Trait Default =
            QueryHelper.DefaultTrait;

        public static readonly Trait SqlServer =
            QueryHelper.DefaultTrait;
        public static readonly Trait Sqlite =
            QueryHelper.DefaultTrait;
        public static readonly Trait MySql =
            QueryHelper.DefaultTrait;
        public static readonly Trait Postgresql =
            QueryHelper.DefaultTrait;
        public static readonly Trait Oracle =
            new Trait(ConversionContext.Default, StringComparer.OrdinalIgnoreCase, ":");

        /////////////////////////////////////////////////////////////////////////////

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Trait CreateTrait(ConversionContext cc) =>
            new Trait(cc, StringComparer.OrdinalIgnoreCase, "@");
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Trait CreateTrait(IComparer<string> fieldComparer) =>
            new Trait(ConversionContext.Default, fieldComparer, "@");
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Trait CreateTrait(string parameterPrefix) =>
            new Trait(ConversionContext.Default, StringComparer.OrdinalIgnoreCase, parameterPrefix);
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Trait CreateTrait(ConversionContext cc, IComparer<string> fieldComparer) =>
            new Trait(cc, fieldComparer, "@");
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Trait CreateTrait(IComparer<string> fieldComparer, string parameterPrefix) =>
            new Trait(ConversionContext.Default, fieldComparer, parameterPrefix);
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Trait CreateTrait(ConversionContext cc, IComparer<string> fieldComparer, string parameterPrefix) =>
            new Trait(cc, fieldComparer, parameterPrefix);
    }
}
