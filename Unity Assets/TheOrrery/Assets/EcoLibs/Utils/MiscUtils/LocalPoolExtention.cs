// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using UnityEngine;

/// <summary>This class is used to implement a custom LocalPool, if we don't want the own object to deal with its own pool stack, we just call this, this recieves a pool as a parameter, so you can handle all the pools 
/// in one script, making it easy to reference the list, the prefab and the container we mean. 
/// note: needs MonoBehaviour to be able to Instanciate </summary>
public class LocalPoolExtention : LocalPool
{
    /// <summary>We have to give the method the pool we want to check, the prefab its gonna instantiate if it doesn't find an available item, and the container which is where is gonna place the instantiated prefab</summary>
    public static GameObject MakeWithCustomPool(List<GameObject> poolList, GameObject prefabForInstance, Transform container)
    {
        foreach (GameObject item in poolList)
            if (!item.activeInHierarchy)
            {
                item.SetActive(true);
                item.transform.SetParent(container);
                item.transform.SetAsLastSibling();
                return item;
            }

        GameObject prefabInstance = Instantiate(prefabForInstance, container, false);
        prefabForInstance.transform.SetAsLastSibling();
        poolList.Add(prefabInstance);
        return prefabInstance;
    }
}
