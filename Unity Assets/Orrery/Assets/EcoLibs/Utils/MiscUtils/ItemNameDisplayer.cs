// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine;

[ExecuteAlways]
public class ItemNameDisplayer : TrackableBehavior
{
	void Update()
    {
        if(this.GetComponent<TMPro.TextMeshProUGUI>().text != this.transform.parent.gameObject.name)
            this.GetComponent<TMPro.TextMeshProUGUI>().text = this.transform.parent.gameObject.name;
	}
}
