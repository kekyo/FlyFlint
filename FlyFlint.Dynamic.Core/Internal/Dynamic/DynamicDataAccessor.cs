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
using System.Data.Common;
using System.Text;

namespace FlyFlint.Internal.Dynamic
{
    internal static class DynamicDataAccessor
    {
        private static readonly Encoding encoding = Encoding.UTF8;

        public static object? GetValue(
            IFormatProvider fp, DbDataReader reader, DataInjectionMetadata metadata, Type targetType) =>
            reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? reader.GetValue(metadata.Index) :
                    DynamicValueConverter.GetConverter(targetType).UnsafeConvert(fp, encoding, reader.GetValue(metadata.Index));
    }
}
