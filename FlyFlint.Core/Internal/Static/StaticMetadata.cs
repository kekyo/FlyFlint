////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.ComponentModel;

namespace FlyFlint.Internal.Static
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IParameterExtractable
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        KeyValuePair<string, object?>[] Extract();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IDataInjectable
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        void Prepare(StaticDataInjectionContext context);
    }
}
