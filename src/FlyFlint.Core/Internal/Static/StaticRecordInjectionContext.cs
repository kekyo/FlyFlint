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
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace FlyFlint.Internal.Static
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public delegate void StaticRecordInjectorByRefDelegate<TRecord>(
        StaticRecordInjectionContext context, ref TRecord record)
        where TRecord : notnull;  // struct on the runtime

    [EditorBrowsable(EditorBrowsableState.Never)]
    public delegate void StaticRecordInjectorObjRefDelegate<in TRecord>(
        StaticRecordInjectionContext context, TRecord record)
        where TRecord : notnull;  // class on the runtime

    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract partial class StaticRecordInjectionContext :
        RecordInjectionContext
    {
        // HACK: Will contains null RecordInjectionMetadata.
        private protected RecordInjectionMetadata[] metadataList = null!;

        // Don't set outside of context.
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool[] IsAvailable = null!;

        // Don't set outside of context.
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int CurrentOffset;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private protected StaticRecordInjectionContext(
            ConversionContext cc,
            IComparer<string> fieldComparer,
            DbDataReader reader) :
            base(cc, fieldComparer, reader)
        {
        }

        public abstract void RegisterMetadata(
            StaticMemberMetadata[] members,
            Delegate injector);   // StaticInjectDelegate<TRecord>
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class StaticRecordInjectionContext<TRecord> :
        StaticRecordInjectionContext
        where TRecord : notnull
    {
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private protected StaticRecordInjectionContext(
            ConversionContext cc,
            IComparer<string> fieldComparer,
            DbDataReader reader) :
            base(cc, fieldComparer, reader)
        {
        }

        private protected void RegisterMemberMetadata(
#if NET35 || NET40
            IList<StaticMemberMetadata> members)
#else
            IReadOnlyList<StaticMemberMetadata> members)
#endif
        {
            Debug.Assert(this.metadataList == null);    // TODO: combine multiple

            var metadataMap =
                QueryHelper.CreateSortedMetadataMap(this.reader, this.fieldComparer);

            this.metadataList = new RecordInjectionMetadata[members.Count];
            this.IsAvailable = new bool[members.Count];
            for (var index = 0; index < members.Count; index++)
            {
                var member = members[index];
                var dbFieldNameIndiciesIndex =
                    Array.BinarySearch(metadataMap.FieldNames, member.Name, this.fieldComparer);
                if (dbFieldNameIndiciesIndex >= 0)
                {
                    var dbFieldMetadata = metadataMap.MetadataList[dbFieldNameIndiciesIndex];

                    var ut = Nullable.GetUnderlyingType(member.Type) ?? member.Type;
                    dbFieldMetadata.StoreDirect = ut == dbFieldMetadata.DbType;

                    this.metadataList[index] = dbFieldMetadata;
                    this.IsAvailable[index] = true;
                }
            }
        }

        public abstract void MakeInjectable();

        public abstract void Inject(ref TRecord record);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class StaticRecordInjectionByRefContext<TRecord> :
        StaticRecordInjectionContext<TRecord>
        where TRecord : notnull
    {
        private StaticRecordInjectorByRefDelegate<TRecord> injector = null!;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal StaticRecordInjectionByRefContext(
            ConversionContext cc,
            IComparer<string> fieldComparer,
            DbDataReader reader) :
            base(cc, fieldComparer, reader)
        {
            Debug.Assert(typeof(TRecord).IsValueType);
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void RegisterMetadata(
            StaticMemberMetadata[] members,
            Delegate injector)   // StaticRecordInjectorByRefDelegate<TRecord>
        {
            Debug.Assert(this.injector == null);

            this.injector = (StaticRecordInjectorByRefDelegate<TRecord>)injector;
            this.RegisterMemberMetadata(members);
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override void MakeInjectable()
        {
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override void Inject(ref TRecord record)
        {
            Debug.Assert(this.injector != null);
            this.CurrentOffset = 0;
            this.injector!(this, ref record);
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class StaticRecordInjectionObjRefContext<TRecord> :
        StaticRecordInjectionContext<TRecord>
        where TRecord : notnull
    {
        private List<StaticMemberMetadata> members = new();
        private List<int> memberCounts = new();
        private List<StaticRecordInjectorObjRefDelegate<TRecord>> injectors = new();

        private int[] readyMemberCounts = null!;
        private StaticRecordInjectorObjRefDelegate<TRecord>[] readyInjectors = null!;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal StaticRecordInjectionObjRefContext(
            ConversionContext cc,
            IComparer<string> fieldComparer,
            DbDataReader reader) :
            base(cc, fieldComparer, reader)
        {
            Debug.Assert(!typeof(TRecord).IsValueType);
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void RegisterMetadata(
            StaticMemberMetadata[] members,
            Delegate injector)   // StaticRecordInjectorObjRefDelegate<TRecord>
        {
            this.members.AddRange(members);
            this.memberCounts.Add(members.Length);
            this.injectors.Add((StaticRecordInjectorObjRefDelegate<TRecord>)injector);
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override void MakeInjectable()
        {
            this.RegisterMemberMetadata(this.members);

            this.readyMemberCounts = this.memberCounts.ToArray();
            this.readyInjectors = this.injectors.ToArray();

            this.members = null!;
            this.memberCounts = null!;
            this.injectors = null!;
        }

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override void Inject(ref TRecord record)
        {
            Debug.Assert(this.readyInjectors.Length >= 1);
            Debug.Assert(this.readyMemberCounts.Length == this.readyInjectors.Length);

            this.CurrentOffset = 0;
            for (var index = 0; index < this.readyInjectors.Length; index++)
            {
                this.readyInjectors[index](this, record);
                this.CurrentOffset += this.readyMemberCounts[index];
            }
        }
    }
}
