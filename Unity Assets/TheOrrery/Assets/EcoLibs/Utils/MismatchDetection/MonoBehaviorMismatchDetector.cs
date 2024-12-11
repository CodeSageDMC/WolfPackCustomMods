// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace EcoEngine.MismatchDetection.Unity
{
    using Eco.Shared.Text;
    using global::UnityEngine;

    /// <summary> <see cref="MonoBehaviour"/> mismatch detector. Compares two behaviours member-by-member. </summary>
    public class MonoBehaviorMismatchDetector : MismatchDetectorBase<MonoBehaviour>
    {
        /// <inheritdoc cref="IMismatchDetector{T}.DetectMismatches(T,T,EcoEngine.MismatchDetection.MismatchDetectionContext)"/>
        public override InfoBuilder DetectMismatches(MonoBehaviour one, MonoBehaviour other, MismatchDetectionContext context) => context.DetectMembersMismatches(one, other, typeof(MonoBehaviour));
    }
}