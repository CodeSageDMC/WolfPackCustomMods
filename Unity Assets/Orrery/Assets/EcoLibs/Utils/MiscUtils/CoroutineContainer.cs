// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Defines a container for holding coroutines that must start and stop while their operants are both active and hidden.  Coroutines can not be 
//started or run on inactive objects, so we need to store them on something else, and track them so we can destroy them later.  This is employed
//to allow using coroutines in UI that may be hidden but not destroyed (like when a World Object UI is cached - subscriptions still need to be updated).
public class CoroutineContainer : BehaviourSingleton<CoroutineContainer>
{
    Dictionary<GameObject, Coroutine> ObjToCoroutine = new Dictionary<GameObject, Coroutine>(); //An id associated with each coroutine.

    /// <summary> Create a coroutine out of the passed enumerator, stopping any previous ones with the same ID.  ID is any arbitrary unique value per the category. </summary>
    public void SetCoroutineAndStopPrevious(GameObject obj, IEnumerator routine)
    {
        this.TryStopCoroutine(obj);
        var coroutine = this.StartCoroutine(routine);
        this.ObjToCoroutine[obj] = coroutine;
    }

    /// <summary> Stop the coroutine of the given id, if it exists. </summary>
    public bool TryStopCoroutine(GameObject obj)
    {
        if (this.ObjToCoroutine.TryGetValue(obj, out var coroutine))
        {
            if (coroutine != null)
            {
                this.StopCoroutine(coroutine);
                this.ObjToCoroutine.Remove(obj);
                return true;
            }
        }
        return false;
    }
}