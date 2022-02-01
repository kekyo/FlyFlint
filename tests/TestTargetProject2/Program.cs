////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint;
using FlyFlint.Internal.Static;
using System;
using System.Threading.Tasks;

namespace TestTargetProject2
{
    public sealed class Inherited : TestTargetProject.TestClass.TargetReferenceTypesDerived2
    {
        public Inherited()
        {
        }
    }

    public static class Program
    {
        public static void Main()
        {
            //var targetValueTypes = new TestTargetProject.TestClass.TargetValueTypes();
            var targetReferenceTypes = new TestTargetProject.TestClass.TargetReferenceTypesDerived2();
            var q1 = Query.IsRecordInjectable(targetReferenceTypes);
            var q2 = Query.IsParameterExtractable(targetReferenceTypes);
            //Console.WriteLine(targetReferenceTypes);
            var r = TestTargetProject.TestClass.InjectExecuteNonQueryWithValueType();
        }
    }
}
