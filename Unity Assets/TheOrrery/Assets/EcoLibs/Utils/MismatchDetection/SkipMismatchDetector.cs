// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace EcoEngine.MismatchDetection
{
    using Eco.Shared.Text;

    /// <summary>
    /// Implementation of <see cref="IMismatchDetector{T}"/> which may be used for skipping any mismatches between two instances.
    /// It is singleton and can't be explicitly created. Use <see cref="Instance"/> instead.
    /// </summary>
    public class SkipMismatchDetector : IMismatchDetector
    {
        public static readonly SkipMismatchDetector Instance = new SkipMismatchDetector();

        private SkipMismatchDetector() { }

        public InfoBuilder DetectMismatches(object one, object other, MismatchDetectionContext context) => null;
    }
}