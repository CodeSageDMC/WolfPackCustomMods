// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Contains a pool of meshes and a list of all meshes.
/// Unity Editor: Updates name of "Mesh Pool" object to show current usage.
/// Functions to Add and Get meshes.
/// </summary>
public static class MeshPool
{
    static Queue<Mesh> pooledMeshes = new Queue<Mesh>(1204);
    static HashSet<Mesh> allMeshes = new HashSet<Mesh>(2048);

#if UNITY_EDITOR
    public static int PooledMeshCount => pooledMeshes.Count;
    public static int AllMeshCount => allMeshes.Count;
#endif

    public static void Add(Mesh mesh)
    {
        if (pooledMeshes.Count >= 128)
        {
            allMeshes.Remove(mesh);
            Object.Destroy(mesh);
        }
        else
        {
            mesh.Clear(false);
            mesh.UploadMeshData(false);
            pooledMeshes.Enqueue(mesh);
        }
    }

    public static Mesh Get(int numVertices)
    {
        var largeMesh = numVertices >= 65534;

        Mesh result;
        if (pooledMeshes.Count > 0)
            result = pooledMeshes.Dequeue();
        else
        {
            result = new Mesh { name = "Pooled Mesh" };
            allMeshes.Add(result);
        }
        result.indexFormat = largeMesh ? IndexFormat.UInt32 : IndexFormat.UInt16;
        return result;
    }
}
