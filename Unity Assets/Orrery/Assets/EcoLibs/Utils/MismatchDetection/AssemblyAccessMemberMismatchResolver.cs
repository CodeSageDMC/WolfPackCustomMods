// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace EcoEngine.MismatchDetection
{
    using System.Reflection;
    using Eco.Shared.Utils;

    /// <summary> Implementation of <see cref="IMemberMismatchResolver"/> which skips all members which isn't directly accessible from <see cref="assembly"/>. </summary>
    public class AssemblyAccessMemberMismatchResolver : IMemberMismatchResolver
    {
        private readonly Assembly assembly;

        public AssemblyAccessMemberMismatchResolver(Assembly assembly) => this.assembly = assembly;

        /// <inheritdoc cref="IMemberMismatchResolver.ShouldIgnoreMember"/>
        public bool ShouldIgnoreMember(object obj, MemberInfo member)
        {
            switch (member)
            {
                case FieldInfo fieldInfo:
                    // only check fields which is public, protected or protected internal and object type is in Eco assembly, or declaring type is in Eco assembly
                    return !this.assembly.CanAccess(fieldInfo);
                case PropertyInfo propertyInfo:
                    // only check writeable properties or non-value type properties which may be accessed from Eco assembly
                    var canAccess = propertyInfo.CanWrite && this.assembly.CanAccess(propertyInfo.SetMethod) || !propertyInfo.PropertyType.IsValueType && this.assembly.CanAccess(propertyInfo.GetMethod);
                    return !canAccess;
                default:
                    return false;
            }
        }
    }
}