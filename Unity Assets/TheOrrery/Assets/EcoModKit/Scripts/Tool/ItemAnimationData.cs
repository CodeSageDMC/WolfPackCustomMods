// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using Eco.Shared.Items;
using UnityEngine;

/// <summary> Basic component to setup item animation overrider. Including Modkit. </summary>
public class ItemAnimationData : TrackableBehavior
{
    public ItemAnimationCategory AnimationCategory = ItemAnimationCategory.WorkingSingle;
    
    [Tooltip("Custom animation set for animation states. Override this to get custom animation and behaviours with this item." +
             " If this is used on tpv tool - avatar animator will be used, and hands animator for fpv tool prefab")]
    public CustomAnimsetOverride CustomAnimset; // Set of overriden animation for general actions. Allows to reuse 1 animator for all tools/items
}
