// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine;

/// <summary>Has an editor button to select referenced game object on scene. Used for editor tools to navigate context objects.</summary>
[ExecuteAlways]
public class ReferenceObjectInspectorHighlighter : TrackableBehavior
{
    public GameObject reference;
}
