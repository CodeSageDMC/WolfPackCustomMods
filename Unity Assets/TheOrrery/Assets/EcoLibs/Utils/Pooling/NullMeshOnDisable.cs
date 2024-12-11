// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Eco.Client.Pooling
{
    using UnityEngine;

    /// <summary>
    /// Component which ensures that <see cref="Mesh"/> reference released when component inactive.
    /// It may be useful if you use shared mesh with multiple game objects and always set it before component enabled.
    /// When game object become inactive it will release reference to <see cref="Mesh"/> and as soon as all reference to <see cref="Mesh"/> released it may be destroyed by GC.
    /// </summary>
    [RequireComponent(typeof(MeshFilter))]
    public class NullMeshOnDisable : TrackableBehavior
    {
        private void OnDisable() => this.GetComponent<MeshFilter>().sharedMesh = null;
    }
}
