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
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace FlyFlint
{
    public static class Database
    {
        public static readonly Trait Default =
            new Trait(ConversionContext.Default, StringComparer.OrdinalIgnoreCase, "@");

        public static readonly Trait SqlServer =
            Default;
        public static readonly Trait Sqlite =
            Default;
        public static readonly Trait MySql =
            Default;
        public static readonly Trait Postgresql =
            Default;
        public static readonly Trait Oracle =
            new Trait(ConversionContext.Default, StringComparer.OrdinalIgnoreCase, ":");
    }
}
