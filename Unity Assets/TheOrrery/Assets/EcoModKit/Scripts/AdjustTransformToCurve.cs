// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine;

// this script curves positions on objects to match world curvature.
// most commonly used on lights
public partial class AdjustTransformToCurve : TrackableBehavior
{
    [Tooltip("Maintain local position (useful for lights attached to animated bones)")]
    public bool localPosition;
}
