using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class WeightedAbilities : IList<Ability>
{
    private List<Ability> abilities;
    private float weight;
    private bool isWeapon;

    public float Weight { get => weight; set => weight = value; }
    public int Count => abilities.Count;
    public bool IsReadOnly => false;
    public List<Ability> Abilities => abilities;

    public Ability this[int index]
    {
        get => abilities[index];
        set
        {
            weight -= abilities[index].DropWeight;
            abilities[index] = value;
            weight += value.DropWeight;
        }
    }

    public WeightedAbilities()
    {
        abilities = new List<Ability>();
        weight = 0;
    }

    public WeightedAbilities(List<Ability> abilities) => this.abilities = abilities;

    public float GetTotalWeight() => abilities.Sum(ability => ability.DropWeight);

    public void Add(Ability ability)
    {
        abilities.Add(ability);
        if (ability != null) weight += ability.DropWeight;
    }

    public void Clear()
    {
        abilities.Clear();
        weight = 0;
    }

    public bool Contains(Ability ability) => abilities.Contains(ability);

    public void CopyTo(Ability[] array, int arrayIndex) => abilities.CopyTo(array, arrayIndex);

    public int IndexOf(Ability ability) => abilities.IndexOf(ability);

    public void Insert(int index, Ability ability)
    {
        abilities.Insert(index, ability);
        if (ability != null) weight += ability.DropWeight;
    }

    public bool Remove(Ability ability)
    {
        if (abilities.Remove(ability))
        {
            if (ability != null) weight -= ability.DropWeight;
            return true;
        }
        return false;
    }

    public void RemoveAt(int index)
    {
        weight -= abilities[index].DropWeight;
        abilities.RemoveAt(index);
    }

    public Ability Find(Predicate<Ability> match)
    {
        if (match == null)
        {
            throw new ArgumentNullException();
        }

        return abilities.FirstOrDefault(t => match(t));
    }

    public IEnumerator<Ability> GetEnumerator() => abilities.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
