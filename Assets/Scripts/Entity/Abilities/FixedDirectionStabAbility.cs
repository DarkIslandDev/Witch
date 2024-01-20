using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedDirectionStabAbility : StabAbility
{
    [SerializeField] protected GameObject swordPrefab;
    [SerializeField] protected UpgradeableProjectileCount projectileCount;
    
    private List<Sword> swords;

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
            sword.weaponSize *= 1.2f;
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
        sword.Init(targetLayer, player);
        sword.weaponSpriteRenderer.enabled = false;
        swords.Add(sword);
    }
    
    protected override IEnumerator Stab()
    {
        for (int i = 0; i < swords.Count; i++)
        {
            hitMonsters = new FastList<GameObject>();
            
            timeSinceLastAttack -= stabTime;
            swords[i].weaponSpriteRenderer.enabled = true;
            
            Vector2 dir;
            float t = 0;
            
            if (i % 2 == 0)
            {
                dir = player.IsLeft ? Vector2.left : Vector2.right;
            }
            else
            {
                dir = player.IsLeft ? Vector2.right : Vector2.left;
            }
            
            while (t < stabTime)
            {
                Vector2 attackBoxPosition = (Vector2)player.transform.position +
                                            dir * (weaponSize.x / 2 + stabOffset + stabDistance / stabTime * t);
                float attackAngle = Vector2.SignedAngle(Vector2.right, dir);
                Collider2D[] hitColliders =
                    Physics2D.OverlapBoxAll(attackBoxPosition, weaponSize, attackAngle, targetLayer);

                swords[i].weaponSpriteRenderer.transform.position = attackBoxPosition;

                swords[i].weaponSpriteRenderer.transform.localRotation =
                    Quaternion.Euler(player.IsLeft ? 180 : 0, 0, attackAngle);

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

            Vector2 initialScale = swords[i].weaponSpriteRenderer.transform.localScale;
            t = 0;
            
            while (t < 1)
            {
                swords[i].weaponSpriteRenderer.transform.localPosition =
                    (Vector2)player.transform.position +
                    dir * (swords[i].weaponSpriteRenderer.transform.localScale.x /
                           initialScale.x * weaponSize.x / 2 + stabOffset +
                           stabDistance);
                swords[i].weaponSpriteRenderer.transform.localScale =
                    Vector2.Lerp(initialScale, Vector2.zero, EasingUtils.EaseInQuart(t));
                t += Time.deltaTime * 4;
                yield return null;
            }

            swords[i].weaponSpriteRenderer.transform.localScale = initialScale;
            swords[i].weaponSpriteRenderer.enabled = false;
        }
    }
}