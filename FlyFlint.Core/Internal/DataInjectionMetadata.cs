////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.ComponentModel;

namespace FlyFlint.Internal
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct DataInjectionMetadata
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public DataInjectionMetadata(int index, Type type)
        {
            this.Index = index;
            this.Type = type;
            this.StoreDirect = false;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public readonly int Index;
        [EditorBrowsable(EditorBrowsableState.Never)]
        public readonly Type Type;
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool StoreDirect;
    }
}
