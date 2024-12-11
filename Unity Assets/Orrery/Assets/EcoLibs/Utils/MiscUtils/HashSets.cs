// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Eco.Shared.Pools;

/// <summary>Similar to <see cref="Lists{T}"/> but for <see cref="HashSet{T}"/></summary>
public static class HashSets<T>
{
    private static readonly PoolService<HashSet<T>> Pool = new PoolService<HashSet<T>>(new ThreadUnsafeFixedSizePool<HashSet<T>>(20), () => new HashSet<T>(), x => x.Clear());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashSet<T> Rent() => Pool.Rent();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Return(HashSet<T> list) => Pool.Return(list);
}