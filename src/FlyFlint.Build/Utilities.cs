////////////////////////////////////////////////////////////////////////////
//
// Lightweight static O/R mapping builder at compile time.
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyFlint
{
    internal static class Utilities
    {
        public static IEnumerable<T> Traverse<T>(this T self, Func<T, T?> next)
            where T : notnull
        {
            var current = self;
            while (current != null)
            {
                yield return current;
                current = next(current);
            }
        }

        public static U[] ParallelSelect<T, U>(
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

        public static IEnumerable<TypeDefinition> GetAllTypes(this ModuleDefinition module)
        {
            static IEnumerable<TypeDefinition> GetAllTypesRecursive(TypeDefinition type) =>
                new[] { type }.Concat(type.NestedTypes.SelectMany(GetAllTypesRecursive));
            return module.Types.SelectMany(GetAllTypesRecursive);
        }

        public static string GetTypeName(TypeReference type)
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

        public static string GetSignatureDroppedGenericTypeName(MethodReference mr)
        {
            var m = mr.GetElementMethod();
            var sig = $"{GetTypeName(m.ReturnType)} {m.Name}{(m.HasGenericParameters ? $"<{string.Join(",", m.GenericParameters.Select(GetTypeName))}>" : "")}({string.Join(",", m.Parameters.Select(p => GetTypeName(p.ParameterType)))})";
            return sig;
        }

        public static TypeReference GetMemberType(
            ModuleDefinition module, MemberReference member) =>
            module.ImportReference(
                member is FieldReference fr ? fr.FieldType :
                ((PropertyReference)member).PropertyType);

        public static bool IsTargetMember(FieldDefinition field)
        {
            if (!field.IsStatic)
            {
                if (field.IsPublic &&
                    !field.CustomAttributes.Any(ca => ca.AttributeType.FullName == "FlyFlint.QueryIgnoreAttribute"))
                {
                    return true;
                }
                if (!field.IsPublic &&
                    field.CustomAttributes.Any(ca => ca.AttributeType.FullName == "FlyFlint.QueryFieldAttribute"))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsTargetMember(PropertyDefinition property, MethodReference? setter)
        {
            if (setter is { } mr && mr.Resolve() is { } method)
            {
                if (!method.IsStatic)
                {
                    if (method.IsPublic &&
                        !property.CustomAttributes.Any(ca => ca.AttributeType.FullName == "FlyFlint.QueryIgnoreAttribute"))
                    {
                        return true;
                    }
                    if (!method.IsPublic &&
                        property.CustomAttributes.Any(ca => ca.AttributeType.FullName == "FlyFlint.QueryFieldAttribute"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static string GetTargetMemberName(MemberReference member)
        {
            if (member is FieldReference fr)
            {
                if (fr.Resolve().CustomAttributes.
                    FirstOrDefault(ca => ca.AttributeType.FullName == "FlyFlint.QueryFieldAttribute")?.
                    ConstructorArguments[0].Value is string name &&
                    !string.IsNullOrWhiteSpace(name))
                {
                    return name;
                }
                else
                {
                    return fr.Name;
                }
            }
            else
            {
                var pr = (PropertyReference)member;
                if (pr.Resolve().CustomAttributes.
                    FirstOrDefault(ca => ca.AttributeType.FullName == "FlyFlint.QueryFieldAttribute")?.
                    ConstructorArguments[0].Value is string name &&
                    !string.IsNullOrWhiteSpace(name))
                {
                    return name;
                }
                else
                {
                    return pr.Name;
                }
            }
        }

        public static bool IsNullableForMember(
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
    }

    public sealed class SignatureDroppedGenericTypeEqualityComparer : IEqualityComparer<MethodReference?>
    {
        public bool Equals(MethodReference? x, MethodReference? y) =>
            Utilities.GetSignatureDroppedGenericTypeName(x!) == Utilities.GetSignatureDroppedGenericTypeName(y!);

        public int GetHashCode(MethodReference? obj) =>
            Utilities.GetSignatureDroppedGenericTypeName(obj!).GetHashCode();

        public static readonly SignatureDroppedGenericTypeEqualityComparer Instance =
            new SignatureDroppedGenericTypeEqualityComparer();
    }

    public sealed class TypeInheritanceDepthComparer : IComparer<TypeReference?>
    {
        public int Compare(TypeReference? x, TypeReference? y)
        {
            var rx = x!.Resolve();
            var ry = y!.Resolve();
            var xDepthCount = rx.Traverse(t => t.BaseType?.Resolve()).Count();
            var yDepthCount = ry.Traverse(t => t.BaseType?.Resolve()).Count();
            return xDepthCount.CompareTo(yDepthCount);
        }

        public static readonly TypeInheritanceDepthComparer Instance =
            new TypeInheritanceDepthComparer();
    }
}
