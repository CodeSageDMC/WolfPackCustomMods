// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace EcoEngine.MismatchDetection.TMPro
{
    using Eco.Shared.Text;
    using global::TMPro;

    /// <summary> Detector for TMP_InputField mismatches. </summary>
    public class TMPInputFieldMismatchDetector : MismatchDetectorBase<TMP_InputField>
    {
        /// <inheritdoc cref="IMismatchDetector{T}.DetectMismatches(T,T,EcoEngine.MismatchDetection.MismatchDetectionContext)"/>
        public override InfoBuilder DetectMismatches(TMP_InputField one, TMP_InputField other, MismatchDetectionContext context)
        {
            var infoBuilder = new InfoBuilder();
            if (one.text != other.text && !TMProMismatchDetectionUtils.ShouldIgnoreTextMismatch(one))
                context.AddPropertyMismatchInfo(infoBuilder, nameof(TMP_InputField.text), one, other);
            context.AddPropertyMismatchInfo(infoBuilder, nameof(TMP_InputField.enabled), one, other);
            return infoBuilder;
        }
    }
}