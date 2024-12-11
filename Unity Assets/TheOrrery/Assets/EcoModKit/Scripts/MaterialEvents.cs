// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Eco.Client
{
    using UnityEngine;
    using DG.Tweening;

    [RequireComponent(typeof(Renderer))]
    public class MaterialEvents : TrackableBehavior
    {
        [Header("Shader Inputs")]
        [Tooltip("Enable when using _EmissionColor as your shader input. Acts as a workaround for a known Unity bug.")]
        public bool UseUnityEmissionsWorkaround = false;

        [Tooltip("Shader name (eg '_Cutoff') of property to modify. Specify names for the types you modify. (Color if you use SetColor, etc)")]
        public string ShaderColorName, ShaderFloatName, ShaderIntName, ShaderVectorName;

        [Header("Tweening Input Values")]
        [Tooltip("Colors for use with SetColor action.")]
        [ColorUsage(true, true)]
        public Color[] indexedColors;

        [Tooltip("Vectors for use with SetVector action.")]
        public Vector4[] indexedVectors;

        [ColorUsage(true, true)]
        public Color tweenFrom, tweenTo;
        private Color tweenColor; // this will be used by the Tween to store the current color

        [Header("Tweening Options")]
        public float tweenLength = 2f;
        public Ease easeType = Ease.InOutCubic;
        public LoopType loopType = LoopType.Yoyo;
        public bool tweenOnEnable = false;

        private Renderer r;
        private bool instanced = false;

        public void SetColor(int colorIndex)
        {
            if (!this.instanced) this.ForceInstanceMats();
            if (this.indexedColors.Length == 0) return;
            foreach (Material m in this.r.sharedMaterials)
                m.SetColor(this.ShaderColorName, this.indexedColors[colorIndex]);
        }

        public void TweenColor()
        {
            if (!this.instanced) this.ForceInstanceMats();
            DOTween.To(() => this.tweenColor, (c) => this.SetColor(c), this.tweenTo, this.tweenLength).SetId("tweenColor");
        }

        public void StopTweens()
        {
            DOTween.Kill("tweenColor");
            DOTween.Kill("tweenFloat");
        }

        public void TweenFloat(float target) => DOTween.To(() => this.r.sharedMaterial.GetFloat(this.ShaderFloatName), (value) => this.SetFloat(value), target, this.tweenLength).SetId("tweenFloat");

        public void SetFloat(float value)
        {
            if (!this.instanced) this.ForceInstanceMats();
            foreach (Material m in this.r.sharedMaterials)
                m.SetFloat(this.ShaderFloatName, value);
        }

        public void SetVector(int index)
        {
            if (!this.instanced) this.ForceInstanceMats();
            foreach (Material m in this.r.sharedMaterials)
                m.SetVector(this.ShaderVectorName, this.indexedVectors[index]);
        }

        public void SetInt(int value)
        {
            if (!this.instanced) this.ForceInstanceMats();
            foreach (Material m in this.r.sharedMaterials)
                m.SetInt(this.ShaderIntName, value);
        }

        public void OnEnable()
        {
            if (this.tweenOnEnable)
                this.TweenColor();
        }

        public void OnDisable()
        {
            if (this.tweenOnEnable)
                this.StopTweens();
        }

        #region internal
        void Awake()
        {
            this.r = this.GetComponent<Renderer>();
            this.instanced = false;
        }

        // Instance the material(s) so we don't have to worry about conflicts w/ highlighting
        // This script will break batching, should only be used on rare objects like crafting stations
        private void ForceInstanceMats()
        {
            bool allInstanced = true;

#if ECO_DEV
            //HACK: skip fading, alternatively we need to have fader use an instanced material when it finishes
            ObjectFader f = this.GetComponent<ObjectFader>();
            if (f != null)
                f.FinishFade();
#endif

            foreach (Material m in this.r.sharedMaterials)
                if (!m.name.Contains("Instance"))
                    allInstanced = false;

            if (!allInstanced)
            {
                Material[] mats = this.r.materials;
                this.r.materials = mats;
            }
            this.instanced = true;
        }

        // called by TweenColor
        public void SetColor(Color c)
        {
            this.tweenColor = c;
            foreach (Material m in this.r.sharedMaterials)
            {
                m.SetColor(this.ShaderColorName, c);

                // There is a bug in Unity that prevents the emission color from updating automatically.
                // We enable the keyword after every change to work around.
                if (this.UseUnityEmissionsWorkaround)
                    m.EnableKeyword("_EMISSION");
            }
        }
        #endregion
    }
}
