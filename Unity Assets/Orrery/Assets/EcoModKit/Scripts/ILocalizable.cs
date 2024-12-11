// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine;

public interface ILocalizable
{
    StringToLocalize[] ExtractStrings();
    Component GetComponent();
    void OnApplyChanges();
    void Localize(bool force = false);
    bool IsNewlyAdded { get; set; }
}
