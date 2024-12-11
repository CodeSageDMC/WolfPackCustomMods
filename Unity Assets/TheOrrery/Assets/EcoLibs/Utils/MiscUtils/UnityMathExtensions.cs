// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace Eco.Client.Utils
{
    using System.Runtime.CompilerServices;
    using Eco.Shared.Math;
    using Unity.Mathematics;

    /// <summary> Extensions for Unity math. </summary>
    public static class UnityMathExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2 FloorToInt(this float2 value) => new int2((int)math.floor(value.x), (int)math.floor(value.y));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2i Convert(this int2 value) => new Vector2i(value.x, value.y);

        public static float InverseLerp(float a, float b, float t, bool clamped = true) => clamped ? UnityEngine.Mathf.Clamp01(UnityEngine.Mathf.InverseLerp(a, b, t)) : UnityEngine.Mathf.InverseLerp(a,b,t);

        /// <summary>Remaps a float from one range to another, defaults to remapping to 0 to 1 but outputMin and outputMax can optionally be included to define custom range</summary>
        public static float RemapValueRange(this float value, float inputMin, float inputMax, float outputMin = 0, float outputMax = 1) => (value - inputMin) / (inputMax - inputMin) * (outputMax - outputMin) + outputMin;

        /// <summary>Applies TRS matrix (Translate-Rotate-Scale) to extents.</summary>
        public static float3 TransformExtents(float3 extents, ref float4x4 trs) => math.abs(trs.c0.xyz * extents.x) + math.abs(trs.c1.xyz * extents.y) + math.abs(trs.c2.xyz * extents.z);
    }
}