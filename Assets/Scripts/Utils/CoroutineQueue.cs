using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineQueue
{
    private MonoBehaviour owner = null;
    private Coroutine internalCoroutine = null;
    private Queue<IEnumerator> coroutineQueue = new Queue<IEnumerator>();

    public CoroutineQueue(MonoBehaviour coroutineOwner) => owner = coroutineOwner;

    public void StartLoop() => internalCoroutine = owner.StartCoroutine(Process());

    public void StopLoop()
    {
        owner.StopCoroutine(internalCoroutine);
        internalCoroutine = null;
    }

    public void EnqueueCoroutine(IEnumerator coroutine) => coroutineQueue.Enqueue(coroutine);

    private IEnumerator Process()
    {
        while (true)
        {
            if (coroutineQueue.Count > 0)
            {
                yield return owner.StartCoroutine(coroutineQueue.Dequeue());
            }
            else
            {
                yield return null;
            }
        }
    }
}