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

namespace FlyFlint.Internal
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct ExtractedParameter
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public readonly string Name;
        [EditorBrowsable(EditorBrowsableState.Never)]
        public readonly object? Value;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ExtractedParameter(string name, object? value)
        {
            this.Name = name;
            this.Value = value;
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class RecordInjectionMetadata
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public readonly int DbFieldIndex;
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool StoreDirect;
        [EditorBrowsable(EditorBrowsableState.Never)]
        public readonly Type DbType;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public RecordInjectionMetadata(int dbfieldIndex, Type dbType)
        {
            this.DbFieldIndex = dbfieldIndex;
            this.StoreDirect = false;
            this.DbType = dbType;
        }
    }
}
