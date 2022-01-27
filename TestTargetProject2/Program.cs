////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace TestTargetProject2
{
    public sealed class Inherited : TestTargetProject.TestClass.TargetReferenceTypes
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
            var targetReferenceTypes = new TestTargetProject.TestClass.TargetReferenceTypes();
            Console.WriteLine(targetReferenceTypes);
        }
    }
}
