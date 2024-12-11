// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine;
using UnityEngine.UI;

// Helper, that scales object using width ratio for target object (parent).
// Can be usefull on objects in Non-Scaled canvases to remove layouting pain.
public class ScaleByParentWidthRatio : TrackableBehavior
{
    private Image target;
    private RectTransform rt;

    private void Start()
    {
        rt = GetComponent<RectTransform>();
        rt.localScale = Vector3.one;
        target = transform.parent.GetComponent<Image>();
        if (target == null) return;

        // Subscribe to any dimensions change on our target
        var notifier = target.gameObject.GetOrAddComponent<RectTransformDimensionsChangeNotifier>();
        notifier.GetOrCreateEvent().AddListener(SetupScale);
        SetupScale();
    }

    private void SetupScale()
    {
        var targetTransform = target.GetComponent<RectTransform>();
        var sourceWidth = target.preferredWidth;
        var currentWidth = targetTransform.rect.width;
        var scale = currentWidth / sourceWidth;

        // Update scale of current object with width ratio of target object
        rt.localScale = new Vector3(scale, scale,  this.transform.localScale.z);

        // Trigger rebuild, cause objects under scale resizing can be buggy, especialy content size fitters and other layout managers
        LayoutRebuilder.ForceRebuildLayoutImmediate(targetTransform);
    }
}
