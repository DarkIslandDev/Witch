using UnityEngine;

public class HealthPotion : Collectable
{
    [SerializeField] protected float healAmount = 30;
    
    protected override void OnCollected()
    {
        player.GainHealth(healAmount);
        Destroy(gameObject);
    }
}