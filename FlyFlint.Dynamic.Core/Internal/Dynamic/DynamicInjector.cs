////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace FlyFlint.Internal.Dynamic
{
    internal sealed class DynamicInjector<T>
    {
        private delegate void Setter(DbDataReader reader, ref T element);
        private readonly Setter[] setters;

        public DynamicInjector(DbDataReader reader, IFormatProvider fp)
        {
            var (dbFieldNames, dbFieldMetadataList) =
                QueryHelper.GetSortedMetadataMap(reader);
            var members =
                DynamicHelper.GetSetterMetadataList<T>();

            var candidates = new List<Setter>(members.Length);
            for (var index = 0; index < members.Length; index++)
            {
                var member = members[index];
                var dbFieldNameIndiciesIndex = Array.BinarySearch(dbFieldNames, member.name);
                if (dbFieldNameIndiciesIndex >= 0)
                {
                    var dbFieldMetadata = dbFieldMetadataList[dbFieldNameIndiciesIndex];
                    var ut = Nullable.GetUnderlyingType(member.type);
                    var targetType = Nullable.GetUnderlyingType(member.type) ?? member.type;
                    candidates.Add((DbDataReader reader, ref T element) =>
                        member.setter(ref element, DynamicDataAccessor.GetValue(fp, reader, dbFieldMetadata, targetType)));
                }
            }

            this.setters = candidates.ToArray();
        }

#if !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Inject(DbDataReader reader, ref T element)
        {
            for (var index = 0; index < this.setters.Length; index++)
            {
                this.setters[index](reader, ref element);
            }
        }
    }
}
