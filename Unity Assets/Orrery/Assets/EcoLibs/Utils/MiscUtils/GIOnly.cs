// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine;

public class GIOnly : TrackableBehavior
{
    public void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("OnlyGI");
    }
}
