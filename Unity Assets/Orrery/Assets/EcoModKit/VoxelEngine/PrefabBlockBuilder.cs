// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

#nullable enable

using System;

/// <summary>A base class for chunk prefab builders which return a prefab that should be spawned for a block given the surrounding set of blocks.</summary>
[Serializable]
public abstract partial class PrefabBlockBuilder : BlockBuilder
{
}
