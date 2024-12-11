// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace EcoEngine.MismatchDetection
{
    using System;
    using System.Reflection;

    /// <summary>
    /// This interface may be implemented by any object (which can be used in <see cref="MismatchDetectionContext"/>) for custom mismatch checks for any specific member.
    /// In example component may set some properties during usage which won't break it re-usable state. In this case it may decide to exclude these properties from mismatch check.
    /// </summary>
    public interface IMemberMismatchDetectionAware
    {
        /// <summary>
        /// Called for every compatible <paramref name="member"/> during member mismatch check.
        /// It also passes <paramref name="prefabValue"/> and <paramref name="prefabException"/> if it happened during obtaining <paramref name="prefabValue"/>.
        /// Analyzing member info and prefab value or exception you may decide if this member should be ignored (by passing internal check) or should go for further checks.
        /// </summary>
        bool ShouldIgnoreMemberMismatch(MemberInfo member, object prefabValue, Exception prefabException);
    }
}