// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine;

// component to make colliders ignore each other on awake
public class IgnoreCollider : TrackableBehavior
{
    [SerializeField] Collider[] ignoreColliders;  // colliders to be ignored
    [SerializeField] Collider[] otherColliders;   // colliders you want to have ignoreColliers to start ignoring collisions with.

    private void Awake()
    {
        foreach (var other in otherColliders)
            foreach (var ignore in ignoreColliders) Physics.IgnoreCollision(other, ignore);
    }
}
