// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using UnityEngine;

public class PoolMeshOnDestroy : TrackableBehavior
{
    [NonSerialized]
    private Mesh mesh;
    
    /// <summary>
    /// The mesh that we want to be returned to <see cref="MeshPool"/> when this gameObject is destroyed.
    /// </summary>
    public Mesh Mesh => this.mesh;

    public void OnDestroy() => ClearMesh();
    
    /// <summary>
    /// Set mesh that needs to be returned to <see cref="MeshPool"/> when this is destroyed.
    /// </summary>
    /// <param name="newMesh">The mesh that we want to return to <see cref="MeshPool"/> when this is destroyed.</param>
    /// <param name="alsoSetMeshFilter">If true, also GetComponent <see cref="MeshFilter"/> attached to this game object and set its sharedMesh to <see cref="newMesh"/></param>
    /// <param name="returnOldMeshToPool">If true, return current <see cref="mesh"/> to <see cref="MeshPool"/></param>
    public void SetMesh(Mesh newMesh, bool alsoSetMeshFilter = false, bool returnOldMeshToPool = true)
    {
        if (this.mesh == newMesh) return;
        
        // When setting this.mesh to a new value, we might want to return the old mesh to MeshPool.
        if (returnOldMeshToPool)
            ClearMesh();
        
        // We might also want to set MeshFilter attached to this gameobject to that mesh.
        if (alsoSetMeshFilter)
        {
            var meshFilter = GetComponent<MeshFilter>();
            if (meshFilter != null)
                meshFilter.sharedMesh = newMesh;
            else
                Debug.LogWarning($"There is no MeshFilter component attached to {gameObject.name}");
        }
        
        this.mesh = newMesh;
    }
    
    /// <summary>
    /// Return this.<see cref="mesh"/> to <see cref="MeshPool"/> to reuse later. This is automatically called
    /// in <see cref="OnDestroy"/>.
    /// </summary>
    public void ClearMesh()
    {
        if (this.mesh == null) return; // sliced meshes may be null
        
        MeshPool.Add(this.mesh);
        this.mesh = null;
    }
}
