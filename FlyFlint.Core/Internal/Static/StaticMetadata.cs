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
using System.Data.Common;

namespace FlyFlint.Internal.Static
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IParameterExtractable
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        (string name, object? value)[] Extract();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IDataInjectable
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        DataInjectionMetadata[] PrepareAndInject(IFormatProvider fp, DbDataReader reader);
        [EditorBrowsable(EditorBrowsableState.Never)]
        void Inject(IFormatProvider fp, DataInjectionMetadata[] metadataList, DbDataReader reader);
    }
}
