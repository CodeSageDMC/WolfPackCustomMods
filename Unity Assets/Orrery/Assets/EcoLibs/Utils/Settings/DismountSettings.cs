// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Eco.Client.Utils.Settings
{
    using System;
    using UnityEngine;

    /// <summary> game settings for dismounting vehicles </summary>
    [CreateAssetMenu(fileName = "DismountSettings", menuName = "Eco/Settings/Dismount", order = 1)]
    [Serializable]
    public class DismountSettings : ScriptableObject
    {
        public Vector3 Size = new Vector3(.6f, 1.8f, .6f); // player size exit area check
        public float YOffset = 0.1f;                        // small offset from the ground
        public LayerMask Mask = -1;                          // layers to check when dismouting
    }
}
