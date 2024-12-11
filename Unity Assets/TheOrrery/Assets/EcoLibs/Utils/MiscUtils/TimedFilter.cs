// Copyright (c) Strange Loop Games. All rights reserved.
// See LICENSE file in the project root for full license information.

/// <summary>
/// TimedFilter may be used for filtering items with expiration period.
/// I.e. if you want to prevent spamming with same blockId while digging.
/// Basic usage is following:
/// <code>
/// TimedFilter&lt;Vector3&gt; tf;
/// void Start()
/// {
///     tf = new TimedFilter(0.5f);
/// }
///
/// void Update()
/// {
///     if (tf.Add(Player.obj.position))
///        doSomeThrottledCode(Player.obj.position);
/// }
/// </code>
///
/// In this sample <code>doSomeThrottledCode</code> will not be executed much frequent than once in 0.5 second for unique position.
/// </summary>
/// <typeparam name="T"></typeparam>
public struct TimedFilter<T>
{
    private readonly TimedQueue<T> timedQueue;

    public TimedFilter(float duration) => this.timedQueue = new TimedQueue<T>(duration);

    public bool Add(T item)
    {
        if (this.timedQueue.Contains(item))
            return false;

        this.timedQueue.Enqueue(item);
        return true;
    }
    public void RemoveExpired() { while (this.timedQueue.TryDequeue(out _)) { } }
    public void SetExpirationTime(float expirationTime) => timedQueue.SetExpirationTime(expirationTime);
    public void Clear()                                 => this.timedQueue.Clear();
}