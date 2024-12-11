// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Eco.Client.Pooling
{
    using UnityEngine;

    /// <summary>
    /// Component which ensures that <see cref="Mesh"/> returned to mesh pool when component inactive (for <see cref="MeshCollider"/> and/or <see cref="meshFilter"/>).
    /// It may be useful if you use mesh from pool with your game object and always set it before component enabled, but wanna to return it back to pool as soon as game object disabled
    /// (i.e. when returned back to pool).
    /// </summary>
    [SupportsPooling]
    public class PoolMeshOnDisable : TrackableBehavior
    {
        MeshCollider meshCollider;
        MeshFilter   meshFilter;

        /// <summary> Cache component references. </summary>
        private void Awake()
        {
            this.meshCollider = this.GetComponent<MeshCollider>();
            this.meshFilter   = this.GetComponent<MeshFilter>();
        }

        private void OnDisable()
        {
            // return collider mesh back to MeshPool (if MeshCollider component exists)
            Mesh colliderMesh = null;
            if (this.meshCollider != null)
            {
                colliderMesh = this.meshCollider.sharedMesh;
                if (colliderMesh != null)
                    MeshPool.Add(colliderMesh);
                this.meshCollider.sharedMesh = null;
            }

            // return rendering mesh back to MeshPool (if MeshFilter component exists and it isn't same as collider mesh)
            if (this.meshFilter != null)
            {
                var filterMesh = this.meshFilter.sharedMesh;
                if (filterMesh != null && filterMesh != colliderMesh)
                    MeshPool.Add(filterMesh);
                this.meshFilter.sharedMesh = null;
            }
        }
    }
}
