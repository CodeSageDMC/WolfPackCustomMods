// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using Eco.Client.Pooling;
using UnityEngine.UI;

//Interface for components that can have their readonly status set.
public interface IReadOnly
{
    void SetReadOnly(bool readOnly);
}

public enum ReadOnlyType { Hide, Disable }

/// <summary> Thie component will hide and show the game object whenever the parent registers itself 'readonly' or not.</summary>
[SupportsPooling]
public class ReadOnlyHideShow : TrackableBehavior, IReadOnly
{
    public ReadOnlyType ReadOnlyType = ReadOnlyType.Hide;
    public void SetReadOnly(bool readOnly) 
    { 
        if (this.ReadOnlyType == ReadOnlyType.Hide)         this.gameObject.SetActive(!readOnly);
        else if (this.ReadOnlyType == ReadOnlyType.Disable) this.gameObject.GetComponent<Selectable>().interactable = !readOnly;
    }
}
