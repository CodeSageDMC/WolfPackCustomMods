// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace EcoEngine.MismatchDetection.TMPro
{
    using Eco.Shared.Text;
    using global::TMPro;

    /// <summary> Detector for TMP_InputField mismatches. </summary>
    public class TMPDropdownMismatchDetector : MismatchDetectorBase<TMP_Dropdown>
    {
        /// <inheritdoc cref="IMismatchDetector{T}.DetectMismatches(T,T,EcoEngine.MismatchDetection.MismatchDetectionContext)"/>
        public override InfoBuilder DetectMismatches(TMP_Dropdown one, TMP_Dropdown other, MismatchDetectionContext context)
        {
            var infoBuilder = new InfoBuilder();
            context.AddPropertyMismatchInfo(infoBuilder, nameof(TMP_Dropdown.value), one, other);
            context.AddPropertyMismatchInfo(infoBuilder, nameof(TMP_Dropdown.options), one, other);
            return infoBuilder;
        }
    }
}