using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightedAbilities : IEnumerable<Ability>
{
    private List<Ability> abilities;
    private float weight;
    private bool isWeapon;

    public float Weight { get => weight; set => weight = value; }
    public int Count => abilities.Count;
    public List<Ability> Abilities => abilities;
    
    public WeightedAbilities()
    {
        abilities = new List<Ability>();
        weight = 0;
    }

    public void Add(Ability ability)
    {
        abilities.Add(ability);
        weight += ability.DropWeight;
    }

    public void Remove(Ability ability)
    {
        if (ability != null)
        {
            weight -= ability.DropWeight;
            abilities.Remove(ability);
        }
    }

    public void RemoveAll()
    {
        for (int i = 0; i < abilities.Count; i++)
        {
            weight -= abilities[i].DropWeight;
            abilities.Remove(abilities[i]);
        }
    }
    
    public Ability Find(Predicate<Ability> match)
    {
        if (match == null)
        {
            throw new ArgumentNullException();
        }

        for (int i = 0; i < abilities.Count; i++)
        {
            if (match(abilities[i]))
            {
                return abilities[i];
            }
        }

        return default(Ability);
    }

    public IEnumerator<Ability> GetEnumerator() => (abilities as IEnumerable<Ability>).GetEnumerator();
        
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    
}