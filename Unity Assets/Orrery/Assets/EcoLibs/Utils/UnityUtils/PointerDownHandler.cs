// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine.EventSystems;
using System;
//This class is used to dynamicly add IPointerDownHandler, since if script will always have it, it will block pointer down event for any other script
//Use sample - LinkText with flag enableClick
public class PointerDownHandler : TrackableBehavior, IPointerDownHandler
{
    public event Action<PointerEventData> OnPointerDownInvoked;
    public void OnPointerDown(PointerEventData eventData) => OnPointerDownInvoked?.Invoke(eventData);
}
