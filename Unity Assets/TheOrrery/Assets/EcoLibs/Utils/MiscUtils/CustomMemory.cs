// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine;

/// <summary>
/// This script could be useful for similar objects when you need to store specific valuef from client side
/// </summary>
public class CustomMemory : TrackableBehavior
{
    public Vector3 MemoryVector;
    public float   MemoryFloat;
    public bool    MemoryBool;
    public int     MemoryInt;
    public string  MemoryString;
}
