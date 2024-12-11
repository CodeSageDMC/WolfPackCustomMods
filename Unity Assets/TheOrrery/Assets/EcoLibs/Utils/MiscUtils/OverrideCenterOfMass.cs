// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine;

public class OverrideCenterOfMass : TrackableBehavior 
{
    public Transform CenterOfMass;

    private void Awake()
    {
        this.GetComponent<Rigidbody>().centerOfMass = this.CenterOfMass.position;
    }
}
