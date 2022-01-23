////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Context;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace FlyFlint.Internal.Dynamic
{
    internal abstract class DynamicDataInjectionContext :
        DataInjectionContext
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

        private static object? Convert(
            ConversionContext cc,
            object value,
            Type targetType)
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

        private protected DataInjectionMetadata[] metadataList = null!;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private protected DynamicDataInjectionContext(
            ConversionContext cc,
            IComparer<string> fieldComparer,
            DbDataReader reader) :
            base(cc, fieldComparer, reader)
        {
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public object? GetValue(int metadataIndex, Type targetType)
        {
            var metadata = this.metadataList[metadataIndex];
            return this.reader.IsDBNull(metadata.DbFieldIndex) ? null :
                metadata.StoreDirect ? this.reader.GetValue(metadata.DbFieldIndex) :
                    Convert(this.cc, this.reader.GetValue(metadata.DbFieldIndex), targetType);
        }
    }

    internal sealed class DynamicDataInjectionContext<TElement> :
        DynamicDataInjectionContext
        where TElement : notnull
    {
        private delegate void Setter(ref TElement element);
        private readonly Setter[] setters;

        internal DynamicDataInjectionContext(
            ConversionContext cc,
            IComparer<string> fieldComparer,
            DbDataReader reader) :
            base(cc, fieldComparer, reader)
        {
            // TODO: improve with bulk setter

            var metadataMap =
                QueryHelper.CreateSortedMetadataMap(this.reader, this.fieldComparer);
            this.metadataList = metadataMap.MetadataList;
            var members =
                DynamicHelper.GetSetterMetadataList<TElement>();

            var candidates = new List<Setter>(members.Length);
            for (var index = 0; index < members.Length; index++)
            {
                var member = members[index];
                var dbFieldNameIndiciesIndex = Array.BinarySearch(metadataMap.FieldNames, member.FieldName);
                if (dbFieldNameIndiciesIndex >= 0)
                {
                    var dbFieldMetadata = this.metadataList[dbFieldNameIndiciesIndex];

                    var ut = Nullable.GetUnderlyingType(member.FieldType) ?? member.FieldType;
                    dbFieldMetadata.StoreDirect = ut == dbFieldMetadata.DbType;

                    candidates.Add((ref TElement element) =>
                        member.Accessor(ref element, this.GetValue(dbFieldNameIndiciesIndex, member.FieldType)));
                }
            }

            this.setters = candidates.ToArray();
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Inject(ref TElement element)
        {
            for (var index = 0; index < this.setters.Length; index++)
            {
                this.setters[index](ref element);
            }
        }
    }
}
