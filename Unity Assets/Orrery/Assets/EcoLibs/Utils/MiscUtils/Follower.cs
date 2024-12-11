// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine;

public class Follower : TrackableBehavior 
{
    public string targetTag;

    void LateUpdate()
    {
        var go = GameObject.FindGameObjectWithTag(targetTag);
        if (go != null)
            transform.position = go.transform.position;
    }
}
