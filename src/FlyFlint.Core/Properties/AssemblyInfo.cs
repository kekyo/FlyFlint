////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FlyFlint")]
[assembly: InternalsVisibleTo("FlyFlint.Dynamic")]
[assembly: InternalsVisibleTo("FSharp.FlyFlint")]

[assembly: InternalsVisibleTo("FlyFlint.Core.Tests")]
[assembly: InternalsVisibleTo("FlyFlint.Dynamic.Core.Tests")]
