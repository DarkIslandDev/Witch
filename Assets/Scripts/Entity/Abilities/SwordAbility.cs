using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SwordAbility : MeleeAbility
{
    [Header("Sword stats")] 
    [SerializeField] protected GameObject swordPrefab;
    [SerializeField] protected LayerMask layerMask;
    [SerializeField] protected UpgradeableProjectileCount projectileCount;
    [SerializeField] protected UpgradeableAOE aoe;

    private List<Sword> swords;
    private int currentSword = 0;

    protected override void Use()
    {
        base.Use();
        projectileCount.OnChanged?.AddListener(RefreshSwords);
        swords = new List<Sword>();

        for (int i = 0; i < projectileCount.Value; i++)
        {
            AddSword();
        }
    }

    protected override void Upgrade()
    {
        base.Upgrade();
        RefreshSwords();

        foreach (Sword sword in swords)
        {
            sword.weaponSize *= aoe.Value * 2;
        }
    }

    protected override void Attack()
    {
        StartCoroutine(swords[currentSword].Stab(timeSinceLastAttack, damage, knockback));

        if (currentSword < swords.Count) currentSword++;
        if (currentSword == swords.Count) currentSword = 0;
    }

    private void RefreshSwords()
    {
        for (int i = swords.Count; i < projectileCount.Value; i++)
        {
            AddSword();
        }
    }

    private void AddSword()
    {
        Sword sword = Instantiate(swordPrefab, player.transform).GetComponent<Sword>();
        sword.Init(layerMask, player);
        swords.Add(sword);

        for (int i = 0; i < swords.Count; i++)
        {
            float x = 0;
            float y = 0;
            int rotX = 0;
            int rotY = 0;
            int rotZ = 0;
            
            if (i % 2 == 0)
            {
                x = -aoe.Value - 1f;
                y = 0.3f;
                rotX = 0;
                rotY = 180;
                rotZ = -45;
                sword.isLeft = true;
            }
            else if (i % 2 != 0)
            {
                x = aoe.Value + 1f;
                y = 0.3f;
                rotX = 0;
                rotY = 0;
                rotZ = 0;
                sword.isLeft = false;
            }

            sword.transform.localPosition = new Vector2(x, y);
            sword.weaponSpriteRenderer.transform.localRotation = Quaternion.Euler(rotX, rotY, rotZ);
        }
    }
}