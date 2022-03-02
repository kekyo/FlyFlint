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
using System.Runtime.CompilerServices;

namespace FlyFlint.Internal.Static
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct StaticMemberMetadata
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public readonly string Name;
        [EditorBrowsable(EditorBrowsableState.Never)]
        public readonly Type NullableUnwrappedType;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public StaticMemberMetadata(string name, Type nullableUnwrappedType)
        {
            this.Name = name;
            this.NullableUnwrappedType = nullableUnwrappedType;
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IParameterExtractable
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        void Extract(StaticParameterExtractionContext context);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IRecordInjectable
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        void Prepare(StaticRecordInjectionContext context);
    }
}
