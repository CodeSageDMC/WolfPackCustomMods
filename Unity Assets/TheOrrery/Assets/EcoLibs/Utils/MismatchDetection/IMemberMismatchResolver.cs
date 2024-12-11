// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace EcoEngine.MismatchDetection
{
    using System.Reflection;

    /// <summary> Interface <see cref="IMemberMismatchResolver"/> may be implemented for custom member handling to skip unwanted members. </summary>
    public interface IMemberMismatchResolver
    {
        /// <summary> Will ignore <paramref name="member"/> during mismatch checking if this returns <c>true</c>. </summary>
        bool ShouldIgnoreMember(object obj, MemberInfo member);
    }
}