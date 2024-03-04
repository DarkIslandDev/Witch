using System;
using UnityEngine;

public class MiscTesting : MonoBehaviour
{
    [SerializeField] private EntityManager entityManager;
    [SerializeField] private Player player;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) entityManager.CollectAllCoinsAndGems();

        if (Input.GetKeyDown(KeyCode.G)) entityManager.DamageAllVisibleEnemies(599);
        
        if(Input.GetKeyDown(KeyCode.E)) player.GainExp(1000);
        
        if(Input.GetKeyDown(KeyCode.V)) player.GainExp(player.expirienceToNextLevel);
        
        if(Input.GetKeyDown(KeyCode.X)) player.TakeDamage(player.CurrentHealth);
        
        if(Input.GetKeyDown(KeyCode.C)) player.TakeDamage(10);

        EnableGodMode();
    }

    private void EnableGodMode()
    {
        player.TakeGodMode(Input.GetKey(KeyCode.Z));
    }
}