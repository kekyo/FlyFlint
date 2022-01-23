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
    public sealed class DataInjectionMetadata
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public DataInjectionMetadata(int dbfieldIndex, Type dbType)
        {
            this.DbFieldIndex = dbfieldIndex;
            this.StoreDirect = false;
            this.DbType = dbType;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public readonly int DbFieldIndex;
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool StoreDirect;
        [EditorBrowsable(EditorBrowsableState.Never)]
        public readonly Type DbType;
    }
}
