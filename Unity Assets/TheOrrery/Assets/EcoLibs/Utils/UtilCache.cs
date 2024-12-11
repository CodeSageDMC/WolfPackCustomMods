// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.


using System;
using UnityEngine;

/// <summary>Caches various things used by utils and others.</summary>
public static class UtilCache
{
    public static Camera MainCamera
    {
        get
        {
            if (mainCamera == null)
            {
                var cameraObj = GameObject.FindWithTag("MainCamera");
                if (cameraObj == null) return null;
                mainCamera = cameraObj.GetComponent<Camera>();
            }
            return mainCamera;
        }
    }

    static Camera mainCamera;

    public static Action<SubscribableBehavior, Action>  SubscribePostDisconnectionEvent;
    public static Action<Action>                        SubscribePostDisconnectionEventPermanently;
    public static Action<object, string>                RegisterConsoleCommand;
}
