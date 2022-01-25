////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace FlyFlint
{
    public enum LogLevels
    {
        Trace,
        Information,
        Warning,
        Error
    }

    public sealed class Injector
    {
        private readonly Action<LogLevels, string> message;
        private readonly DefaultAssemblyResolver assemblyResolver = new();

        private readonly TypeSystem typeSystem;

        private readonly TypeDefinition queryFacadeExtensionType;
        private readonly TypeDefinition synchronizedQueryFacadeExtensionType;
        private readonly TypeDefinition staticQueryFacadeType;
        private readonly Dictionary<MethodReference, MethodDefinition> queryFacadeMapping;
        private readonly HashSet<MethodReference> queryFacadeGenericMethods;

        // System.Runtime.Serialization.DataContractAttribute.

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

            this.typeSystem = flyFlintCoreAssembly.MainModule.TypeSystem;

            this.queryFacadeExtensionType = flyFlintCoreAssembly.MainModule.GetType(
                "FlyFlint.QueryFacadeExtension")!;
            this.synchronizedQueryFacadeExtensionType = flyFlintCoreAssembly.MainModule.GetType(
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
            this.queryFacadeGenericMethods = new HashSet<MethodReference>(
                this.queryFacadeMapping.Keys.Where(m => m.HasGenericParameters),
                SignatureDroppedGenericTypeEqualityComparer.Instance);
        }

        private static U[] ParallelSelect<T, U>(
            IEnumerable<T> enumerable, Func<T, U> mapper)
        {
#if DEBUG
            return enumerable.Select(mapper).ToArray();
#else
            var results = new List<U>();
            Parallel.ForEach(enumerable, value =>
            {
                var result = mapper(value);
                lock (results)
                {
                    results.Add(result);
                }
            });
            return results.ToArray();
#endif
        }

        private static string GetTypeName(TypeReference type)
        {
            if (type is GenericParameter gp)
            {
                var index = gp.DeclaringType is { } t ?
                    t.GenericParameters.IndexOf(gp) :
                    gp.DeclaringMethod.GenericParameters.IndexOf(gp);
                return $"`t{index}";
            }

            var parentName = type.DeclaringType is { } ? GetTypeName(type.DeclaringType) : type.Namespace;
            if (type is GenericInstanceType git)
            {
                var name = git.Name.Substring(0, git.Name.LastIndexOf('`'));
                return $"{parentName}.{name}<{string.Join(",", git.GenericArguments.Select(GetTypeName))}>";
            }
            else if (type is ArrayType array)
            {
                return $"{parentName}.{array.Name}[{new string(',', array.Dimensions.Count - 1)}]";
            }
            else if (type.IsByReference)
            {
                return $"{parentName}.{type.Name}&";
            }
            else if (type.IsPointer)
            {
                return $"{parentName}.{type.Name}*";
            }
            else
            {
                return $"{parentName}.{type.Name}";
            }
        }

        private static string GetSignatureDroppedGenericType(MethodReference mr)
        {
            var m = mr.GetElementMethod();
            var sig = $"{GetTypeName(m.ReturnType)} {m.Name}{(m.HasGenericParameters ? $"<{string.Join(",", m.GenericParameters.Select(GetTypeName))}>" : "")}({string.Join(",", m.Parameters.Select(p => GetTypeName(p.ParameterType)))})";
            return sig;
        }

        private sealed class SignatureDroppedGenericTypeEqualityComparer : IEqualityComparer<MethodReference?>
        {
            public bool Equals(MethodReference? x, MethodReference? y) =>
                GetSignatureDroppedGenericType(x!) == GetSignatureDroppedGenericType(y!);

            public int GetHashCode(MethodReference? obj) =>
                GetSignatureDroppedGenericType(obj!).GetHashCode();

            public static readonly SignatureDroppedGenericTypeEqualityComparer Instance =
                new SignatureDroppedGenericTypeEqualityComparer();
        }

        private IEnumerable<(TypeReference[], TypeReference[])> EnumerateTargetTypes(AssemblyDefinition targetAssembly)
        {
            var dataContractTypes =
                targetAssembly.Modules.
                    SelectMany(module => module.Types).
                Where(type =>
                    type.CustomAttributes.Any(ca => ca.AttributeType.FullName == "System.Runtime.Serialization.DataContractAttribute") ||
                    type.Fields.Any(f => f.CustomAttributes.Any(ca => ca.AttributeType.FullName == "System.Runtime.Serialization.DataMemberAttribute")) ||
                    type.Properties.Any(p => p.CustomAttributes.Any(ca => ca.AttributeType.FullName == "System.Runtime.Serialization.DataMemberAttribute"))).
                ToArray();

            //static MethodReference GetGenericMethodDefinitionIfApplicable(MethodReference method) =>
            //    method.IsGenericInstance ? method.GetElementMethod() : method;

            var usingQueryTypes = ParallelSelect(
                targetAssembly.Modules.
                    SelectMany(module => module.Types).
                    SelectMany(type => new[] { type }.Concat(type.NestedTypes).SelectMany(t => t.Methods)).
                    Where(method => method.HasBody),
                method =>
                {
                    var facadeMethodCallers = method.Body.Instructions.
                        Select((i, index) => (i, index, mr: i.Operand as MethodReference)).
                        Where(entry =>
                            (entry.i.OpCode == OpCodes.Call || entry.i.OpCode == OpCodes.Callvirt) &&
                            entry.mr != null &&
                            this.queryFacadeMapping.ContainsKey(entry.mr)).
                        ToArray();
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
                    var r = (new TypeReference[0], new TypeReference[0]);
                    return r;
                });

            return usingQueryTypes;
        }

        public bool Inject(string targetAssemblyPath, string? injectedAssemblyPath = null)
        {
            this.assemblyResolver.AddSearchDirectory(
                Path.GetDirectoryName(targetAssemblyPath));

            var targetAssemblyName = Path.GetFileNameWithoutExtension(
                targetAssemblyPath);

            using (var targetAssembly = AssemblyDefinition.ReadAssembly(
                targetAssemblyPath,
                new ReaderParameters(ReadingMode.Immediate)
                {
                    ReadSymbols = true,
                    ReadWrite = false,
                    InMemory = true,
                    AssemblyResolver = this.assemblyResolver,
                }))
            {
                var targetTypes = EnumerateTargetTypes(targetAssembly).
                    ToArray();

                if (targetTypes.Length >= 1)
                {
                    var injected = true;

                    foreach (var targetType in targetTypes)
                    {
                        //if (this.InjectIntoType(targetAssembly.MainModule, targetType))
                        //{
                        //    injected = true;
                        //    this.message(
                        //        LogLevels.Trace,
                        //        $"Injected a view model: Assembly={targetAssemblyName}, Type={targetType.FullName}");
                        //}
                        //else
                        //{
                        //    this.message(
                        //        LogLevels.Trace,
                        //        $"InjectProperties: Ignored a type: Assembly={targetAssemblyName}, Type={targetType.FullName}");
                        //}
                    }

                    if (injected)
                    {
                        injectedAssemblyPath = injectedAssemblyPath ?? targetAssemblyPath;

                        targetAssembly.Write(
                            injectedAssemblyPath,
                            new WriterParameters
                            {
                                WriteSymbols = true,
                                DeterministicMvid = true,
                            });
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
