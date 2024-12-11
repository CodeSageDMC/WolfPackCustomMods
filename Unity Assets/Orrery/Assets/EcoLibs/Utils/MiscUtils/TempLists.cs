// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Eco.Shared.Pools;
using JetBrains.Annotations;

/// <summary>
/// Temp lists for Unity queries like GetComponent which uses thread-unsafe fixed size pool for max performance.
/// Intended for short living lists and have low cache capacity.
/// WARNING: It is intended to be used only in Unity main thread (at least rented and returned)! Using it in another threads may cause unpredictable results.
/// </summary>
public static class TempLists
{
    /// <summary>Syntax sugar for Rent and Return operations which may be used with `using` operator like `using var promise = TempLists.RentAndPromiseToReturn&lt;string&gt;(out var list);`</summary>
    [MustUseReturnValue, PublicAPI] public static PoolService<List<T>>.ReturnPromise RentAndPromiseToReturn<T>(out List<T> value) => PoolHolder<T>.Pool.RentAndPromiseToReturn(out value);

    /// <summary>Rents temp list of type <see typeref="List{T}"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<T> Rent<T>() => PoolHolder<T>.Pool.Rent();
    
    /// <summary>Returns to pool temp list of type <see typeref="List{T}"/> previously rented with <see cref="Rent{T}"/>.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Return<T>(List<T> list) => PoolHolder<T>.Pool.Return(list);
    
    /// <summary>Holder for lists <see cref="Pool"/> of type <typeparamref name="T"/>. It uses thread-unsafe fixed size pool for max performance.</summary>
    static class PoolHolder<T>
    {
        public static readonly PoolService<List<T>> Pool = new(new ThreadUnsafeFixedSizePool<List<T>>(20), () => new List<T>(), x => x.Clear());
    }
}
