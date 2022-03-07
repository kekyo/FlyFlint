////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Diagnostics;
using System.Linq;

namespace FlyFlint
{
    partial class Injector
    {
        private FieldReference InjectStaticMemberField(
            ModuleDefinition module,
            TypeDefinition targetType, MethodDefinition cctor, MemberReference[] targetMembers)
        {
            var staticMemberMetadatasType = new ArrayType(
                module.ImportReference(this.staticMemberMetadataType));

            var membersField = new FieldDefinition(
                "flyflint_members__",
                FieldAttributes.Private | FieldAttributes.Static | FieldAttributes.InitOnly,
                staticMemberMetadatasType);
            membersField.CustomAttributes.Add(
                new CustomAttribute(
                    module.ImportReference(this.compilerGeneratedAttributeConstructor)));
            targetType.Fields.Add(membersField);

            var cctorInsts = cctor.Body.Instructions;

            var instIndex = 0;
            cctorInsts.Insert(instIndex++,
                Instruction.Create(OpCodes.Ldc_I4_S, (sbyte)targetMembers.Length));
            cctorInsts.Insert(instIndex++,
                Instruction.Create(
                    OpCodes.Newarr,
                    module.ImportReference(this.staticMemberMetadataType)));

            for (var metadataIndex = 0; metadataIndex < targetMembers.Length; metadataIndex++)
            {
                var targetMember = targetMembers[metadataIndex];

                cctorInsts.Insert(instIndex++,
                    Instruction.Create(OpCodes.Dup));
                cctorInsts.Insert(instIndex++,
                    Instruction.Create(OpCodes.Ldc_I4_S, (sbyte)metadataIndex));
                cctorInsts.Insert(instIndex++,
                    Instruction.Create(
                        OpCodes.Ldstr,
                        Utilities.GetTargetMemberName(targetMember)));
                cctorInsts.Insert(instIndex++,
                    Instruction.Create(
                        OpCodes.Ldtoken,
                        Utilities.DereferenceWhenNullableType(
                            module,
                            Utilities.GetMemberType(module, targetMember))));
                cctorInsts.Insert(instIndex++,
                    Instruction.Create(
                        OpCodes.Call,
                        module.ImportReference(this.getTypeFromHandleMethod)));
                cctorInsts.Insert(instIndex++,
                    Instruction.Create(
                        OpCodes.Newobj,
                        module.ImportReference(this.staticMemberMetadataConstructor)));
                cctorInsts.Insert(instIndex++,
                    Instruction.Create(
                        OpCodes.Stelem_Any,
                        module.ImportReference(this.staticMemberMetadataType)));
            }

            cctorInsts.Insert(instIndex++,
                Instruction.Create(OpCodes.Stsfld, membersField));

            return module.ImportReference(membersField);
        }

        ////////////////////////////////////////////////////////////////////////////

        private FieldReference InjectInjectorMethod(
            ModuleDefinition module,
            TypeDefinition targetType, MethodDefinition cctor, MemberReference[] targetMembers)
        {
            var staticRecordInjectorDelegateType =
                new GenericInstanceType(
                    module.ImportReference(
                        targetType.IsValueType ?
                            this.staticRecordInjectorByRefDelegateType :
                            this.staticRecordInjectorObjRefDelegateType));
            staticRecordInjectorDelegateType.GenericArguments.
                Add(targetType);

            // Couldn't get constructor reference from instantiated type directly.
            var staticRecordInjectorDelegateConstructor =
                new MethodReference(
                    ".ctor",
                    this.typeSystem.Void,
                    staticRecordInjectorDelegateType);
            staticRecordInjectorDelegateConstructor.HasThis = true;    // Important
            staticRecordInjectorDelegateConstructor.Parameters.Add(
                new ParameterDefinition(this.typeSystem.Object));
            staticRecordInjectorDelegateConstructor.Parameters.Add(
                new ParameterDefinition(this.typeSystem.IntPtr));

            var injectorField = new FieldDefinition(
                "flyflint_injector__",
                FieldAttributes.Private | FieldAttributes.Static | FieldAttributes.InitOnly,
                module.ImportReference(this.delegateType));
            injectorField.CustomAttributes.Add(
                new CustomAttribute(
                    module.ImportReference(this.compilerGeneratedAttributeConstructor)));
            targetType.Fields.Add(injectorField);

            //////////////////////////////////////////////

            var injectMethod = new MethodDefinition(
                "flyflint_inject__",
                MethodAttributes.HideBySig | MethodAttributes.Private | MethodAttributes.Static,
                this.typeSystem.Void);
            injectMethod.Parameters.Add(
                new ParameterDefinition(
                    "context",
                    ParameterAttributes.None,
                    module.ImportReference(this.staticRecordInjectionContextType)));
            injectMethod.Parameters.Add(
                new ParameterDefinition(
                    "record",
                    ParameterAttributes.None,
                    targetType.IsValueType ?
                        new ByReferenceType(targetType) :   // ref TRecord record
                        targetType));                       // TRecord record
            injectMethod.ImplAttributes = MethodImplAttributes.AggressiveInlining;
            injectMethod.CustomAttributes.Add(
                new CustomAttribute(
                    module.ImportReference(this.compilerGeneratedAttributeConstructor)));
            targetType.Methods.Add(injectMethod);

            var injectMethodInsts = injectMethod.Body.Instructions;

            ////////////////////////////////////////////////////////////////
            // var offset = context.CurrentOffset;

            if (!targetType.IsValueType)
            {
                injectMethod.Body.Variables.Add(
                    new VariableDefinition(this.typeSystem.Int32));

                injectMethodInsts.Add(
                    Instruction.Create(OpCodes.Ldarg_0));
                injectMethodInsts.Add(
                    Instruction.Create(
                        OpCodes.Ldfld,
                        module.ImportReference(this.staticRecordInjectionContextCurrentOffsetField)));
                injectMethodInsts.Add(
                    Instruction.Create(OpCodes.Stloc_0));
            }

            ////////////////////////////////////////////////////////////////
            // var isAvailable = context.IsAvailable;

            injectMethodInsts.Add(
                Instruction.Create(OpCodes.Ldarg_0));
            injectMethodInsts.Add(
                Instruction.Create(
                    OpCodes.Ldfld,
                    module.ImportReference(this.staticRecordInjectionContextIsAvailableField)));

            ////////////////////////////////////////////////////////////////

            MethodReference GetValueMethod(
                MemberReference targetMember, TypeReference memberType)
            {
                var isNullable = Utilities.IsNullableForMember(targetMember, memberType);
                var key = new TypeKey(memberType, isNullable);

                if (this.getValueMethods.TryGetValue(key, out var gvm))
                {
                    return module.ImportReference(gvm);
                }
                else
                {
                    var m = new GenericInstanceMethod(
                        module.ImportReference(
                            (isNullable, memberType.IsValueType) switch
                            {
                                (false, false) => this.getObjRefValueMethod,
                                (false, true) => this.getValueMethod,
                                (true, false) => this.getNullableObjRefValueMethod,
                                (true, true) => this.getNullableValueMethod,
                            }));
                    m.GenericArguments.Add(
                        Utilities.DereferenceWhenNullableType(
                            module,
                            memberType));
                    return m;
                }
            }

            var dummy = Instruction.Create(OpCodes.Nop);
            var branchTargetInstructions = new Instruction[targetMembers.Length + 1];
            var fixupBranchTargets = new Action[targetMembers.Length];

            for (var metadataIndex = 0; metadataIndex < targetMembers.Length; metadataIndex++)
            {
                // if (isAvailable[metadataIndex + offset])

                var topOfIfExpression =
                    Instruction.Create(OpCodes.Dup);
                injectMethodInsts.Add(topOfIfExpression);

                branchTargetInstructions[metadataIndex] = topOfIfExpression;

                injectMethodInsts.Add(
                    Instruction.Create(OpCodes.Ldc_I4_S, (sbyte)metadataIndex));

                if (!targetType.IsValueType)
                {
                    injectMethodInsts.Add(
                        Instruction.Create(OpCodes.Ldloc_0));
                    injectMethodInsts.Add(
                        Instruction.Create(OpCodes.Add));
                }

                injectMethodInsts.Add(
                    Instruction.Create(OpCodes.Ldelem_U1));

                ////////////////////////

                // Captured current indices.
                var pi = injectMethodInsts.Count;
                var mi = metadataIndex + 1;
                fixupBranchTargets[metadataIndex] = () =>
                {
                    // Fixup branch target with next top of block expression.
                    injectMethodInsts[pi] =
                        Instruction.Create(OpCodes.Brfalse_S, branchTargetInstructions[mi]);
                };

                // Set dummy branch target, because it's forwarded declaration.
                injectMethodInsts.Add(
                    Instruction.Create(OpCodes.Brfalse_S, dummy));

                ////////////////////////

                var targetMember = targetMembers[metadataIndex];

                var memberType = Utilities.GetMemberType(module, targetMember);
                var getValueMethod = GetValueMethod(targetMember, memberType);

                injectMethodInsts.Add(
                    Instruction.Create(OpCodes.Ldarg_1));
                injectMethodInsts.Add(
                    Instruction.Create(OpCodes.Ldarg_0));
                injectMethodInsts.Add(
                    Instruction.Create(OpCodes.Ldc_I4_S, (sbyte)metadataIndex));

                if (!targetType.IsValueType)
                {
                    injectMethodInsts.Add(
                        Instruction.Create(OpCodes.Ldloc_0));
                    injectMethodInsts.Add(
                        Instruction.Create(OpCodes.Add));
                }

                injectMethodInsts.Add(
                    Instruction.Create(OpCodes.Call, getValueMethod));

                if (targetMember is FieldDefinition f)
                {
                    injectMethodInsts.Add(
                        Instruction.Create(OpCodes.Stfld, f));
                }
                else
                {
                    Debug.Assert(targetMember is PropertyReference);
                    var p = (PropertyDefinition)targetMember;
                    var sm = p.SetMethod;
                    Debug.Assert(sm != null);

                    injectMethodInsts.Add(
                        Instruction.Create(
                            sm!.IsVirtual ? OpCodes.Callvirt : OpCodes.Call,
                            sm));
                }
            }

            // Remove isAvailable.
            var lastTargetInstruction = Instruction.Create(OpCodes.Pop);
            injectMethodInsts.Add(lastTargetInstruction);

            // Set last instrunction
            branchTargetInstructions[targetMembers.Length] = lastTargetInstruction;

            injectMethodInsts.Add(
                Instruction.Create(OpCodes.Ret));

            // Fixup branch target.
            foreach (var fixupLabel in fixupBranchTargets)
            {
                fixupLabel();
            }

            ////////////////////////////////////////////////////////////////

            var cctorInsts = cctor.Body.Instructions;

            var instIndex = 0;
            cctorInsts.Insert(instIndex++,
                Instruction.Create(OpCodes.Ldnull));
            cctorInsts.Insert(instIndex++,
                Instruction.Create(OpCodes.Ldftn, injectMethod));
            cctorInsts.Insert(instIndex++,
                Instruction.Create(OpCodes.Newobj, staticRecordInjectorDelegateConstructor));
            cctorInsts.Insert(instIndex++,
                Instruction.Create(OpCodes.Stsfld, injectorField));

            return module.ImportReference(injectorField);
        }

        ////////////////////////////////////////////////////////////////////////////

        private InjectResults InjectPrepareMethod(
            ModuleDefinition module,
            TypeDefinition targetType)
        {
            // Already injected?
            if (targetType.CustomAttributes.Any(ca =>
                ca.AttributeType.FullName == "FlyFlint.Internal.Static.RecordInjectableInjectedAttribute"))
            {
                return InjectResults.Ignored;
            }

            var targetFields = targetType.Fields.
                Where(f => !f.IsInitOnly && Utilities.IsTargetMember(f)).
                Cast<MemberReference>();
            var targetProperties = targetType.Properties.
                Where(p => Utilities.IsTargetMember(p, true)).
                Cast<MemberReference>();

            var targetMembers = targetFields.Concat(targetProperties).ToArray();
            if (targetMembers.Length == 0)
            {
                return InjectResults.Ignored;
            }

            var requiredOverrideMethod = targetType.
                Traverse(t => t.BaseType?.Resolve()).
                SelectMany(t => t.Methods.Where(m =>
                    m.IsPublic && m.IsVirtual && m.IsHideBySig && (m.Name == "Prepare"))).
                FirstOrDefault();

            // Injected manually (?)
            if (requiredOverrideMethod != null &&
                requiredOverrideMethod.DeclaringType == targetType)
            {
                return InjectResults.Ignored;
            }

            // Check injection target where modules.
            if (targetType.Module.Assembly.FullName != module.Assembly.FullName)
            {
                return InjectResults.CouldNot;
            }

            //////////////////////////////////////////////

            // Detect type initializer or define here.
            var cctor = targetType.Methods.
                FirstOrDefault(m => m.IsStatic && m.IsConstructor);
            if (cctor == null)
            {
                cctor = new MethodDefinition(
                    ".cctor",
                    MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName | MethodAttributes.Public | MethodAttributes.Static,
                    this.typeSystem.Void);
                cctor.CustomAttributes.Add(
                    new CustomAttribute(
                        module.ImportReference(this.compilerGeneratedAttributeConstructor)));
                targetType.Methods.Add(cctor);
                cctor.Body.Instructions.Add(
                    Instruction.Create(OpCodes.Ret));
            }

            //////////////////////////////////////////////

            // Inject metadatas.
            var injectorField = InjectInjectorMethod(module, targetType, cctor, targetMembers);
            var membersField = InjectStaticMemberField(module, targetType, cctor, targetMembers);

            //////////////////////////////////////////////

            // Define `Prepare` method.
            var prepareMethod = new MethodDefinition(
                // The CLR and CoreCLR, will cause TypeLoadException when uses different name in inferface member method...
                "Prepare",
                MethodAttributes.HideBySig | MethodAttributes.Public | MethodAttributes.Virtual |
                    ((requiredOverrideMethod != null) ?
                        MethodAttributes.ReuseSlot :
                        MethodAttributes.NewSlot),
                this.typeSystem.Void);
            prepareMethod.Parameters.Add(
                new ParameterDefinition(
                    "context",
                    ParameterAttributes.None,
                    module.ImportReference(this.staticRecordInjectionContextType)));
            prepareMethod.ImplAttributes = MethodImplAttributes.AggressiveInlining;
            prepareMethod.CustomAttributes.Add(
                new CustomAttribute(
                    module.ImportReference(this.compilerGeneratedAttributeConstructor)));
            targetType.Methods.Add(prepareMethod);

            //////////////////////////////////////////////

            // Inject `Prepare` method body.
            var prepareMethodInsts = prepareMethod.Body.Instructions;

            if (requiredOverrideMethod != null)
            {
                // Chaining prepare methods.
                prepareMethodInsts.Add(
                    Instruction.Create(OpCodes.Ldarg_0));
                prepareMethodInsts.Add(
                    Instruction.Create(OpCodes.Ldarg_1));
                prepareMethodInsts.Add(
                    Instruction.Create(
                        OpCodes.Call,
                        module.ImportReference(requiredOverrideMethod)));
            }

            prepareMethodInsts.Add(
                Instruction.Create(OpCodes.Ldarg_1));
            prepareMethodInsts.Add(
                Instruction.Create(OpCodes.Ldsfld, membersField));
            prepareMethodInsts.Add(
                Instruction.Create(OpCodes.Ldsfld, injectorField));
            prepareMethodInsts.Add(
                Instruction.Create(
                    OpCodes.Callvirt,
                    module.ImportReference(this.registerMetadataMethod)));
            prepareMethodInsts.Add(
                Instruction.Create(OpCodes.Ret));

            //////////////////////////////////////////////

            // Implements interface.
            if (requiredOverrideMethod == null)
            {
                var ii = new InterfaceImplementation(
                    module.ImportReference(this.recordInjectableType));
                targetType.Interfaces.Add(ii);

                prepareMethod.Overrides.Add(
                    module.ImportReference(this.prepareMethod));
            }

            // Mark injected.
            var injectedAttribute = new CustomAttribute(
                module.ImportReference(this.recordInjectableInjectedAttributeConstructor));
            injectedAttribute.ConstructorArguments.Add(
                new CustomAttributeArgument(
                    this.typeSystem.String, ThisAssembly.AssemblyVersion));
            targetType.CustomAttributes.Add(injectedAttribute);

            return InjectResults.Success;
        }
    }
}
