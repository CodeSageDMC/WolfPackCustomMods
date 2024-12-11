// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace EcoEngine.MismatchDetection
{
    using Eco.Shared.Text;

    /// <summary> Interface which may be implemented for using with <see cref="MismatchDetectionContext"/> for detecting and reporting any mismatches between two objects. </summary>
    public interface IMismatchDetector
    {
        /// <summary>
        /// Detects and reports (using <see cref="InfoBuilder"/>) mismatches between <paramref name="one"/> and <paramref name="other"/> objects.
        /// As additional parameter it receives <paramref name="context"/> which may be used to get any context info (like one and other names) and also
        /// for recursive mismatches detection for other references objects with <see cref="MismatchDetectionContext.DetectMismatches"/> method.
        /// </summary>
        InfoBuilder DetectMismatches(object one, object other, MismatchDetectionContext context);
    }

    /// <summary> Same as <see cref="IMismatchDetector"/>, but with typed <see cref="DetectMismatches"/> method. </summary>
    public interface IMismatchDetector<in T> : IMismatchDetector
    {
        /// <summary> Same as <see cref="IMismatchDetector.DetectMismatches"/>, but with typed parameters. </summary>
        InfoBuilder DetectMismatches(T one, T other, MismatchDetectionContext context);
    }

    /// <summary> Base implementation of <see cref="IMismatchDetector{T}"/> interface which helps you to avoid explicit implementation of <see cref="IMismatchDetector"/> interface. </summary>
    public abstract class MismatchDetectorBase<T> : IMismatchDetector<T>
    {
        /// <inheritdoc cref="IMismatchDetector{T}.DetectMismatches(T,T,EcoEngine.MismatchDetection.MismatchDetectionContext)"/>
        public abstract InfoBuilder DetectMismatches(T one, T other, MismatchDetectionContext context);
        /// <inheritdoc cref="IMismatchDetector.DetectMismatches"/>
        InfoBuilder IMismatchDetector.DetectMismatches(object one, object other, MismatchDetectionContext context) => this.DetectMismatches((T)one, (T)other, context);
    }
}