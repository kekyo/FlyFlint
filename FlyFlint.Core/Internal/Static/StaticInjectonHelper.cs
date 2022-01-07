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
using System.ComponentModel;
using System.Data.Common;

namespace FlyFlint.Internal.Static
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class StaticInjectonHelper<T>
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static DataInjectionMetadata[] Prepare(
            DbDataReader reader, (string name, Type type)[] members)
        {
            var (dbFieldNames, dbFieldMetadataList) = QueryHelper.GetSortedMetadataMap(reader);

            var candidates = new List<DataInjectionMetadata>(members.Length);
            for (var index = 0; index < members.Length; index++)
            {
                var member = members[index];
                var dbFieldNameIndexesIndex = Array.BinarySearch(dbFieldNames, member.name);
                if (dbFieldNameIndexesIndex >= 0)
                {
                    var dbFieldMetadata = dbFieldMetadataList[dbFieldNameIndexesIndex];

                    var ut = Nullable.GetUnderlyingType(member.type) ?? member.type;
                    dbFieldMetadata.StoreDirect = ut == dbFieldMetadata.Type;

                    candidates.Add(dbFieldMetadata);
                }
            }

            return candidates.ToArray();
        }
    }
}
