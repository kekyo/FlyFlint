////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Context;
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
    public delegate void InjectDelegate<TElement>(
        ref TElement element,
        DataInjectionContext context,
        DataInjectionMetadata[] metadataList);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct PreparingResult<TElement>
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public readonly InjectDelegate<TElement> Injector;
        [EditorBrowsable(EditorBrowsableState.Never)]
        public readonly DataInjectionMetadata[] MetadataList;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public PreparingResult(
            InjectDelegate<TElement> injector, DataInjectionMetadata[] metadataList)
        {
            this.Injector = injector;
            this.MetadataList = metadataList;
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IDataInjectable<TElement>
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        PreparingResult<TElement> Prepare(DataInjectionContext context);
    }
}
