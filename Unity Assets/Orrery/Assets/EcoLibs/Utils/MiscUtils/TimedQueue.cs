// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Eco.Shared.Collections;
using UnityEngine;

/// <summary>
/// Provides unified queue implementation for items with time mark in Unity.
/// It exposes interface very similar to regular queue, but for every added item it also adds a time mark (<see cref="Time.realtimeSinceStartup"/> + duration).
/// Then <see cref="TryDequeue(out T)"/> returns only items until <see cref="Time.realtimeSinceStartup"/> (inclusive).
/// If you want to get items until specific time mark then you can use overload <see cref="TryDequeue(float,out T)"/>.
///
/// May be used for expiration tracking, for periodic processing (dequeue item process, enqueue to the end with new time mark) etc.
/// </summary>
/// <typeparam name="T">item type</typeparam>
public class TimedQueue<T>
{
    private static readonly Comparer ComparerInstance = new Comparer();

    private readonly float                    duration;
    private readonly RandomAccessQueue<(float, T)> queue;

    public TimedQueue(float duration, int capacity = 16)
    {
        this.duration = duration;
        this.queue = new RandomAccessQueue<(float, T)>(ComparerInstance, capacity);
    }

    public bool Contains(T item) => this.queue.Contains((0, item));

    public void Enqueue(T item) => this.queue.Enqueue((Time.realtimeSinceStartup + this.duration, item));
    public bool Remove(T item)  => this.queue.Remove((0, item));

    public bool TryDequeue(out T value) => this.TryDequeue(Time.realtimeSinceStartup, out value);

    /// <summary>
    /// Return first item from queue till <paramref name="tillTime"/>.
    /// </summary>
    /// <param name="tillTime">time till item will be returned, queue may be not empty, but return false if no items till this time exists</param>
    /// <param name="value">output value</param>
    /// <returns></returns>
    public bool TryDequeue(float tillTime, out T value)
    {
        if (!this.queue.TryDequeue(out var entry))
        {
            value = default;
            return false;
        }

        if (entry.Item1 > tillTime)
        {
            this.queue.EnqueueFirst(entry);
            value = default;
            return false;
        }

        value = entry.Item2;
        return true;
    }

    /// <summary> overrides all items duration with new time </summary>
    public void SetExpirationTime(float expirationTime)
    {
        var count = this.queue.Count;
        for (var i = 0; i < count; ++i)                  // for every item in queue
        {
            var (_, value) = this.queue.Dequeue();       // get it
            this.queue.Enqueue((expirationTime, value)); // and add back with new time
        }
    }

    public void Clear() => this.queue.Clear();

    private class Comparer : IEqualityComparer<(float, T)>
    {
        public bool Equals((float, T) x, (float, T) y) => Equals(x.Item2, y.Item2);
        public int  GetHashCode((float, T) obj)        => obj.Item2.GetHashCode();
    }
}
