// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using TMPro;
using UnityEngine;

[ExecuteAlways]
public class ContainerObject : TrackableBehavior
{
    public string containerType;
    private int lastNumChildren;
    public TextMeshProUGUI Title;
    public GameObject Container;

#if UNITY_EDITOR
    protected void Awake()
    {
        var go = this.gameObject;
        if (string.IsNullOrEmpty(containerType))
            containerType = go.name;

        var containerObject = this.Container != null ? this.Container : this.gameObject;
        lastNumChildren = containerObject.transform.childCount;
        go.name = $"{this.containerType} [{this.lastNumChildren}]";
        if (Title != null) Title.text = go.name;

        // don't use this component in runtime
        if (!Application.isEditor)
            Destroy(this);
    }

    protected void OnTransformChildrenChanged()
    {
        var container   = this.Container != null ? this.Container.transform : transform;
        var numChildren = container.childCount;
        if (lastNumChildren != numChildren)
        {
            lastNumChildren = numChildren;
            var newName = $"{this.containerType} [{numChildren}]";
            this.gameObject.name = newName;
        }
    }
#endif
}
