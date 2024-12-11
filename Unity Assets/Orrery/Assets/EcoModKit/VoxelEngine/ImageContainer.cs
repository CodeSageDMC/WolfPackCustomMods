// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

#nullable enable

using System;
using UnityEngine;

/// <summary>
/// A container for holding a group of sprites and returning them in a dictionary keyed to their names for later lookup.
/// </summary>
public class ImageContainer : TrackableBehavior
{
    public Sprite[] Sprites = Array.Empty<Sprite>();
}
