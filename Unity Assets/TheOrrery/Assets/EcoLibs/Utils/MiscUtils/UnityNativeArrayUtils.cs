// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

#nullable enable
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public static class UnityNativeArrayUtils
{
    public static unsafe void Clear<T>(this ref NativeArray<T> array) where T : struct => UnsafeUtility.MemClear(array.GetUnsafePtr(), (long)array.Length * UnsafeUtility.SizeOf<T>());

    /// <summary>Fill a rect in a given array with the passed value, assuming width of given value for calculating position.</summary>
    public static void SetRect<T>(this ref NativeArray<T> array, int width, RectInt rect, T value) where T : struct
    {
        for (var x = rect.xMin; x < rect.xMax; x++)
        for (var y = rect.yMin; y < rect.yMax; y++)
            array[x + y * width] = value;
    }
}
