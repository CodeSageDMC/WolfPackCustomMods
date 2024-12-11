// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine;

[RequireComponent(typeof(Collider), typeof(HighlightableObject))]
public partial class SpecificInteractable : TrackableBehavior
{
    [Tooltip("Tells the server which part of an object you interacted with. Check for it server-side in OnInteract with context.Parameters.ContainsKey")]
    public string interactionTargetName;

    [Tooltip("Tells the server the value of the parameter you want to pass when an interaction takes place with this specific part." +
             "\nThis can be empty if the server doesn't need to know any additional info.")]
    public string interactionTargetValue;
}
