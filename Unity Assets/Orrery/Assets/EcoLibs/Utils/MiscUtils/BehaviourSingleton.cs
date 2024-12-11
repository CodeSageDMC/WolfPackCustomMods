// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine;


public class BehaviourSingleton<T> : SubscribableBehavior where T : BehaviourSingleton<T>
{
    public static T obj
    {
        get;
        private set;
    }

    public virtual void Awake()
    {
        if (obj != null) 
            Debug.LogError("Assigning a singleton twice: " + typeof(T).Name);
        
        obj = this.GetComponent<T>();
        UtilCache.SubscribePostDisconnectionEvent?.Invoke(this, Reset);
        Reset();
    }

    //Called when first awake, and each time the client reconnects.
    protected virtual void Reset() { }

    protected override void OnDestroy()
    {
        if (this == obj)
            obj = null;
        base.OnDestroy();
    }
}
