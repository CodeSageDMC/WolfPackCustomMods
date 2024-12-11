// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Eco.Client.Utils
{
    using System.Collections;
    using UnityEngine;

    public class AnimatedTilingTexture : TrackableBehavior
    {
        public int columns = 2;
        public int rows = 2;
        public float framesPerSecond = 10f;

        // the current frame to display
        private int index = 0;

        public void Start()
        {
            this.StartCoroutine(this.UpdateTiling());

            // set the tile size of the texture (in UV units), based on the rows and columns
            Vector2 size = new Vector2(1f / this.columns, 1f / this.rows);
            this.GetComponent<Renderer>().sharedMaterial.SetTextureScale("_MainTex", size);
        }

        private IEnumerator UpdateTiling()
        {
            while (true)
            {
                // move to the next index
                this.index++;
                if (this.index >= this.rows * this.columns)
                    this.index = 0;

                // split into x and y indexes
                Vector2 offset = new Vector2(
                    ((float)this.index / this.columns) - (this.index / this.columns),
                    (this.index / this.columns) / (float)this.rows);

                this.GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", offset);

                yield return new WaitForSeconds(1f / this.framesPerSecond);
            }

        }
    }
}
