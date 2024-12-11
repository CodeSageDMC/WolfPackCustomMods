// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine.EventSystems;

namespace Eco.Client.UnityUtils
{
    //A place you can stick breakpoints to see when transforms change.
    public class UITracker : UIBehaviour
    {
        protected override void OnBeforeTransformParentChanged() { base.OnBeforeTransformParentChanged(); }
        protected override void OnRectTransformDimensionsChange() { base.OnRectTransformDimensionsChange(); }
        
    }
}
