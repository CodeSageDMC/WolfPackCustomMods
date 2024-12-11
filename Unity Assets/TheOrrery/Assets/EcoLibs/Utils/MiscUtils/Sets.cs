// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Eco.Shared.Pools;

/// <summary>
/// Holder for sets <see cref="Pool"/> of type <typeparamref name="T"/>.
/// It uses thread-unsafe fixed size pool for max performance.
/// WARNING: It is intended to be used only in Unity main thread (at least rented and returned)! Using it in another threads may cause unpredictable results.
/// </summary>
public static class Sets<T>
{
    private static readonly PoolService<HashSet<T>> Pool = new PoolService<HashSet<T>>(new ThreadUnsafeFixedSizePool<HashSet<T>>(20), () => new HashSet<T>(), x => x.Clear());

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static HashSet<T> Rent() => Pool.Rent();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Return(HashSet<T> list) => Pool.Return(list);
}