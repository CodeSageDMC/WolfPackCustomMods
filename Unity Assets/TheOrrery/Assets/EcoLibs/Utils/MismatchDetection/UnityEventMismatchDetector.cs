// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace EcoEngine.MismatchDetection.Unity
{
    using Eco.Shared.Text;
    using UnityEngine.Events;

    /// <summary> Mismatch detector for <see cref="UnityEventBase"/>. </summary>
    public class UnityEventMismatchDetector : MismatchDetectorBase<UnityEventBase>
    {
        /// <inheritdoc cref="IMismatchDetector{T}.DetectMismatches(T,T,EcoEngine.MismatchDetection.MismatchDetectionContext)"/>
        public override InfoBuilder DetectMismatches(UnityEventBase one, UnityEventBase other, MismatchDetectionContext context)
        {
            var infoBuilder = new InfoBuilder();
            var eventCount  = one.GetPersistentEventCount();
            if (eventCount != other.GetPersistentEventCount())
                return infoBuilder.AppendLineLoc($"events count mismatch, {context.OneName}: {eventCount}, {context.OtherName}: {other.GetPersistentEventCount()}");

            for (var i = 0; i < eventCount; ++i)
            {
                if (one.GetPersistentMethodName(i) != other.GetPersistentMethodName(i))
                    infoBuilder.AppendLineLoc($"event method name mismatch at index {i}, {context.OneName}: {one.GetPersistentMethodName(i)}, {context.OtherName}: {other.GetPersistentMethodName(i)}");
            }

            return infoBuilder;
        }
    }
}