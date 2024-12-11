// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using Eco.Shared.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary> A simple pool that keeps the unused pooled gameobjects under the transform disabled. </summary>
public class LocalPool : TrackableBehavior
{
    [SerializeField] GameObject prefab;

    Queue<GameObject> pool  = new Queue<GameObject>();
    List<GameObject> active = new List<GameObject>();

    /// <summary> Retrieve active object from pool (or instantiate a prefab copy) </summary>
    public GameObject Make(bool lastSibling = true)
    {
        var entry = pool.Any() ? pool.Dequeue() : Instantiate(prefab, transform);
        entry.SetActive(true);
        if (lastSibling) entry.transform.SetAsLastSibling();
        else             entry.transform.SetAsFirstSibling();
        active.Add(entry);
        return entry;        
    }

    /// <summary> Return an object back to the pool and disable it. </summary>
    public void ReturnToPool(GameObject obj)
    {
        DebugUtils.Assert(!pool.Contains(obj), "Pool already contains obj");
        obj.SetActive(false);
        obj.transform.SetAsFirstSibling();
        pool.Enqueue(obj);
        active.Remove(obj);
    }

    /// <summary> Return all active objects back to the pool and disable them. </summary>
    public void ReturnAllToPool() { for (int i = active.Count - 1; i >= 0; i--) { ReturnToPool(active[i]); } }
}
