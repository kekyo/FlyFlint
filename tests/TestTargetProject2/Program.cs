////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint;
using System.Diagnostics;

namespace TestTargetProject2
{
    public sealed class InheritedTest :
        TestTargetProject.WithoutUsingTypes.TargetReferenceTypesDerived2
    {
        public InheritedTest()
        {
        }
    }

    public static class Program
    {
        public static void Main()
        {
            var targetReferenceTypes = new InheritedTest();
            var q1 = Query.IsRecordInjectable(targetReferenceTypes);
            var q2 = Query.IsParameterExtractable(targetReferenceTypes);
            Debug.Assert(q1);
            Debug.Assert(!q2);

            //Console.WriteLine(targetReferenceTypes);
            var r1 = TestTargetProject.TestClass.InjectExecuteNonQueryWithValueType();
            var r2 = TestTargetProject.TestClass.InjectExecuteNonQueryWithReferenceType();
        }
    }
}
