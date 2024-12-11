// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using UnityEngine;

/// <summary>
/// A container for prefabs that will get loaded by the modkit. Any type of prefab the modkit can load can be used.
/// </summary>
public class ModkitPrefabContainer : TrackableBehavior
{
    public GameObject[] Prefabs = Array.Empty<GameObject>();
}
