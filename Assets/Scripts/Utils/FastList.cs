using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FastList<T> : IEnumerable<T>
{
    private List<T> itemList;
    private Dictionary<T, int> indexByItem;

    public int Count => itemList.Count;

    public FastList()
    {
        itemList = new List<T>();
        indexByItem = new Dictionary<T, int>();
    }

    public void Add(T item)
    {
        if (indexByItem.ContainsKey(item))
        {
            Debug.LogError("Итем уже существует в листе");
            return;
        }

        int index = itemList.Count;
        itemList.Add(item);
        
        indexByItem[item] = index;
    }

    public void Remove(T item)
    {
        if (!indexByItem.Remove(item, out int index))
        {
            // Debug.LogError("Итем не найден в листе");
            return;
        }

        int last = itemList.Count - 1;
        // Особый случай: Удаление последнего элемента
        if (index == last)
        {
            itemList.RemoveAt(last);
        }
        else
        {
            T lastItem = itemList[last];
            itemList[index] = lastItem;
            itemList.RemoveAt(last);

            indexByItem[lastItem] = index;
        }
    }

    public bool Contains(T item) => indexByItem.ContainsKey(item);
    
    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)itemList).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // public T this[int index]
    // {
    //     get
    //     {
    //         if (index < 0 || index > this.Count() - 1)
    //         {
    //             throw new ArgumentOutOfRangeException(nameof(index));
    //         }
    //
    //         return this.Skip(index).First();
    //     }
    //     set
    //     {
    //         if (index < 0)
    //         {
    //             throw new ArgumentOutOfRangeException(nameof(index));
    //         }
    //
    //         List<T> current = itemList;
    //         int i = 0;
    //         while (current != null && i < index)
    //         {
    //             current = current;
    //             i++;
    //         }
    //
    //         if (current == null)
    //         {
    //             throw new ArgumentOutOfRangeException(nameof(index));
    //         }
    //     }
    // }
}