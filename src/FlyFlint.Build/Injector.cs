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

    public sealed partial class Injector
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

        private enum InjectResults
        {
            Success,
            Ignored,
            CouldNot,
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
        private readonly FieldDefinition staticRecordInjectionContextCurrentOffsetField;
        private readonly FieldDefinition staticRecordInjectionContextIsAvailableField;
        private readonly MethodDefinition registerMetadataMethod;
        private readonly TypeDefinition staticParameterExtractionContextType;
        private readonly MethodDefinition setByRefParameterMethod;
        private readonly MethodDefinition setByValParameterMethod;
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
            this.staticRecordInjectionContextCurrentOffsetField = this.staticRecordInjectionContextType.Fields.
                First(f => f.Name == "CurrentOffset");
            this.staticRecordInjectionContextIsAvailableField = this.staticRecordInjectionContextType.Fields.
                First(f => f.Name == "IsAvailable");
            this.registerMetadataMethod = this.staticRecordInjectionContextType.Methods.
                First(m => m.IsPublic && m.IsVirtual && m.Name.StartsWith("RegisterMetadata"));

            this.staticParameterExtractionContextType = flyFlintCoreAssembly.MainModule.GetType(
                "FlyFlint.Internal.Static.StaticParameterExtractionContext")!;
            this.setByRefParameterMethod = staticParameterExtractionContextType.Methods.
                First(m => m.IsPublic && m.Name.StartsWith("SetByRefParameter") && m.Parameters[1].ParameterType.IsByReference);
            this.setByValParameterMethod = staticParameterExtractionContextType.Methods.
                First(m => m.IsPublic && m.Name.StartsWith("SetByValParameter") && !m.Parameters[1].ParameterType.IsByReference);

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

        private (TypeDefinition[] parametersTypes, TypeDefinition[] recordTypes) GetTargetTypes(
            AssemblyDefinition targetAssembly)
        {
            var module = targetAssembly.MainModule;

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
                    // Extract target instructions.
                    var facadeMethodCallers = method.Body.Instructions.
                        Select((i, index) => (i, index, mr: i.Operand as MethodReference)).
                        Where(entry =>
                            (entry.i.OpCode == OpCodes.Call || entry.i.OpCode == OpCodes.Callvirt) &&
                            entry.mr != null &&
                            this.queryFacadeMapping.ContainsKey(entry.mr)).
                        ToArray();

                    // Step 2-1-1. Declare replacer for method call ops.
                    void Replacer()
                    {
                        foreach (var (i, index, mr) in facadeMethodCallers!)
                        {
                            if (mr is GenericInstanceMethod gim)
                            {
                                var staticMethod = module!.ImportReference(
                                    this.queryFacadeMapping[mr]);
                                var replaced = new GenericInstanceMethod(staticMethod);
                                var foundAnotherModule = false;
                                foreach (var t in gim.GenericArguments)
                                {
                                    var it = module.ImportReference(t);

                                    // Check injection target where modules.
                                    if (it.Resolve().Module.Assembly.FullName != module!.Assembly.FullName)
                                    {
                                        // Force ignored.
                                        foundAnotherModule = true;
                                        break;
                                    }
                                    replaced.GenericArguments.Add(it);
                                }
                                if (!foundAnotherModule)
                                {
                                    method.Body.Instructions[index] =
                                        Instruction.Create(i.OpCode, replaced);
                                }
                            }
                            else
                            {
                                var staticMethod = module!.ImportReference(this.queryFacadeMapping[mr]);
                                method.Body.Instructions[index] =
                                    Instruction.Create(i.OpCode, staticMethod);
                            }
                        }
                    }

                    // Step 2-2. Aggregate parameters/record types.
                    var parameterTypes = facadeMethodCallers.
                        Where(inst =>
                            inst.mr!.Name.StartsWith("Parameter") &&
                            inst.mr is GenericInstanceMethod gim &&
                            gim.GenericArguments.Count == 1).
                        Select(inst =>
                            ((GenericInstanceMethod)inst.mr!).GenericArguments[0]).
                        Cast<TypeReference>().
                        ToArray();
                    var recordOnParameterTypes = facadeMethodCallers.
                        Where(inst =>
                            inst.mr!.Name.StartsWith("Parameter") &&
                            inst.mr is GenericInstanceMethod gim &&
                            gim.GenericArguments.Count == 2).
                        Select(inst => ((GenericInstanceMethod)inst.mr!).GenericArguments[0]).
                        Cast<TypeReference>();
                    var parametersOnParameterTypes = facadeMethodCallers.
                        Where(inst =>
                            inst.mr!.Name.StartsWith("Parameter") &&
                            inst.mr is GenericInstanceMethod gim &&
                            gim.GenericArguments.Count == 2).
                        Select(inst => ((GenericInstanceMethod)inst.mr!).GenericArguments[1]).
                        Cast<TypeReference>();
                    var executeTypes = facadeMethodCallers.
                        Where(inst =>
                            inst.mr!.Name.StartsWith("Execute")).
                        Select(inst => ((GenericInstanceMethod)inst.mr!).GenericArguments[0]).
                        Cast<TypeReference>();
                    var parametersTypes =
                        parameterTypes.
                        Concat(parametersOnParameterTypes).
                        ToArray();
                    var recordTypes =
                        recordOnParameterTypes.
                        Concat(executeTypes).
                        ToArray();

                    return (replacer: new Action(Replacer), parametersTypes, recordTypes);
                });

            // Step 2-1-2. Do replace at outside of parallelism.
            //   (Maybe cecil is not safe multithreading.)
            foreach (var entry in aggregatedTypes)
            {
                entry.replacer();
            }

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
                    switch (this.InjectExtractMethod(targetAssembly.MainModule, parametersType))
                    {
                        case InjectResults.Success:
                            parametersInjected++;
                            this.message(
                                LogLevels.Trace,
                                $"Injected an parameter type: TargetAssembly={targetAssemblyName}, Type={parametersType.FullName}");
                            break;
                        case InjectResults.CouldNot:
                            this.message(
                                LogLevels.Warning,
                                $"Could not inject parameter type, because it is declared in another assembly: TargetAssembly={targetAssemblyName}, Type={parametersType.FullName}");
                            break;
                        default:
                            this.message(
                                LogLevels.Trace,
                                $"Ignored an parameter type: TargetAssembly={targetAssemblyName}, Type={parametersType.FullName}");
                            break;
                    }
                }

                foreach (var recordType in recordTypes.
                    OrderBy(t => t, TypeInheritanceDepthComparer.Instance))
                {
                    // By IRecordInjectable interface.
                    switch (this.InjectPrepareMethod(targetAssembly.MainModule, recordType))
                    {
                        case InjectResults.Success:
                            recordInjected++;
                            this.message(
                                LogLevels.Trace,
                                $"Injected an record type: TargetAssembly={targetAssemblyName}, Type={recordType.FullName}");
                            break;
                        case InjectResults.CouldNot:
                            this.message(
                                LogLevels.Warning,
                                $"Could not inject record type, because it is declared in another assembly: TargetAssembly={targetAssemblyName}, Type={recordType.FullName}");
                            break;
                        default:
                            this.message(
                                LogLevels.Trace,
                                $"Ignored an record type: TargetAssembly={targetAssemblyName}, Type={recordType.FullName}");
                            break;
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
