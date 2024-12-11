// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Eco.Client.Utils
{
    using UnityEngine;

    public static class GeomUtils
    {
        /// <summary>Checks if point is inside of ellipse defined by center and two radius (extents).</summary>
        public static bool IsPointInEllipse(Vector2 center, Vector2 extents, Vector2 point)
        {
            var delta = point - center;
            return (delta.x * delta.x) / (extents.x * extents.x) + (delta.y * delta.y) / (extents.y * extents.y) <= 1f;
        }
    }
}