// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.
using System;
using UnityEngine.Events;

/// <summary> Allows to setup OnEnable and OnDisable handlers in the Inspector menu. </summary>
public class OnEnableRetranslator : TrackableBehavior
{   
    [Serializable] public class BoolEvent : UnityEvent<bool> { } // UnityEvent<bool> won't be visible in the Inspector by itself.  So we need to create an inherited class with a [Serializable] attribute

    /// <summary> Sends object's state (when it changes) to handlers. </summary>
    public BoolEvent OnEnableStateChanged; // Handful to bind objects out of hierarchy by triggering SetActive(bool state).

    /// <summary> Triggered on the corresponding event. Handlers can be setup in the Inspector. </summary>
    public UnityEvent OnEnabled, OnDisabled;

    protected virtual void OnEnable()
    {
        this.OnEnabled.Invoke();
        this.OnEnableStateChanged.Invoke(true);
    }

    protected virtual void OnDisable()
    {
        this.OnDisabled.Invoke();
        this.OnEnableStateChanged.Invoke(false);
    }
}
