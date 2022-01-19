////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace FlyFlint.Context
{
    internal sealed class DynamicDataInjectionContext
    {
        private static readonly Dictionary<Type, Func<ConversionContext, object, object?>> converts = new();

        private abstract class Converter
        {
            public abstract object? Convert(ConversionContext cc, object value);
        }

        private sealed class Converter<T> : Converter
        {
            public override object? Convert(ConversionContext cc, object value) =>
                cc.Convert<T>(value);
        }

        private static object? Convert(ConversionContext cc, object value, Type targetType)
        {
            Func<ConversionContext, object, object?>? convert;
            lock (converts)
            {
                if (!converts.TryGetValue(targetType, out convert))
                {
                    var converter = (Converter)Activator.CreateInstance(
                        typeof(Converter<>).MakeGenericType(targetType))!;
                    convert = converter.Convert;
                    converts.Add(targetType, convert);
                }
            }
            return convert(cc, value);
        }

        internal readonly ConversionContext cc;
        internal readonly IComparer<string> fieldComparer;
        internal readonly DbDataReader reader;

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal DynamicDataInjectionContext(
            ConversionContext cc, IComparer<string> fieldComparer, DbDataReader reader)
        {
            this.cc = cc;
            this.fieldComparer = fieldComparer;
            this.reader = reader;
        }

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public object? GetValue(DataInjectionMetadata metadata, Type targetType) =>
            this.reader.IsDBNull(metadata.Index) ? null :
                metadata.StoreDirect ? this.reader.GetValue(metadata.Index) :
                    Convert(this.cc, this.reader.GetValue(metadata.Index), targetType);
    }
}
