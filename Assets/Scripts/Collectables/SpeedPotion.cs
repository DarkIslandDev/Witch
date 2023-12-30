using System.Collections;
using UnityEngine;

public class SpeedPotion : Collectable
{
    [SerializeField] protected float speedAmount = 10;
    [SerializeField] protected float timeAmount = 10;
    
    protected override void OnCollected()
    {
        player.TakeSpeedGain(timeAmount, speedAmount);
        Destroy(gameObject);
    }
}