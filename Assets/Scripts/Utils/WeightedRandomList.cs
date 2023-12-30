using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

[Serializable]
public class WeightedRandomList<T>
{
    [Serializable]
    public struct Pair
    {
        public T item;
        public float weight;

        public Pair(T item, float weight)
        {
            this.item = item;
            this.weight = weight;
        }
    }

    public List<Pair> list = new List<Pair>();

    public int Count => list.Count;

    public void Add(T item, float weight) => list.Add(new Pair(item, weight));

    public T GetRandom()
    {
        float totalWeight = list.Sum(p => p.weight);

        float value = Random.value * totalWeight;

        float sumWeight = 0;

        foreach (Pair p in list)
        {
            sumWeight += p.weight;

            if (sumWeight >= value)
            {
                return p.item;
            }
        }

        return default(T);
    }
}
