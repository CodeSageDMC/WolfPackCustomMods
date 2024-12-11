// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class AnimatedUV_UI : TrackableBehavior
{
    public Vector2 uvAnimationRate = new Vector2(1.0f, 0.0f);
    public string textureName = "_MainTex";
    public float uvReset;

    private Vector2 uvOffset = Vector2.zero;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        // make instance of a material to change only this object texture offset (not global)
        var materialInstance = Instantiate(image.material);
        image.material = materialInstance;
    }

    void LateUpdate() 
    {
        if (image == null || !image.enabled) return;

        uvOffset += this.uvAnimationRate * Time.deltaTime;
        if (Mathf.Abs(uvOffset.x) >= Mathf.Abs(uvReset)) uvOffset = Vector2.zero;
        image.material.SetTextureOffset(textureName, uvOffset);
    }

}
