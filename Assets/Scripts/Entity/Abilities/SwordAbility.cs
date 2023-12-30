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
    [SerializeField] protected new UpgradeableDamage damage;
    [SerializeField] protected new UpgradeableKnockback knockback;
    [SerializeField] protected UpgradeableRotationSpeed speed;

    private List<Sword> swords;

    [SerializeField] private int currentSword = 0;

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
    }

    protected override void Attack()
    {
        for (int i = 0; i < swords.Count; )
        {
            currentSword = i;
            Debug.Log(currentSword);
            // StartCoroutine(swords[currentSword].Stab(timeSinceLastAttack));
            // i++;

            if (timeSinceLastAttack >= cooldown.Value)
            {
                StartCoroutine(swords[currentSword].Stab(timeSinceLastAttack));
                
            }
            i++;
        }
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
        sword.Init(this, layerMask, player);
        swords.Add(sword);

        for (int i = 0; i < swords.Count; i++)
        {
            float x = 0;
            float y = 0;
            int rotY = 0;
            
            if (i % 2 != 0)
            {
                x = 0.5f;
                y = 0.3f;
                rotY = 0;
            }
            else
            {
                x = -0.5f;
                y = 0.3f;
                rotY = 180;
            }
            
            sword.transform.position = new Vector3(x, y, 0);
            sword.weaponSpriteRenderer.transform.localRotation = new Quaternion(0, 0, rotY, 0);
        }
    }

    public void Damage(IDamageable damageable)
    {
        Vector2 knockbackDirection = (damageable.transform.position - player.transform.position).normalized;
        damageable.TakeDamage(damage.Value, knockback.Value * knockbackDirection);
        player.OnDealDamage?.Invoke(damage.Value);
    }
}