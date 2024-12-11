// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine;

public class WorldObjectOccupancyObject : TrackableBehavior
{
    public string blockType;
    public string uniqueName;
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = transform.localToWorldMatrix;

        //outlines the extents of the to-be-placed occupancy block
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
}