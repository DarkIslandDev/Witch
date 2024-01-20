﻿using System.Collections;
using UnityEngine;

public class Shield : Collectable
{
    [SerializeField] private float timeAmount = 10;
    
    protected override void OnCollected()
    {
        player.TakeGodMode(timeAmount);
        Destroy(gameObject);
    }

}