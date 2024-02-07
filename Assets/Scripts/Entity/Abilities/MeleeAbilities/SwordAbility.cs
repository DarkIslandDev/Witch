using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAbility : Ability
{
    [Header("Splash stats")] 
    [SerializeField] protected GameObject swordPrefab;
    [SerializeField] protected LayerMask monsterLayer;

    [SerializeField] protected float attackTime;
    [SerializeField] protected float attackOffset;
    [SerializeField] protected float attackDistance;

    [SerializeField] protected UpgradeableProjectileCount projectileCount;
    [SerializeField] protected UpgradeableDamage damage;
    [SerializeField] protected UpgradeableKnockback knockback;
    [SerializeField] protected UpgradeableWeaponCooldown cooldown;
    [SerializeField] protected UpgradeableAOE aoe;

    protected List<Sword> swords;
    protected FastList<GameObject> hitMonsters;
    protected float timeSinceLastAttack;
    protected Vector2 dir;
    protected Vector2 attackBoxPosition;


    protected override void Use()
    {
        base.Use();
        gameObject.SetActive(true);
        projectileCount.OnChanged?.AddListener(RefreshSwords);
        swords = new List<Sword>();
        timeSinceLastAttack = cooldown.Value;

        for (int i = 0; i < projectileCount.Value; i++)
        {
            AddSword();
        }
    }

    protected override void Upgrade()
    {
        base.Upgrade();
        RefreshSwords();
        UpgradeWeaponSize();
    }

    private void Update()
    {
        timeSinceLastAttack += Time.deltaTime;

        if (timeSinceLastAttack >= cooldown.Value)
        {
            timeSinceLastAttack = Mathf.Repeat(timeSinceLastAttack, cooldown.Value);
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        for (int i = 0; i < swords.Count; i++)
        {
            hitMonsters = new FastList<GameObject>();
            timeSinceLastAttack -= attackTime;
            float t = 0;

            //  Activate current sword
            swords[i].gameObject.SetActive(true);

            // Choosing the correct direction and rotation of the sword
            switch (i)
            {
                case 0:
                    dir = player.IsLeft ? Vector2.left : Vector2.right;
                    swords[i].weaponSpriteRenderer.transform.localRotation =
                        Quaternion.Euler(player.IsLeft ? 180 : 0, 0, player.IsLeft ? 135 : -45);
                    break;
                case 1:
                    dir = player.IsLeft ? Vector2.right : Vector2.left;
                    swords[i].weaponSpriteRenderer.transform.localRotation =
                        Quaternion.Euler(player.IsLeft ? 0 : 180, 0, player.IsLeft ? -45 : 135);
                    break;
            }

            // Pitch
            float attackAngle = -45;
            // Sword position
            attackBoxPosition = (Vector2)player.CenterTransform.position +
                                dir * (swords[i].weaponSize.x / 2 + attackOffset + attackDistance /
                                    attackTime * t);

            //  Setting the correct position of the current sword
            swords[i].transform.position = attackBoxPosition;

            // Attack cycle
            while (t < attackTime)
            {
                // Collect enemy colliders
                Collider2D[] hitColliders =
                    Physics2D.OverlapBoxAll(attackBoxPosition, swords[i].weaponSize, attackAngle, monsterLayer);

                // Damage Cycle
                foreach (Collider2D col in hitColliders)
                {
                    if (!hitMonsters.Contains(col.gameObject))
                    {
                        hitMonsters.Add(col.gameObject);
                        Monster monster = col.gameObject.GetComponentInParent<Monster>();
                        DamageMonster(monster, damage.Value, dir * knockback.Value);
                        player.OnDealDamage.Invoke(damage.Value);
                    }
                }

                t += Time.deltaTime;
                yield return null;
            }

            // Deactivate current sword
            swords[i].gameObject.SetActive(false);
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
        Sword sword = Instantiate(swordPrefab, player.CenterTransform).GetComponent<Sword>();
        sword.Init();
        sword.gameObject.SetActive(false);
        swords.Add(sword);

        sword.weaponSize = swords[0].weaponSize;
        sword.transform.localScale = swords[0].transform.localScale;
    }

    private void UpgradeWeaponSize()
    {
        foreach (Sword sword in swords)
        {
            sword.weaponSize = new Vector2(sword.weaponSize.x + aoe.Value, sword.weaponSize.y + aoe.Value);
            sword.transform.localScale = sword.weaponSize / 2;
            attackBoxPosition = new Vector2(attackBoxPosition.x + 0.1f, attackBoxPosition.y);
        }
    }

    protected virtual void DamageMonster(Monster monster, float damage, Vector2 knockBack) =>
        monster.TakeDamage(damage, knockBack);
}