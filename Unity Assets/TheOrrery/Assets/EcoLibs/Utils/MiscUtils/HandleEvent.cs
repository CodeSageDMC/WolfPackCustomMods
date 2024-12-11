// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

/// <summary> An event that can have callbacks removed via a passed handle. </summary>
public class HandleEvent
{
    private readonly Dictionary<object, Action> callbacks = new Dictionary<object, Action>();

    public void Add(object handle, Action a)
    {
        this.Remove(handle); //Remove the old one
        this.callbacks[handle] = a;
    }

    public void Remove(object handle) => this.callbacks.Remove(handle);

    public void AddAndCall(object handle, Action a)
    {
        this.Add(handle, a);
        a();
    }

    public void Invoke()
    {
        foreach (var action in this.callbacks.Values)
            action.Invoke();
    }

    public void Clear() => this.callbacks.Clear();
}
