// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Eco.Avatar
{
    using UnityEngine;

    public enum TintType
    {
        Clothing,
        Skin,
        Hair
    }

    public abstract partial class AvatarPart : TrackableBehavior
    {
        // Data
        [Header("Objects From FBX")] public GameObject MalePrefab;
        public GameObject FemalePrefab;

        [Header("Material Override (leave blank to use default avatar materials)")]
        public Material[] CurvedMaterials;

        public Material[] UIMaterials;
        public bool UseMaterialTexture;
        public TintType firstTint;
    }
}
