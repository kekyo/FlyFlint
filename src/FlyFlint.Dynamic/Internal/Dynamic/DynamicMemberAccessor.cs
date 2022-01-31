////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using FlyFlint.Context;
using System.Reflection;
using System.Reflection.Emit;

namespace FlyFlint.Internal.Dynamic
{
    internal static class DynamicMemberAccessor
    {
        public static DynamicMethod CreateDirectGetter(FieldInfo fi)
        {
            var fullName = $"MemberAccessor.{fi.DeclaringType!.Namespace}.{fi.DeclaringType!.Name}.get_{fi.Name}";
            var dm = new DynamicMethod(
                fullName,
                typeof(object),
                new[] { fi.DeclaringType.MakeByRefType(), typeof(ConversionContext) }, // TODO: cc is not used
                true);
            var ig = dm.GetILGenerator();
            ig.Emit(OpCodes.Ldarg_0);
            if (!fi.DeclaringType.IsValueType)
            {
                ig.Emit(OpCodes.Ldind_Ref);
            }
            ig.Emit(OpCodes.Ldfld, fi);
            if (fi.FieldType.IsValueType)
            {
                ig.Emit(OpCodes.Box, fi.FieldType);
            }
            ig.Emit(OpCodes.Ret);
            return dm;
        }

        public static DynamicMethod CreateDirectSetter(FieldInfo fi)
        {
            var fullName = $"MemberAccessor.{fi.DeclaringType!.Namespace}.{fi.DeclaringType!.Name}.set_{fi.Name}";
            var dm = new DynamicMethod(
                fullName,
                typeof(void),
                new[] { fi.DeclaringType.MakeByRefType(), typeof(object) },
                true);
            var ig = dm.GetILGenerator();
            ig.Emit(OpCodes.Ldarg_0);
            if (!fi.DeclaringType.IsValueType)
            {
                ig.Emit(OpCodes.Ldind_Ref);
            }
            ig.Emit(OpCodes.Ldarg_1);
            if (fi.FieldType.IsValueType)
            {
                ig.Emit(OpCodes.Unbox_Any, fi.FieldType);
            }
            else
            {
                ig.Emit(OpCodes.Castclass, fi.FieldType);
            }
            ig.Emit(OpCodes.Stfld, fi);
            ig.Emit(OpCodes.Ret);
            return dm;
        }

        public static DynamicMethod CreateDirectGetter(PropertyInfo pi)
        {
            var fullName = $"MemberAccessor.{pi.DeclaringType!.Namespace}.{pi.DeclaringType!.Name}.get_{pi.Name}";
            var dm = new DynamicMethod(
                fullName,
                typeof(object),
                new[] { pi.DeclaringType.MakeByRefType(), typeof(ConversionContext) }, // TODO: cc is not used
                true);
            var ig = dm.GetILGenerator();
            var getter = pi.GetGetMethod(true)!;
            ig.Emit(OpCodes.Ldarg_0);
            if (!pi.DeclaringType.IsValueType)
            {
                ig.Emit(OpCodes.Ldind_Ref);
            }
            ig.Emit(getter.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, getter);
            if (pi.PropertyType.IsValueType)
            {
                ig.Emit(OpCodes.Box, pi.PropertyType);
            }
            ig.Emit(OpCodes.Ret);
            return dm;
        }

        public static DynamicMethod CreateDirectSetter(PropertyInfo pi)
        {
            var fullName = $"MemberAccessor.{pi.DeclaringType!.Namespace}.{pi.DeclaringType!.Name}.set_{pi.Name}";
            var dm = new DynamicMethod(
                fullName,
                typeof(void),
                new[] { pi.DeclaringType.MakeByRefType(), typeof(object) },
                true);
            var ig = dm.GetILGenerator();
            var setter = pi.GetSetMethod(true)!;
            ig.Emit(OpCodes.Ldarg_0);
            if (!pi.DeclaringType.IsValueType)
            {
                ig.Emit(OpCodes.Ldind_Ref);
            }
            ig.Emit(OpCodes.Ldarg_1);
            if (pi.PropertyType.IsValueType)
            {
                ig.Emit(OpCodes.Unbox_Any, pi.PropertyType);
            }
            else
            {
                ig.Emit(OpCodes.Castclass, pi.PropertyType);
            }
            ig.Emit(setter.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, setter);
            ig.Emit(OpCodes.Ret);
            return dm;
        }
    }
}
