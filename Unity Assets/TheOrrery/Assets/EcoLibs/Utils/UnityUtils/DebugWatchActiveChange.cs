// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System;
using UnityEngine;

//Attach this to a game object to allow trace points when various functions are called by Unity.  Allows you to see what's activating/deactivating something.
public class DebugWatchActiveChange : TrackableBehavior
{
    public void Awake()     => Debug.Log($"Awake {this.gameObject.name}:\n{Environment.StackTrace}");
    public void Start()     => Debug.Log($"Start {this.gameObject.name}:\n{Environment.StackTrace}");
    public void OnDestroy() => Debug.Log($"OnDestroy {this.gameObject.name}:\n{Environment.StackTrace}");
    public void OnEnable()  => Debug.Log($"OnEnable {this.gameObject.name}:\n{Environment.StackTrace}");
    public void OnDisable() => Debug.Log($"OnDisable {this.gameObject.name}:\n{Environment.StackTrace}");

}
