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

        private readonly TypeDefinition compilerGeneratedAttributeType;
        private readonly MethodDefinition compilerGeneratedAttributeConstructor;

        private readonly TypeDefinition queryFacadeExtensionType;
        private readonly TypeDefinition synchronizedQueryFacadeExtensionType;
        private readonly TypeDefinition staticQueryFacadeType;
        private readonly TypeDefinition extractedParameterType;
        private readonly TypeDefinition recordInjectorDelegateType;
        private readonly MethodDefinition recordInjectableInjectedAttributeConstructor;
        private readonly MethodDefinition parameterExtractableInjectedAttributeConstructor;
        private readonly TypeDefinition staticMemberMetadataType;
        private readonly MethodDefinition staticMemberMetadataConstructor;
        private readonly TypeDefinition staticRecordInjectorByRefDelegateType;
        private readonly MethodDefinition staticRecordInjectorByRefDelegateConstructor;
        private readonly TypeDefinition staticRecordInjectorObjRefDelegateType;
        private readonly MethodDefinition staticRecordInjectorObjRefDelegateConstructor;
        private readonly TypeDefinition staticRecordInjectionContextType;
        private readonly MethodDefinition registerMetadataMethod;
        private readonly TypeDefinition staticParameterExtractionContextType;
        private readonly MethodDefinition setParameterByRefMethod;
        private readonly MethodDefinition setParameterMethod;
        private readonly TypeDefinition parameterExtractableType;
        private readonly TypeDefinition recordInjectableType;
        private readonly MethodDefinition extractMethod;
        private readonly MethodDefinition prepareMethod;
        private readonly Dictionary<TypeKey, MethodDefinition> getValueMethods;
        private readonly MethodDefinition getValueMethod;
        private readonly MethodDefinition getObjRefValueMethod;
        private readonly MethodDefinition getNullableValueMethod;
        private readonly MethodDefinition getNullableObjRefValueMethod;
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
                FirstOrDefault(File.Exists);
            if (flyFlintCorePath == null)
            {
                throw new InvalidOperationException(
                    $"Couldn't find FlyFlint.Core.dll: Path=[{string.Join(",", referencesBasePath)}]");
            }

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
                FirstOrDefault(File.Exists);
            if (flyFlintPath == null)
            {
                throw new InvalidOperationException(
                    $"Couldn't find FlyFlint.dll: Path=[{string.Join(",", referencesBasePath)}]");
            }

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

            this.compilerGeneratedAttributeType = typeSystem.Object.Resolve().Module.Types.First(
                t => t.FullName == "System.Runtime.CompilerServices.CompilerGeneratedAttribute");
            this.compilerGeneratedAttributeConstructor = this.compilerGeneratedAttributeType.Methods.
                First(m => m.IsConstructor);

            this.queryFacadeExtensionType = flyFlintAssembly.MainModule.GetType(
                "FlyFlint.QueryFacadeExtension")!;
            this.synchronizedQueryFacadeExtensionType = flyFlintAssembly.MainModule.GetType(
                "FlyFlint.Synchronized.QueryFacadeExtension")!;
            this.staticQueryFacadeType = flyFlintCoreAssembly.MainModule.GetType(
                "FlyFlint.Internal.Static.StaticQueryFacade")!;

            this.queryFacadeMapping = this.queryFacadeExtensionType.Methods.
                Concat(this.synchronizedQueryFacadeExtensionType.Methods).
                Where(m => m.IsPublic).
                Join(this.staticQueryFacadeType.Methods.Where(m => m.IsPublic),
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

            this.recordInjectorDelegateType = flyFlintCoreAssembly.MainModule.Types.
                First(t => t.FullName.StartsWith("FlyFlint.Internal.RecordInjectorDelegate"));

            var recordInjectableInjectedAttributeType = flyFlintCoreAssembly.MainModule.Types.
                First(t => t.FullName.StartsWith("FlyFlint.Internal.Static.RecordInjectableInjectedAttribute"));
            this.recordInjectableInjectedAttributeConstructor = recordInjectableInjectedAttributeType.Methods.
                First(m => m.IsConstructor && !m.IsStatic);

            var parameterExtractableInjectedAttributeType = flyFlintCoreAssembly.MainModule.Types.
                First(t => t.FullName.StartsWith("FlyFlint.Internal.Static.ParameterExtractableInjectedAttribute"));
            this.parameterExtractableInjectedAttributeConstructor = parameterExtractableInjectedAttributeType.Methods.
                First(m => m.IsConstructor && !m.IsStatic);

            this.staticMemberMetadataType = flyFlintCoreAssembly.MainModule.GetType(
                "FlyFlint.Internal.Static.StaticMemberMetadata")!;
            this.staticMemberMetadataConstructor = staticMemberMetadataType.Methods.
                First(m => m.IsConstructor);

            this.staticRecordInjectorByRefDelegateType = flyFlintCoreAssembly.MainModule.Types.
                First(t => t.FullName.StartsWith("FlyFlint.Internal.Static.StaticRecordInjectorByRefDelegate"));
            this.staticRecordInjectorByRefDelegateConstructor = this.staticRecordInjectorByRefDelegateType.Methods.
                First(m => m.IsConstructor && !m.IsStatic);

            this.staticRecordInjectorObjRefDelegateType = flyFlintCoreAssembly.MainModule.Types.
                First(t => t.FullName.StartsWith("FlyFlint.Internal.Static.StaticRecordInjectorObjRefDelegate"));
            this.staticRecordInjectorObjRefDelegateConstructor = this.staticRecordInjectorObjRefDelegateType.Methods.
                First(m => m.IsConstructor && !m.IsStatic);

            this.staticRecordInjectionContextType = flyFlintCoreAssembly.MainModule.GetType(
                "FlyFlint.Internal.Static.StaticRecordInjectionContext")!;
            this.registerMetadataMethod = staticRecordInjectionContextType.Methods.
                First(m => m.IsPublic && m.IsVirtual && m.Name.StartsWith("RegisterMetadata"));

            this.staticParameterExtractionContextType = flyFlintCoreAssembly.MainModule.GetType(
                "FlyFlint.Internal.Static.StaticParameterExtractionContext")!;
            this.setParameterByRefMethod = staticParameterExtractionContextType.Methods.
                First(m => m.IsPublic && m.Name.StartsWith("SetParameter") && m.Parameters[1].ParameterType.IsByReference);
            this.setParameterMethod = staticParameterExtractionContextType.Methods.
                First(m => m.IsPublic && m.Name.StartsWith("SetParameter") && !m.Parameters[1].ParameterType.IsByReference);

            this.parameterExtractableType = flyFlintCoreAssembly.MainModule.GetType(
                "FlyFlint.Internal.Static.IParameterExtractable")!;
            this.recordInjectableType = flyFlintCoreAssembly.MainModule.GetType(
                "FlyFlint.Internal.Static.IRecordInjectable")!;
            this.extractMethod = this.parameterExtractableType.Methods.
                First(m => m.Name.StartsWith("Extract"));
            this.prepareMethod = this.recordInjectableType.Methods.
                First(m => m.Name.StartsWith("Prepare"));

            this.getValueMethods = this.staticRecordInjectionContextType.
                Methods.
                Where(m => m.IsPublic && !m.IsStatic && !m.HasGenericParameters && m.Name.StartsWith("Get")).
                ToDictionary(m => new TypeKey(m.ReturnType, m.Name.StartsWith("GetNullable")));
            this.getValueMethod = this.staticRecordInjectionContextType.
                Methods.
                First(m => m.IsPublic && !m.IsStatic && m.Name.StartsWith("GetValue"));
            this.getObjRefValueMethod = this.staticRecordInjectionContextType.
                Methods.
                First(m => m.IsPublic && !m.IsStatic && m.Name.StartsWith("GetObjRefValue"));
            this.getNullableValueMethod = this.staticRecordInjectionContextType.
                Methods.
                First(m => m.IsPublic && !m.IsStatic && m.Name.StartsWith("GetNullableValue"));
            this.getNullableObjRefValueMethod = this.staticRecordInjectionContextType.
                Methods.
                First(m => m.IsPublic && !m.IsStatic && m.Name.StartsWith("GetNullableObjRefValue"));
        }

        ////////////////////////////////////////////////////////////////////////////

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

            for (var metadataIndex = 0; metadataIndex < targetMembers.Length; metadataIndex++)
            {
                var targetMember = targetMembers[metadataIndex];

                var memberType = Utilities.GetMemberType(module, targetMember);
                var getValueMethod = GetValueMethod(targetMember, memberType);

                injectMethodInsts.Add(
                    Instruction.Create(OpCodes.Ldarg_1));
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

            injectMethodInsts.Add(
                Instruction.Create(OpCodes.Ret));

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

        private bool InjectPrepareMethod(
            ModuleDefinition module,
            TypeDefinition targetType)
        {
            // Already injected?
            if (targetType.CustomAttributes.Any(ca =>
                ca.AttributeType.FullName == "FlyFlint.Internal.Static.RecordInjectableInjectedAttribute"))
            {
                return false;
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
                return false;
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
                return false;
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
            targetType.CustomAttributes.Add(
                new CustomAttribute(
                    module.ImportReference(this.recordInjectableInjectedAttributeConstructor)));

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////

        private bool InjectExtractMethod(
            ModuleDefinition module,
            TypeDefinition targetType)
        {
            // Already injected?
            if (targetType.CustomAttributes.Any(ca =>
                ca.AttributeType.FullName == "FlyFlint.Internal.Static.ParameterExtractableInjectedAttribute"))
            {
                return false;
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
                return false;
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
                return false;
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
                        setParameterMethod = this.setParameterByRefMethod;
                    }
                    else
                    {
                        extractMethodInsts.Add(
                            Instruction.Create(
                                OpCodes.Ldfld,
                                module.ImportReference(fr)));
                        setParameterMethod = this.setParameterMethod;
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
                    setParameterMethod = this.setParameterMethod;
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
            targetType.CustomAttributes.Add(
                new CustomAttribute(
                    module.ImportReference(this.parameterExtractableInjectedAttributeConstructor)));

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////

        private (TypeDefinition[] parametersTypes, TypeDefinition[] recordTypes) GetTargetTypes(
            AssemblyDefinition targetAssembly)
        {
            /////////////////////////////////////////////////////////
            // Step 1. Extract types by the target attributes.

            var targetTypes = Utilities.ParallelSelect(
                targetAssembly.Modules.
                    SelectMany(Utilities.GetAllTypes),
                type =>
                    (queryParameterType: type.CustomAttributes.Any(ca =>
                        ca.AttributeType.FullName == "FlyFlint.QueryParameterAttribute") ? type : null,
                     queryRecordType: type.CustomAttributes.Any(ca =>
                        ca.AttributeType.FullName == "FlyFlint.QueryRecordAttribute") ? type : null));

            var queryParameterTypes = targetTypes.
                Select(entry => entry.queryParameterType).
                OfType<TypeDefinition>().
                ToArray();

            var queryRecordTypes = targetTypes.
                Select(entry => entry.queryRecordType).
                OfType<TypeDefinition>().
                ToArray();

            /////////////////////////////////////////////////////////
            // Step 2. Extract types by type usages from method body opcodes.

            var aggregatedTypes = Utilities.ParallelSelect(
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
                    var parameterTypes = facadeMethodCallers.
                        Where(inst =>
                            inst.mr!.Name.StartsWith("Parameter") &&
                            inst.mr!.GenericArguments.Count == 1).
                        Select(inst =>
                            inst.mr!.GenericArguments[0]).
                        Cast<TypeReference>().
                        ToArray();
                    var recordOnParameterTypes = facadeMethodCallers.
                        Where(inst =>
                            inst.mr!.Name.StartsWith("Parameter") &&
                            inst.mr!.GenericArguments.Count == 2).
                        Select(inst => inst.mr!.GenericArguments[0]).
                        Cast<TypeReference>();
                    var parametersOnParameterTypes = facadeMethodCallers.
                        Where(inst =>
                            inst.mr!.Name.StartsWith("Parameter") &&
                            inst.mr!.GenericArguments.Count == 2).
                        Select(inst => inst.mr!.GenericArguments[1]).
                        Cast<TypeReference>();
                    var executeTypes = facadeMethodCallers.
                        Where(inst =>
                            inst.mr!.Name.StartsWith("Execute")).
                        Select(inst => inst.mr!.GenericArguments[0]).
                        Cast<TypeReference>();
                    var parametersTypes =
                        parameterTypes.
                        Concat(parametersOnParameterTypes).
                        ToArray();
                    var recordTypes =
                        recordOnParameterTypes.
                        Concat(executeTypes).
                        ToArray();

                    return (parametersTypes, recordTypes);
                });

            var parametersTypes = aggregatedTypes.
                SelectMany(entry => entry.parametersTypes).
                Concat(queryParameterTypes).
                Distinct().
                Select(t => t.Resolve()).
                ToArray();
            var recordTypes = aggregatedTypes.
                SelectMany(entry => entry.recordTypes).
                Concat(queryRecordTypes).
                Distinct().
                Select(t => t.Resolve()).
                ToArray();

            return (parametersTypes, recordTypes);
        }

        ////////////////////////////////////////////////////////////////////////////

        public bool Inject(string targetAssemblyPath)
        {
            var targetBasePath = Path.GetDirectoryName(targetAssemblyPath)!;

            // Add reference assembly search path at same directory.
            this.assemblyResolver.AddSearchDirectory(targetBasePath);

            var targetAssemblyName = Path.GetFileNameWithoutExtension(
                targetAssemblyPath);
            var targetDebuggingPath = Path.Combine(
                targetBasePath,
                Path.GetFileNameWithoutExtension(targetAssemblyPath) + ".pdb");

            // HACK: cecil will lock symbol file when uses defaulted reading method,
            //   (and couldn't replace it manually).
            MemoryStream? symbolStream = null;
            if (File.Exists(targetDebuggingPath))
            {
                using var pdbStream = new FileStream(
                    targetDebuggingPath, FileMode.Open, FileAccess.Read, FileShare.None);
                symbolStream = new MemoryStream();
                pdbStream.CopyTo(symbolStream);
                symbolStream.Position = 0;
            }

            // Reading target assembly (and symbol file) by cecil.
            using var targetAssembly = AssemblyDefinition.ReadAssembly(
                targetAssemblyPath,
                new ReaderParameters(ReadingMode.Immediate)
                {
                    ReadWrite = false,
                    InMemory = true,
                    AssemblyResolver = this.assemblyResolver,
                    SymbolStream = symbolStream,
                    ReadSymbols = symbolStream != null,
                });

            // Gathering target types from IL streams.
            var (parametersTypes, recordTypes) = this.GetTargetTypes(targetAssembly);

            // If found target types:
            if ((parametersTypes.Length >= 1) || (recordTypes.Length >= 1))
            {
                var parametersInjected = 0;
                var recordInjected = 0;

                foreach (var parametersType in parametersTypes.
                    OrderBy(t => t, TypeInheritanceDepthComparer.Instance))
                {
                    // By IParameterExtractable interface.
                    if (this.InjectExtractMethod(targetAssembly.MainModule, parametersType))
                    {
                        parametersInjected++;
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

                foreach (var recordType in recordTypes.
                    OrderBy(t => t, TypeInheritanceDepthComparer.Instance))
                {
                    // By IRecordInjectable interface.
                    if (this.InjectPrepareMethod(targetAssembly.MainModule, recordType))
                    {
                        recordInjected++;
                        this.message(
                            LogLevels.Trace,
                            $"Injected an data type: Assembly={targetAssemblyName}, Type={recordType.FullName}");
                    }
                    else
                    {
                        this.message(
                            LogLevels.Trace,
                            $"Ignored an data type: Assembly={targetAssemblyName}, Type={recordType.FullName}");
                    }
                }

                // One or more types injected:
                if (parametersInjected >= 1 || recordInjected >= 1)
                {
                    // Backup original assembly and symbol files,
                    // because cecil will fail when contains invalid metadata.
                    var backupAssemblyPath = Path.Combine(
                        targetBasePath,
                        Path.GetFileNameWithoutExtension(targetAssemblyPath) + "_backup" +
                            Path.GetExtension(targetAssemblyPath));
                    var backupDebuggingPath = Path.Combine(
                        targetBasePath,
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
                            // Write injected assembly and symbol file.
                            targetAssembly.Write(
                                targetAssemblyPath,
                                new WriterParameters
                                {
                                    WriteSymbols = true,
                                    DeterministicMvid = true,
                                });
                        }
                        // Failed:
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
                    // Failed:
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

                    // Remove originals.
                    if (File.Exists(backupAssemblyPath))
                    {
                        File.Delete(backupAssemblyPath);
                    }
                    if (File.Exists(backupDebuggingPath))
                    {
                        File.Delete(backupDebuggingPath);
                    }

                    this.message(
                        LogLevels.Information,
                        $"Injected: Assembly={targetAssemblyName}, Parameters={parametersInjected}, Record={recordInjected}");

                    return true;
                }
                else
                {
                    this.message(
                        LogLevels.Information,
                        $"Found nothing target types: Assembly={targetAssemblyName}");
                }
            }

            return false;
        }
    }
}
