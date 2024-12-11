// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using UnityEngine;

public class AnimatedUVs : TrackableBehavior
{
    public int materialIndex = 0;
    public Vector2 uvAnimationRate = new Vector2(1.0f, 0.0f);
    public string textureName = "_MainTex";

    Vector2 uvOffset = Vector2.zero;
    void LateUpdate()
    {
        uvOffset += this.uvAnimationRate * Time.deltaTime;
        if (this.GetComponent<Renderer>().enabled)
        {
            this.GetComponent<Renderer>().materials[materialIndex].SetTextureOffset(textureName, uvOffset);
        }
    }

}
