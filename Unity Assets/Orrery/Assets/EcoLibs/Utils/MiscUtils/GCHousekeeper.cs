// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Eco.Client.Utils
{
    using System;
    using UnityEngine;

    /// <summary>
    /// <see cref="GCHousekeeper"/> tries to optimize GC for Unity by making full GC runs by request at safe points like UI Open/UI Close.
    /// Incremental GC may not always effectively handle GC pressure created by lot of object allocations and eventually stats running to fast.
    /// Full GC may help to reduce load on incremental GC and start it over.
    /// </summary>
    public static class GCHousekeeper
    {
        const float minInterval = 600f; // minimal interval in seconds between full GC collect runs (10 minutes)

        static float lastFullGC;

        /// <summary>Asks for cleanup. It won't force GC run if it was already called recently. May create performance spikes so should be only used in safe places where it isn't so noticeable (like UI Open/Close events).</summary>
        public static void AskForCleanup()
        {
            var now = Time.realtimeSinceStartup;
            if (now >= lastFullGC + minInterval)
            {
                GC.Collect();
                lastFullGC = now;
            }
        }

        /// <summary>Resets last cleanup time. <see cref="AskForCleanup"/> won't run full GC during <see cref="minInterval"/>.</summary>
        public static void ResetCleanupTime() => lastFullGC = Time.realtimeSinceStartup;
    }
}