using System;
using UnityEngine;

public class Book : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1;
    
    private BookAbility bookAbility;
    private LayerMask layerMask;

    public void Init(BookAbility bookAbility, LayerMask layerMask)
    {
        this.bookAbility = bookAbility;
        this.layerMask = layerMask;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((layerMask & (1 << other.gameObject.layer)) != 0)
        {
            bookAbility.Damage(other.gameObject.GetComponentInParent<Monster>());
        }
    }
}