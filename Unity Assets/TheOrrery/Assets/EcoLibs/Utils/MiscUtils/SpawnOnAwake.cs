// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.


using UnityEngine;

public class SpawnOnAwake : TrackableBehavior
{ 
    public GameObject obj;

    private void Awake()
    {
        GameObject.Instantiate(obj, this.transform.parent, false);
        GameObject.Destroy(this);
    }
}
