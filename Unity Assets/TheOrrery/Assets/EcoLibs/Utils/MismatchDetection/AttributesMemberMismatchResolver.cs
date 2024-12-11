// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace EcoEngine.MismatchDetection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Eco.Shared.Utils;

    /// <summary>
    /// Implementation of <see cref="IMemberMismatchResolver"/> which scans register types for <see cref="MismatchDetectionIgnoreAttribute"/>,
    /// adds it to <see cref="mapping"/> and ignores when requested in <see cref="ShouldIgnoreMember"/>.
    /// </summary>
    public class AttributesMemberMismatchResolver : IMemberMismatchResolver
    {
        private readonly Dictionary<Type, HashSet<string>> mapping = new Dictionary<Type, HashSet<string>>();

        /// <summary> Scans <paramref name="type"/> for mismatch detection attributes (<see cref="MismatchDetectionIgnoreAttribute"/>) and updates <see cref="mapping"/>. </summary>
        public void ScanType(Type type)
        {
            // find all members with [MismatchDetectionIgnore] attribute
            foreach (var member in type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(m => m.HasAttribute<MismatchDetectionIgnoreAttribute>()))
            {
                // add for current type  and for every base type till declaring type
                var declaringType = member.DeclaringType;
                while (true)
                {
                    this.mapping.AddToSet(type, member.Name);
                    if (type == declaringType) break;
                    // ReSharper disable once PossibleNullReferenceException
                    type = type.BaseType;
                }
            }
        }

        /// <inheritdoc cref="IMemberMismatchResolver.ShouldIgnoreMember"/>
        public bool ShouldIgnoreMember(object obj, MemberInfo member) => this.mapping.TryGetValue(obj.GetType(), out var memberNames) && memberNames.Contains(member.Name);
    }
}