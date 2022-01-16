////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Internal.Converter;
using System;

namespace FlyFlint.Internal.Dynamic
{
    internal static class DynamicDataAccessor
    {
        public static object? GetValue(
            DataInjectionContext context, DataInjectionMetadata metadata, Type targetType) =>
            context.reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? context.reader.GetValue(metadata.Index) :
                    DynamicValueConverter.GetConverter(targetType).UnsafeConvert(context, context.reader.GetValue(metadata.Index));
    }
}
