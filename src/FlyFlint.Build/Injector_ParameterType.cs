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
using System.Diagnostics;
using System.Linq;

namespace FlyFlint
{
    partial class Injector
    {
        private InjectResults InjectExtractMethod(
            ModuleDefinition module,
            TypeDefinition targetType)
        {
            // Already injected?
            if (targetType.CustomAttributes.Any(ca =>
                ca.AttributeType.FullName == "FlyFlint.Internal.Static.ParameterExtractableInjectedAttribute"))
            {
                return InjectResults.Ignored;
            }

            var targetFields = targetType.Fields.
                Where(f => Utilities.IsTargetMember(f)).
                Cast<MemberReference>();
            var targetProperties = targetType.Properties.
                Where(p => Utilities.IsTargetMember(p, false)).
                Cast<MemberReference>();

            var targetMembers = targetFields.Concat(targetProperties).ToArray();
            if (targetMembers.Length == 0)
            {
                return InjectResults.Ignored;
            }

            var requiredOverrideMethod = targetType.
                Traverse(t => t.BaseType?.Resolve()).
                SelectMany(t => t.Methods.Where(m =>
                    m.IsPublic && m.IsVirtual && m.IsHideBySig && (m.Name == "Extract"))).
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

            // Define `Extract` method.
            var extractMethod = new MethodDefinition(
                // The CLR and CoreCLR, will cause TypeLoadException when uses different name in inferface member method...
                "Extract",
                MethodAttributes.HideBySig | MethodAttributes.Public | MethodAttributes.Virtual |
                    ((requiredOverrideMethod != null) ?
                        MethodAttributes.ReuseSlot :
                        MethodAttributes.NewSlot),
                this.typeSystem.Void);
            extractMethod.Parameters.Add(
                new ParameterDefinition(
                    "context",
                    ParameterAttributes.None,
                    module.ImportReference(this.staticParameterExtractionContextType)));
            extractMethod.ImplAttributes = MethodImplAttributes.AggressiveInlining;
            extractMethod.CustomAttributes.Add(
                new CustomAttribute(
                    module.ImportReference(this.compilerGeneratedAttributeConstructor)));
            targetType.Methods.Add(extractMethod);

            //////////////////////////////////////////////
            
            // Inject `Extract` method body.
            var extractMethodInsts = extractMethod.Body.Instructions;

            if (requiredOverrideMethod != null)
            {
                // Chaining extract methods.
                extractMethodInsts.Add(
                    Instruction.Create(OpCodes.Ldarg_0));
                extractMethodInsts.Add(
                    Instruction.Create(OpCodes.Ldarg_1));
                extractMethodInsts.Add(
                    Instruction.Create(
                        OpCodes.Call,
                        module.ImportReference(requiredOverrideMethod)));
            }

            for (var metadataIndex = 0; metadataIndex < targetMembers.Length; metadataIndex++)
            {
                var targetMember = targetMembers[metadataIndex];

                var memberType = Utilities.GetMemberType(module, targetMember);

                extractMethodInsts.Add(
                    Instruction.Create(OpCodes.Ldarg_1));
                extractMethodInsts.Add(
                    Instruction.Create(
                        OpCodes.Ldstr,
                        Utilities.GetTargetMemberName(targetMember)));
                extractMethodInsts.Add(
                    Instruction.Create(OpCodes.Ldarg_0));

                MethodReference setParameterMethod;
                if (targetMember is FieldReference fr)
                {
                    if (memberType.IsValueType)
                    {
                        extractMethodInsts.Add(
                            Instruction.Create(
                                OpCodes.Ldflda,
                                module.ImportReference(fr)));
                        setParameterMethod = this.setByRefParameterMethod;
                    }
                    else
                    {
                        extractMethodInsts.Add(
                            Instruction.Create(
                                OpCodes.Ldfld,
                                module.ImportReference(fr)));
                        setParameterMethod = this.setByValParameterMethod;
                    }
                }
                else
                {
                    Debug.Assert(targetMember is PropertyReference);
                    var pr = (PropertyReference)targetMember;
                    var getMethod = pr.Resolve().GetMethod;
                    MethodReference getter;
                    if (targetType.GenericParameters.Count >= 1)
                    {
                        // Instantiates method on generic type (maybe anonymous type)
                        var targetInstanceType = new GenericInstanceType(targetType);
                        foreach (var gp in targetType.GenericParameters)
                        {
                            targetInstanceType.GenericArguments.Add(
                                module.ImportReference(gp));
                        }
                        getter = new MethodReference(
                            getMethod.Name,
                            module.ImportReference(getMethod.ReturnType),
                            targetInstanceType);
                        getter.HasThis = getMethod.HasThis;
                        getter.ExplicitThis = getMethod.ExplicitThis;
                        getter.CallingConvention = getMethod.CallingConvention;
                    }
                    else
                    {
                        getter = module.ImportReference(getMethod);
                    }
                    extractMethodInsts.Add(
                        Instruction.Create(
                            getMethod.IsVirtual ? OpCodes.Callvirt : OpCodes.Call,
                            getter));
                    setParameterMethod = this.setByValParameterMethod;
                }

                var setParameterGenericInstanceMethod =
                    new GenericInstanceMethod(
                        module.ImportReference(setParameterMethod));
                setParameterGenericInstanceMethod.GenericArguments.Add(memberType);
                extractMethodInsts.Add(
                    Instruction.Create(
                        OpCodes.Call,
                        setParameterGenericInstanceMethod));
            }

            extractMethodInsts.Add(
                Instruction.Create(OpCodes.Ret));

            //////////////////////////////////////////////

            // Implements interface.
            if (requiredOverrideMethod == null)
            {
                var ii = new InterfaceImplementation(
                    module.ImportReference(this.parameterExtractableType));
                targetType.Interfaces.Add(ii);

                extractMethod.Overrides.Add(
                    module.ImportReference(this.extractMethod));
            }

            // Mark injected.
            var injectedAttribute = new CustomAttribute(
                module.ImportReference(this.parameterExtractableInjectedAttributeConstructor));
            injectedAttribute.ConstructorArguments.Add(
                new CustomAttributeArgument(
                    this.typeSystem.String, ThisAssembly.AssemblyVersion));
            targetType.CustomAttributes.Add(injectedAttribute);

            return InjectResults.Success;
        }
    }
}
