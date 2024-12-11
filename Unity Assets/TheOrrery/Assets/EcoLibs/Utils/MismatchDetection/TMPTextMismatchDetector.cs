// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace EcoEngine.MismatchDetection.TMPro
{
    using Eco.Shared.Text;
    using global::TMPro;

    /// <summary> Detector for TMP_Text mismatches. </summary>
    public class TMPTextMismatchDetector : MismatchDetectorBase<TMP_Text>
    {
        /// <inheritdoc cref="IMismatchDetector{T}.DetectMismatches(T,T,EcoEngine.MismatchDetection.MismatchDetectionContext)"/>
        public override InfoBuilder DetectMismatches(TMP_Text one, TMP_Text other, MismatchDetectionContext context)
        {
            var infoBuilder = new InfoBuilder();
            if (one.text != other.text && !TMProMismatchDetectionUtils.ShouldIgnoreTextMismatch(one))
                context.AddPropertyMismatchInfo(infoBuilder, nameof(TMP_Text.text), one, other);
            context.AddPropertyMismatchInfo(infoBuilder, nameof(TMP_Text.enabled), one, other);
            return infoBuilder;
        }
    }
}