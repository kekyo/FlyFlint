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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FlyFlint
{
    public enum LogLevels
    {
        Trace,
        Information,
        Warning,
        Error
    }

    ////////////////////////////////////////////////////////////////////////////

    public sealed class Injector
    {
        private const string dirtySymbolPrefix = "<>`";

        private sealed class TypeKey : IEquatable<TypeKey?>
        {
            public readonly TypeReference Type;
            public readonly bool IsNullable;

            public TypeKey(TypeReference type, bool isNullable)
            {
                this.Type = type;
                this.IsNullable = isNullable;
            }

            public override int GetHashCode() =>
                Utilities.GetTypeName(this.Type).GetHashCode() ^ this.IsNullable.GetHashCode();

            public bool Equals(TypeKey? other) =>
                other is TypeKey o &&
                Utilities.GetTypeName(this.Type).Equals(Utilities.GetTypeName(o.Type)) &&
                this.IsNullable == o.IsNullable;

            public override string ToString() =>
                $"{Utilities.GetTypeName(this.Type)}{(!this.Type.IsValueType ? this.IsNullable ? "?" : "" : "")}";
        }

        ////////////////////////////////////////////////////////////////////////////

        private readonly Action<LogLevels, string> message;
        private readonly DefaultAssemblyResolver assemblyResolver = new();

        private readonly TypeSystem typeSystem;
        private readonly TypeDefinition typeType;
        private readonly MethodDefinition getTypeFromHandleMethod;
        private readonly TypeDefinition delegateType;

        private readonly TypeDefinition debuggerHiddenAttributeType;
        private readonly TypeDefinition compilerGeneratedAttributeType;
        private readonly MethodDefinition compilerGeneratedAttributeConstructor;
        private readonly TypeDefinition debuggerBrowsableAttributeType;
        private readonly TypeDefinition debuggerBrowsableStateType;

        private readonly TypeDefinition queryFacadeExtensionType;
        private readonly TypeDefinition synchronizedQueryFacadeExtensionType;
        private readonly TypeDefinition staticQueryFacadeType;
        private readonly TypeDefinition extractedParameterType;
        private readonly TypeDefinition dataInjectorDelegateType;
        private readonly MethodDefinition dataInjectableInjectedAttributeConstructor;
        private readonly MethodDefinition parameterExtractableInjectedAttributeConstructor;
        private readonly TypeDefinition staticMemberMetadataType;
        private readonly MethodDefinition staticMemberMetadataConstructor;
        private readonly TypeDefinition staticDataInjectorDelegateType;
        private readonly MethodDefinition staticDataInjectorDelegateConstructor;
        private readonly TypeDefinition staticDataInjectionContextType;
        private readonly MethodDefinition registerMetadataMethod;
        private readonly TypeDefinition parameterExtractableType;
        private readonly TypeDefinition dataInjectableType;
        private readonly MethodDefinition extractMethod;
        private readonly MethodDefinition prepareMethod;
        private readonly Dictionary<TypeKey, MethodDefinition> getValueMethods;
        private readonly MethodDefinition getEnumValueMethod;
        private readonly MethodDefinition getNullableEnumValueMethod;
        private readonly Dictionary<MethodReference, MethodDefinition> queryFacadeMapping;

        ////////////////////////////////////////////////////////////////////////////

        public Injector(string[] referencesBasePath, Action<LogLevels, string> message)
        {
            this.message = message;

            foreach (var referenceBasePath in referencesBasePath)
            {
                this.assemblyResolver.AddSearchDirectory(referenceBasePath);
            }

            var flyFlintCorePath = referencesBasePath.
                Select(basePath => Path.Combine(basePath, "FlyFlint.Core.dll")).
                First(File.Exists);

            var flyFlintCoreAssembly = AssemblyDefinition.ReadAssembly(
                flyFlintCorePath,
                new ReaderParameters
                {
                    AssemblyResolver = assemblyResolver,
                }
            );

            this.message(
                LogLevels.Trace,
                $"FlyFlint.Core.dll is loaded: Path={flyFlintCorePath}");

            var flyFlintPath = referencesBasePath.
                Select(basePath => Path.Combine(basePath, "FlyFlint.dll")).
                First(File.Exists);

            var flyFlintAssembly = AssemblyDefinition.ReadAssembly(
                flyFlintPath,
                new ReaderParameters
                {
                    AssemblyResolver = assemblyResolver,
                }
            );

            this.message(
                LogLevels.Trace,
                $"FlyFlint.dll is loaded: Path={flyFlintPath}");

            this.typeSystem = flyFlintCoreAssembly.MainModule.TypeSystem;

            this.typeType = typeSystem.Object.Resolve().Module.Types.First(
                t => t.FullName == "System.Type");
            this.getTypeFromHandleMethod = this.typeType.Methods.First(
                m => m.Name == "GetTypeFromHandle");
            this.delegateType = typeSystem.Object.Resolve().Module.Types.First(
                t => t.FullName == "System.Delegate");

            this.debuggerHiddenAttributeType = typeSystem.Object.Resolve().Module.Types.First(
                t => t.FullName == "System.Diagnostics.DebuggerHiddenAttribute");
            this.compilerGeneratedAttributeType = typeSystem.Object.Resolve().Module.Types.First(
                t => t.FullName == "System.Runtime.CompilerServices.CompilerGeneratedAttribute");
            this.compilerGeneratedAttributeConstructor = this.compilerGeneratedAttributeType.Methods.
                First(m => m.IsConstructor);
            this.debuggerBrowsableAttributeType = typeSystem.Object.Resolve().Module.Types.First(
                t => t.FullName == "System.Diagnostics.DebuggerBrowsableAttribute");
            this.debuggerBrowsableStateType = typeSystem.Object.Resolve().Module.Types.First(
                t => t.FullName == "System.Diagnostics.DebuggerBrowsableState");

            this.queryFacadeExtensionType = flyFlintAssembly.MainModule.GetType(
                "FlyFlint.QueryFacadeExtension")!;
            this.synchronizedQueryFacadeExtensionType = flyFlintAssembly.MainModule.GetType(
                "FlyFlint.Synchronized.QueryFacadeExtension")!;
            this.staticQueryFacadeType = flyFlintCoreAssembly.MainModule.GetType(
                "FlyFlint.Internal.Static.StaticQueryFacade")!;

            this.queryFacadeMapping = this.queryFacadeExtensionType.Methods.
                Concat(this.synchronizedQueryFacadeExtensionType.Methods).
                Join(this.staticQueryFacadeType.Methods,
                    m => m,
                    m => m,
                    (qfm, sqfm) => (qfm, sqfm),
                    SignatureDroppedGenericTypeEqualityComparer.Instance).
                ToDictionary(entry =>
                    (MethodReference)entry.qfm,
                    entry => entry.sqfm,
                    SignatureDroppedGenericTypeEqualityComparer.Instance);

            this.extractedParameterType = flyFlintCoreAssembly.MainModule.GetType(
                "FlyFlint.Internal.ExtractedParameter")!;

            this.dataInjectorDelegateType = flyFlintCoreAssembly.MainModule.Types.
                First(t => t.FullName.StartsWith("FlyFlint.Internal.DataInjectorDelegate"));

            var dataInjectableInjectedAttributeType = flyFlintCoreAssembly.MainModule.Types.
                First(t => t.FullName.StartsWith("FlyFlint.Internal.Static.DataInjectableInjectedAttribute"));
            this.dataInjectableInjectedAttributeConstructor = dataInjectableInjectedAttributeType.Methods.
                First(m => m.IsConstructor && !m.IsStatic);

            var parameterExtractableInjectedAttributeType = flyFlintCoreAssembly.MainModule.Types.
                First(t => t.FullName.StartsWith("FlyFlint.Internal.Static.ParameterExtractableInjectedAttribute"));
            this.parameterExtractableInjectedAttributeConstructor = parameterExtractableInjectedAttributeType.Methods.
                First(m => m.IsConstructor && !m.IsStatic);

            this.staticMemberMetadataType = flyFlintCoreAssembly.MainModule.GetType(
                "FlyFlint.Internal.Static.StaticMemberMetadata")!;
            this.staticMemberMetadataConstructor = staticMemberMetadataType.Methods.
                First(m => m.IsConstructor);

            this.staticDataInjectorDelegateType = flyFlintCoreAssembly.MainModule.Types.
                First(t => t.FullName.StartsWith("FlyFlint.Internal.Static.StaticDataInjectorDelegate"));
            this.staticDataInjectorDelegateConstructor = this.staticDataInjectorDelegateType.Methods.
                First(m => m.IsConstructor && !m.IsStatic);

            this.staticDataInjectionContextType = flyFlintCoreAssembly.MainModule.GetType(
                "FlyFlint.Internal.Static.StaticDataInjectionContext")!;
            this.registerMetadataMethod = staticDataInjectionContextType.Methods.
                First(m => m.IsPublic && m.IsVirtual && m.Name.StartsWith("RegisterMetadata"));

            this.parameterExtractableType = flyFlintCoreAssembly.MainModule.GetType(
                "FlyFlint.Internal.Static.IParameterExtractable")!;
            this.dataInjectableType = flyFlintCoreAssembly.MainModule.GetType(
                "FlyFlint.Internal.Static.IDataInjectable")!;
            this.extractMethod = this.parameterExtractableType.Methods.
                First(m => m.Name.StartsWith("Extract"));
            this.prepareMethod = this.dataInjectableType.Methods.
                First(m => m.Name.StartsWith("Prepare"));

            this.getValueMethods = this.staticDataInjectionContextType.
                Methods.
                Where(m => m.IsPublic && !m.IsStatic && !m.HasGenericParameters && m.Name.StartsWith("Get")).
                ToDictionary(m => new TypeKey(m.ReturnType, m.Name.StartsWith("GetNullable")));
            this.getEnumValueMethod = this.staticDataInjectionContextType.
                Methods.
                First(m => m.IsPublic && !m.IsStatic && m.Name.StartsWith("GetEnum"));
            this.getNullableEnumValueMethod = this.staticDataInjectionContextType.
                Methods.
                First(m => m.IsPublic && !m.IsStatic && m.Name.StartsWith("GetNullableEnum"));
        }

        ////////////////////////////////////////////////////////////////////////////

        private FieldReference InjectStaticMemberField(
            ModuleDefinition module,
            TypeDefinition targetType, MethodDefinition cctor, MemberReference[] targetMembers)
        {
            var staticMemberMetadatasType = new ArrayType(
                module.ImportReference(this.staticMemberMetadataType));

            var membersField = new FieldDefinition(
                dirtySymbolPrefix + "flyflint_members__",
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
                    Instruction.Create(OpCodes.Ldstr, targetMember.Name));  // TODO: DataMemberAttribute
                cctorInsts.Insert(instIndex++,
                    Instruction.Create(
                        OpCodes.Ldtoken,
                        Utilities.GetMemberType(module, targetMember)));
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
            var staticDataInjectorDelegateType =
                new GenericInstanceType(
                    module.ImportReference(this.staticDataInjectorDelegateType));
            staticDataInjectorDelegateType.GenericArguments.
                Add(targetType);

            // Couldn't get constructor reference from instantiated type directly.
            var staticDataInjectorDelegateConstructor =
                new MethodReference(
                    ".ctor",
                    this.typeSystem.Void,
                    staticDataInjectorDelegateType);
            staticDataInjectorDelegateConstructor.HasThis = true;    // Important
            staticDataInjectorDelegateConstructor.Parameters.Add(
                new ParameterDefinition(this.typeSystem.Object));
            staticDataInjectorDelegateConstructor.Parameters.Add(
                new ParameterDefinition(this.typeSystem.IntPtr));

            var injectorField = new FieldDefinition(
                dirtySymbolPrefix + "flyflint_injector__",
                FieldAttributes.Private | FieldAttributes.Static | FieldAttributes.InitOnly,
                module.ImportReference(this.delegateType));
            injectorField.CustomAttributes.Add(
                new CustomAttribute(
                    module.ImportReference(this.compilerGeneratedAttributeConstructor)));
            targetType.Fields.Add(injectorField);

            //////////////////////////////////////////////

            var injectMethod = new MethodDefinition(
                dirtySymbolPrefix + "flyflint_inject__",
                MethodAttributes.HideBySig | MethodAttributes.Private | MethodAttributes.Static,
                this.typeSystem.Void);
            injectMethod.Parameters.Add(
                new ParameterDefinition(
                    "context",
                    ParameterAttributes.None,
                    module.ImportReference(this.staticDataInjectionContextType)));
            injectMethod.Parameters.Add(
                new ParameterDefinition(
                    "element",
                    ParameterAttributes.None,
                    new ByReferenceType(targetType)));
            injectMethod.ImplAttributes = MethodImplAttributes.AggressiveInlining;
            injectMethod.CustomAttributes.Add(
                new CustomAttribute(
                    module.ImportReference(this.compilerGeneratedAttributeConstructor)));
            targetType.Methods.Add(injectMethod);

            var injectMethodInsts = injectMethod.Body.Instructions;

            static bool IsNullableForMember(
                MemberReference targetMember, TypeReference memberType)
            {
                if (memberType.IsValueType)
                {
                    return memberType.FullName.StartsWith("System.Nullable");
                }
                else
                {
                    return
                        (targetMember is FieldDefinition f ? f.CustomAttributes :
                         ((PropertyDefinition)targetMember).CustomAttributes).
                        Any(ca => ca.AttributeType.FullName.StartsWith("System.Runtime.CompilerServices.NullableAttribute"));
                }
            }

            MethodReference GetValueMethod(
                MemberReference targetMember, TypeReference memberType)
            {
                var key = new TypeKey(memberType, IsNullableForMember(targetMember, memberType));

                if (this.getValueMethods.TryGetValue(key, out var gvm))
                {
                    return module.ImportReference(gvm);
                }
                else if (memberType.Resolve().IsEnum)
                {
                    var m = new GenericInstanceMethod(
                        module.ImportReference(this.getEnumValueMethod));
                    m.GenericArguments.Add(memberType);
                    return m;
                }
                else
                {
                    Debug.Assert(memberType.FullName.StartsWith("System.Nullable"));
                    var m = new GenericInstanceMethod(
                        module.ImportReference(this.getNullableEnumValueMethod));
                    m.GenericArguments.Add(memberType);   // TODO: dereferenced generic argument type
                    return m;
                }
            }

            for (var metadataIndex = 0; metadataIndex < targetMembers.Length; metadataIndex++)
            {
                var targetMember = targetMembers[metadataIndex];

                var memberType = Utilities.GetMemberType(module, targetMember);
                var getValueMethod = GetValueMethod(targetMember, memberType);

                injectMethodInsts.Add(
                    Instruction.Create(OpCodes.Ldarg_1));
                if (!targetType.IsValueType)
                {
                    injectMethodInsts.Add(
                        Instruction.Create(OpCodes.Ldind_Ref));
                }
                injectMethodInsts.Add(
                    Instruction.Create(OpCodes.Ldarg_0));
                injectMethodInsts.Add(
                    Instruction.Create(OpCodes.Ldc_I4_S, (sbyte)metadataIndex));
                injectMethodInsts.Add(
                    Instruction.Create(OpCodes.Call, getValueMethod));

                if (targetMember is FieldDefinition f)
                {
                    injectMethodInsts.Add(
                        Instruction.Create(OpCodes.Stfld, f));
                }
                else
                {
                    var p = (PropertyDefinition)targetMember;
                    var sm = p.SetMethod;
                    Debug.Assert(sm != null);

                    injectMethodInsts.Add(
                        Instruction.Create(
                            sm!.IsVirtual ? OpCodes.Callvirt : OpCodes.Call,
                            sm));
                }
            }

            injectMethodInsts.Add(
                Instruction.Create(OpCodes.Ret));

            var cctorInsts = cctor.Body.Instructions;

            var instIndex = 0;
            cctorInsts.Insert(instIndex++,
                Instruction.Create(OpCodes.Ldnull));
            cctorInsts.Insert(instIndex++,
                Instruction.Create(OpCodes.Ldftn, injectMethod));
            cctorInsts.Insert(instIndex++,
                Instruction.Create(OpCodes.Newobj, staticDataInjectorDelegateConstructor));
            cctorInsts.Insert(instIndex++,
                Instruction.Create(OpCodes.Stsfld, injectorField));

            return module.ImportReference(injectorField);
        }

        ////////////////////////////////////////////////////////////////////////////

        private bool InjectPrepareMethod(
            ModuleDefinition module,
            TypeDefinition targetType)
        {
            if (targetType.CustomAttributes.Any(ca =>
                ca.AttributeType.Name == "FlyFlint.Internal.Static.DataInjectableInjectedAttribute"))
            {
                return false;
            }

            var targetFields = targetType.Fields.
                Where(f => f.IsPublic && !f.IsStatic && !f.IsInitOnly).
                Cast<MemberReference>();
            var targetProperties = targetType.Properties.
                Where(p =>
                    p.SetMethod is MethodReference mr &&
                    mr.Resolve() is { } m &&
                    m.IsPublic && !m.IsStatic).   // TODO: DataMemberAttribute
                Cast<MemberReference>();

            var targetMembers = targetFields.Concat(targetProperties).ToArray();
            if (targetMembers.Length == 0)
            {
                return false;
            }

            //////////////////////////////////////////////

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

            var injectorField = InjectInjectorMethod(module, targetType, cctor, targetMembers);
            var membersField = InjectStaticMemberField(module, targetType, cctor, targetMembers);

            //////////////////////////////////////////////

            var requiredOverrideMethod = targetType.
                Traverse(t => t.BaseType?.Resolve()).
                SelectMany(t => t.Methods.Where(m =>
                    m.IsFamily && m.IsVirtual && m.IsHideBySig && (m.Name == "Prepare"))).
                FirstOrDefault();

            var prepareMethod = new MethodDefinition(
                // The CLR and CoreCLR, will cause TypeLoadException when uses different name in inferface member method...
                /* dirtySymbolPrefix + */ "Prepare",
                MethodAttributes.HideBySig | MethodAttributes.Family | MethodAttributes.Virtual |
                    ((requiredOverrideMethod != null) ?
                        MethodAttributes.ReuseSlot :
                        MethodAttributes.NewSlot),
                this.typeSystem.Void);
            prepareMethod.Parameters.Add(
                new ParameterDefinition(
                    "context",
                    ParameterAttributes.None,
                    module.ImportReference(this.staticDataInjectionContextType)));
            prepareMethod.ImplAttributes = MethodImplAttributes.AggressiveInlining;
            prepareMethod.CustomAttributes.Add(
                new CustomAttribute(
                    module.ImportReference(this.compilerGeneratedAttributeConstructor)));
            targetType.Methods.Add(prepareMethod);

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

            if (requiredOverrideMethod == null)
            {
                var ii = new InterfaceImplementation(
                    module.ImportReference(this.dataInjectableType));
                targetType.Interfaces.Add(ii);

                prepareMethod.Overrides.Add(
                    module.ImportReference(this.prepareMethod));
            }

            targetType.CustomAttributes.Add(
                new CustomAttribute(
                    module.ImportReference(this.dataInjectableInjectedAttributeConstructor)));

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////

        private bool InjectExtractMethod(
            ModuleDefinition module,
            TypeDefinition targetType)
        {
            if (targetType.CustomAttributes.Any(ca =>
                ca.AttributeType.Name == "FlyFlint.Internal.Static.ParameterExtractableInjectedAttribute"))
            {
                return false;
            }

            var targetFields = targetType.Fields.
                Where(f => f.IsPublic && !f.IsStatic && !f.IsInitOnly).
                Cast<MemberReference>();
            var targetProperties = targetType.Properties.
                Where(p =>
                    p.SetMethod is MethodReference mr &&
                    mr.Resolve() is { } m &&
                    m.IsPublic && !m.IsStatic).   // TODO: DataMemberAttribute
                Cast<MemberReference>();

            var targetMembers = targetFields.Concat(targetProperties).ToArray();
            if (targetMembers.Length == 0)
            {
                return false;
            }

            //////////////////////////////////////////////

            var requiredOverrideMethod = targetType.
                Traverse(t => t.BaseType?.Resolve()).
                SelectMany(t => t.Methods.Where(m =>
                    m.IsFamily && m.IsVirtual && m.IsHideBySig && (m.Name == "Extract"))).
                FirstOrDefault();

            var extractMethod = new MethodDefinition(
                // The CLR and CoreCLR, will cause TypeLoadException when uses different name in inferface member method...
                /* dirtySymbolPrefix + */ "Extract",
                MethodAttributes.HideBySig | MethodAttributes.Family | MethodAttributes.Virtual |
                    ((requiredOverrideMethod != null) ?
                        MethodAttributes.ReuseSlot :
                        MethodAttributes.NewSlot),
                this.typeSystem.Void);
            extractMethod.CustomAttributes.Add(
                new CustomAttribute(
                    module.ImportReference(this.compilerGeneratedAttributeConstructor)));
            targetType.Methods.Add(extractMethod);

            //////////////////////////////////////////////


            // TODO:


            //////////////////////////////////////////////

            if (requiredOverrideMethod == null)
            {
                var ii = new InterfaceImplementation(
                    module.ImportReference(this.parameterExtractableType));
                targetType.Interfaces.Add(ii);

                extractMethod.Overrides.Add(
                    module.ImportReference(this.extractMethod));
            }

            targetType.CustomAttributes.Add(
                new CustomAttribute(
                    module.ImportReference(this.parameterExtractableInjectedAttributeConstructor)));

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////

        private (TypeDefinition[] parametersTypes, TypeDefinition[] elementTypes) GetTargetTypes(
            AssemblyDefinition targetAssembly)
        {
            var dataContractTypes =
                targetAssembly.Modules.
                    SelectMany(Utilities.GetAllTypes).
                Where(type =>
                    type.CustomAttributes.Any(ca => ca.AttributeType.FullName == "System.Runtime.Serialization.DataContractAttribute") ||
                    type.Fields.Any(f => f.CustomAttributes.Any(ca => ca.AttributeType.FullName == "System.Runtime.Serialization.DataMemberAttribute")) ||
                    type.Properties.Any(p => p.CustomAttributes.Any(ca => ca.AttributeType.FullName == "System.Runtime.Serialization.DataMemberAttribute"))).
                ToArray();

            //static MethodReference GetGenericMethodDefinitionIfApplicable(MethodReference method) =>
            //    method.IsGenericInstance ? method.GetElementMethod() : method;

            var usingQueryTypes = Utilities.ParallelSelect(
                targetAssembly.Modules.
                    SelectMany(Utilities.GetAllTypes).
                    SelectMany(type => new[] { type }.Concat(type.NestedTypes).SelectMany(t => t.Methods)).
                    Where(method => method.HasBody),
                method =>
                {
                    var facadeMethodCallers = method.Body.Instructions.
                        Select((i, index) => (i, index, mr: i.Operand as GenericInstanceMethod)).
                        Where(entry =>
                            (entry.i.OpCode == OpCodes.Call || entry.i.OpCode == OpCodes.Callvirt) &&
                            entry.mr != null &&
                            this.queryFacadeMapping.ContainsKey(entry.mr)).
                        ToArray();
#if false
                    foreach (var (i, index, mr) in facadeMethodCallers)
                    {
                        static MethodReference MakeGenericMethod(
                            MethodReference mr, IEnumerable<TypeReference> types)
                        {
                            var gim = new GenericInstanceMethod(mr.Resolve());
                            foreach (var t in types)
                            {
                                gim.GenericArguments.Add(t.Resolve());
                            }
                            return gim;
                        }

                        static MethodReference ResolveMethod(
                            MethodReference mr)
                        {
                            var declaringType = mr.DeclaringType.Resolve();
                            var returnType = mr.ReturnType.Resolve();
                            var nmr = new MethodReference(mr.Name, returnType, declaringType);
                            for (var index = 0; index < mr.Parameters.Count; index++)
                            {
                                nmr.Parameters.Add(new ParameterDefinition(mr.Parameters[index].ParameterType.Resolve()));
                            }
                            nmr = nmr.Resolve();
                            nmr.ReturnType = returnType;
                            return nmr;
                        }

                        var replaced = ResolveMethod(this.queryFacadeMapping[mr]);

                        var instantiated = mr is GenericInstanceMethod gim ?
                            MakeGenericMethod(replaced, gim.GenericArguments) :   // TODO: apply each generic arguments
                            replaced;

                        method.Body.Instructions[index] =
                            Instruction.Create(i.OpCode, instantiated);
                    }
                    var returnTypes = facadeMethodCallers.
                        Select(entry => entry.mr as GenericInstanceMethod).
                        Where(gim => gim != null).
                        SelectMany(gim =>
                            gim!.GenericArguments.
                            Select((ga, index) => (gim: gim!, ga, index))).
                        Where(entry => entry.gim.ReturnType == entry.ga).
                        ToArray();
#endif
                    var parametersTypes = facadeMethodCallers.
                        Where(inst => inst.mr!.Name.StartsWith("Parameter")).
                        Select(inst => inst.mr!.GenericArguments.Last()).
                        Cast<TypeReference>().
                        ToArray();

                    var parameterTElementTypes = facadeMethodCallers.
                        Where(inst =>
                            inst.mr!.Name.StartsWith("Parameter") &&
                            inst.mr!.GenericParameters.Count == 1).
                        Select(inst => inst.mr!.GenericArguments.First()).
                        Cast<TypeReference>();
                    var executeTElementTypes = facadeMethodCallers.
                        Where(inst =>
                            inst.mr!.Name.StartsWith("Parameter") ||
                            inst.mr!.Name.StartsWith("Execute")).
                        Select(inst => inst.mr!.GenericArguments.Last()).
                        Cast<TypeReference>();
                    var elementTypes =
                        parameterTElementTypes.
                        Concat(executeTElementTypes).
                        ToArray();

                    return (parametersTypes, elementTypes);
                });

            var parametersTypes = usingQueryTypes.
                SelectMany(entry => entry.parametersTypes).
                Distinct().
                Select(t => t.Resolve()).
                ToArray();
            var elementTypes = usingQueryTypes.
                SelectMany(entry => entry.elementTypes).
                Concat(dataContractTypes).
                Distinct().
                Select(t => t.Resolve()).
                ToArray();

            return (parametersTypes, elementTypes);
        }

        ////////////////////////////////////////////////////////////////////////////

        public bool Inject(string targetAssemblyPath)
        {
            this.assemblyResolver.AddSearchDirectory(
                Path.GetDirectoryName(targetAssemblyPath));

            var targetAssemblyName = Path.GetFileNameWithoutExtension(
                targetAssemblyPath);

            using (var targetAssembly = AssemblyDefinition.ReadAssembly(
                targetAssemblyPath,
                new ReaderParameters(ReadingMode.Immediate)
                {
                    ReadingMode = ReadingMode.Immediate,
                    ReadSymbols = true,
                    ReadWrite = false,
                    InMemory = true,
                    AssemblyResolver = this.assemblyResolver,
                }))
            {
                var (parametersTypes, elementTypes) = GetTargetTypes(targetAssembly);

                if ((parametersTypes.Length >= 1) || (elementTypes.Length >= 1))
                {
                    var injected = false;

                    foreach (var parametersType in parametersTypes.
                        OrderBy(t => t, TypeInheritanceDepthComparer.Instance))
                    {
                        if (this.InjectExtractMethod(targetAssembly.MainModule, parametersType))
                        {
                            injected = true;
                            this.message(
                                LogLevels.Trace,
                                $"Injected an parameter type: Assembly={targetAssemblyName}, Type={parametersType.FullName}");
                        }
                        else
                        {
                            this.message(
                                LogLevels.Trace,
                                $"Ignored an parameter type: Assembly={targetAssemblyName}, Type={parametersType.FullName}");
                        }
                    }

                    foreach (var elementType in elementTypes.
                        OrderBy(t => t, TypeInheritanceDepthComparer.Instance))
                    {
                        if (this.InjectPrepareMethod(targetAssembly.MainModule, elementType))
                        {
                            injected = true;
                            this.message(
                                LogLevels.Trace,
                                $"Injected an element type: Assembly={targetAssemblyName}, Type={elementType.FullName}");
                        }
                        else
                        {
                            this.message(
                                LogLevels.Trace,
                                $"Ignored an element type: Assembly={targetAssemblyName}, Type={elementType.FullName}");
                        }
                    }

                    if (injected)
                    {
                        var outputBasePath = Path.GetDirectoryName(targetAssemblyPath)!;

                        var targetDebuggingPath = Path.Combine(
                            outputBasePath,
                            Path.GetFileNameWithoutExtension(targetAssemblyPath) + ".pdb");
                        var backupAssemblyPath = Path.Combine(
                            outputBasePath,
                            Path.GetFileNameWithoutExtension(targetAssemblyPath) + "_backup" +
                                Path.GetExtension(targetAssemblyPath));
                        var backupDebuggingPath = Path.Combine(
                            outputBasePath,
                            Path.GetFileNameWithoutExtension(targetAssemblyPath) + "_backup.pdb");

                        if (File.Exists(backupAssemblyPath))
                        {
                            File.Delete(backupAssemblyPath);
                        }
                        if (File.Exists(backupDebuggingPath))
                        {
                            File.Delete(backupDebuggingPath);
                        }

                        if (File.Exists(targetAssemblyPath))
                        {
                            File.Move(targetAssemblyPath, backupAssemblyPath);
                        }
                        try
                        {
                            if (File.Exists(targetDebuggingPath))
                            {
                                File.Move(targetDebuggingPath, backupDebuggingPath);
                            }
                            try
                            {
                                targetAssembly.Write(
                                    targetAssemblyPath,
                                    new WriterParameters
                                    {
                                        WriteSymbols = true,
                                        DeterministicMvid = true,
                                    });
                            }
                            catch
                            {
                                if (File.Exists(targetDebuggingPath))
                                {
                                    File.Delete(targetDebuggingPath);
                                }
                                if (File.Exists(backupDebuggingPath))
                                {
                                    File.Move(backupDebuggingPath, targetDebuggingPath);
                                }
                                throw;
                            }
                        }
                        catch
                        {
                            if (File.Exists(targetAssemblyPath))
                            {
                                File.Delete(targetAssemblyPath);
                            }
                            if (File.Exists(backupAssemblyPath))
                            {
                                File.Move(backupAssemblyPath, targetAssemblyPath);
                            }
                            throw;
                        }

                        if (File.Exists(backupAssemblyPath))
                        {
                            File.Delete(backupAssemblyPath);
                        }
                        if (File.Exists(backupDebuggingPath))
                        {
                            File.Delete(backupDebuggingPath);
                        }

                        return true;
                    }
                }
            }

            return false;
        }
    }
}
