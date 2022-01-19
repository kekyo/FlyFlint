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
using System.Runtime.CompilerServices;

namespace FlyFlint.Internal.Dynamic
{
    internal sealed class DynamicInjector<T>
    {
        private delegate void Setter(ref T element);
        private readonly Setter[] setters;

        public DynamicInjector(DynamicDataInjectionContext context)
        {
            var metadataMap =
                QueryHelper.CreateSortedMetadataMap(context.reader, context.fieldComparer);
            var members =
                DynamicHelper.GetSetterMetadataList<T>();

            var candidates = new List<Setter>(members.Length);
            for (var index = 0; index < members.Length; index++)
            {
                var member = members[index];
                var dbFieldNameIndiciesIndex = Array.BinarySearch(metadataMap.FieldNames, member.FieldName);
                if (dbFieldNameIndiciesIndex >= 0)
                {
                    var dbFieldMetadata = metadataMap.MetadataList[dbFieldNameIndiciesIndex];

                    var ut = Nullable.GetUnderlyingType(member.FieldType) ?? member.FieldType;
                    dbFieldMetadata.StoreDirect = ut == dbFieldMetadata.Type;

                    candidates.Add((ref T element) =>
                        member.Accessor(ref element, context.GetValue(dbFieldMetadata, member.FieldType)));
                }
            }

            this.setters = candidates.ToArray();
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Inject(ref T element)
        {
            for (var index = 0; index < this.setters.Length; index++)
            {
                this.setters[index](ref element);
            }
        }
    }
}
