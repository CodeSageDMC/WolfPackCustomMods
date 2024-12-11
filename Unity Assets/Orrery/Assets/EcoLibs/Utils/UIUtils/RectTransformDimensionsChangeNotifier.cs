// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine.Events;
using Eco.Client.Pooling;

// Helper component to react on RectTransform dimension changes (or any other modification)
// Triggers built in OnRectTransformDimensionsChange callback
[SupportsPooling]
public class RectTransformDimensionsChangeNotifier : TrackableBehavior
{
    public UnityEvent OnRectTransformDimensionsChangeEvent = null;

    // Helper getter to access even through code with null check
    public UnityEvent GetOrCreateEvent()
    {
        if (OnRectTransformDimensionsChangeEvent == null) OnRectTransformDimensionsChangeEvent = new UnityEvent();
        return OnRectTransformDimensionsChangeEvent;
    }

    private void OnRectTransformDimensionsChange() { if (OnRectTransformDimensionsChangeEvent != null) OnRectTransformDimensionsChangeEvent.Invoke(); }
}
