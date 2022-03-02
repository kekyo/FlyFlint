////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Context;
using FlyFlint.Internal.Static;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;

namespace FlyFlint.Internal.Dynamic
{
    internal sealed class DynamicRecordInjectionContext<TRecord>
        where TRecord : notnull
    {
        private static readonly bool isValueType =
            typeof(TRecord).IsValueType;
        private static readonly StaticMemberMetadata[] members =
            Utilities.GetTargetMembers<TRecord>().
            Select(member => new StaticMemberMetadata(
                member.Name,
                Utilities.DereferenceWhenNullableType(member.Type))).
            ToArray();
        //private static readonly Delegate setter =
        //    isValueType ?
        //        Utilities.CreateSetter<StaticRecordInjectorObjRefDelegate<TRecord>, TRecord>() :
        //        Utilities.CreateSetter<StaticRecordInjectorByRefDelegate<TRecord>, TRecord>();

        private readonly StaticRecordInjectionContext<TRecord> context;

        internal DynamicRecordInjectionContext(
            ConversionContext cc,
            IComparer<string> fieldComparer,
            DbDataReader reader)
        {
            Delegate setter =
                isValueType ?
                    Utilities.CreateSetter<StaticRecordInjectorByRefDelegate<TRecord>, TRecord>() :
                    Utilities.CreateSetter<StaticRecordInjectorObjRefDelegate<TRecord>, TRecord>();

            this.context = isValueType ?
                new StaticRecordInjectionByRefContext<TRecord>(cc, fieldComparer, reader) :
                new StaticRecordInjectionObjRefContext<TRecord>(cc, fieldComparer, reader);
            context.RegisterMetadata(members, setter);
            context.MakeInjectable();
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Inject(ref TRecord record) =>
            this.context.Inject(ref record);
    }
}
