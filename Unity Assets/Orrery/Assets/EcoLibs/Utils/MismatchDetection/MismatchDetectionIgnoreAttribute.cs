// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace EcoEngine.MismatchDetection
{
    using System;

    /// <summary> This attribute may be used on fields or properties which should be ignored during mismatch detection (for different reasons, like this attribute initialized only once on demand, but may not be initialized in prefab). </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class MismatchDetectionIgnoreAttribute : Attribute { }
}
