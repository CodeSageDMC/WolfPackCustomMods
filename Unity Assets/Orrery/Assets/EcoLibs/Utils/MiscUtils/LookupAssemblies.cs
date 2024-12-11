// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Eco.Client.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Eco.Shared.Utils;

    public static class LookupAssemblies
    {
        // Excluded assemblies (bug in Unity 2018.3.7-8, https://forum.unity.com/threads/reflection-causing-errors-since-version-2018-3-7f1.641782/, but also may extend this list for optimization)
        private static readonly HashSet<string> ExcludedAssemblies = new HashSet<string> { "mscorlib", "System", "ThirdParty", "UnityEngine", "UnityEditor" };
        private static readonly string[] ExcludePrefixes = { "Unity.", "UnityEngine.", "UnityEditor.", "Microsoft.", "System.", "Editor." };
        public static readonly Assembly[] Assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a =>
        {
            var assemblyName = a.GetName();
            if (ExcludePrefixes.Any(prefix => assemblyName.Name.StartsWith(prefix))) return false;
            if (!a.IsDynamic && a.CodeBase.Contains("/unityjit/")) return false;
            return !ExcludedAssemblies.Contains(assemblyName.Name);
        }).ToArray();

        public static IEnumerable<Type> DerivedTypes<T>(bool includeSelf = false) => Assemblies.DerivedTypes<T>(includeSelf);
    }
}