using System;
using System.Collections.Concurrent;
using UnityEngine;

/// <summary>Dispatches actions from background threads onto Unity's main thread.</summary>
public class UnityMainThread : MonoBehaviour
{
    private static readonly ConcurrentQueue<Action> Queue = new();

    public static void Post(Action action)
    {
        if (action != null) Queue.Enqueue(action);
    }

    void Update()
    {
        while (Queue.TryDequeue(out var a)) a?.Invoke();
    }
}
