////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

#if !(NET40_OR_GREATER || NETSTANDARD2_0_OR_GREATER || NETCOREAPP2_0_OR_GREATER)
namespace System
{
    internal struct ValueTuple<T1, T2>
    {
        public readonly T1 Item1;
        public readonly T2 Item2;

        public ValueTuple(T1 value1, T2 value2)
        {
            this.Item1 = value1;
            this.Item2 = value2;
        }
    }

    internal struct ValueTuple<T1, T2, T3>
    {
        public readonly T1 Item1;
        public readonly T2 Item2;
        public readonly T3 Item3;

        public ValueTuple(T1 value1, T2 value2, T3 value3)
        {
            this.Item1 = value1;
            this.Item2 = value2;
            this.Item3 = value3;
        }
    }
}

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Field)]
    internal sealed class TupleElementNamesAttribute : Attribute
    {
        public TupleElementNamesAttribute(string[] names)
        { }
    }
}
#endif
