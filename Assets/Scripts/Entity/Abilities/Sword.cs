using System.Collections;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public SpriteRenderer weaponSpriteRenderer;
    
    [SerializeField] protected float stabOffset;
    [SerializeField] protected float stabDistance;
    [SerializeField] protected float stabTime;
    
    protected Player player;
    private SwordAbility swordAbility;
    private LayerMask layerMask;
    public Vector2 weaponSize;
    protected FastList<GameObject> hitMonsters;

    [HideInInspector] public bool isLeft;

    public void Init(LayerMask layerMask, Player player)
    {
        this.swordAbility = swordAbility;
        this.layerMask = layerMask;
        this.player = player;
        weaponSize = weaponSpriteRenderer.bounds.size;
        weaponSpriteRenderer.enabled = false;
    }

    public virtual IEnumerator Stab(float timeSinceLastAttack, UpgradeableDamage damage, UpgradeableKnockback knockback)
    {
        hitMonsters = new FastList<GameObject>();
        timeSinceLastAttack -= stabTime;
        float t = 0;
        weaponSpriteRenderer.transform.localScale = weaponSize;
        weaponSpriteRenderer.enabled = true;
        Vector2 dir = isLeft ? Vector2.left : Vector2.right;

        while (t < stabTime)
        {
            
            Vector2 attackBoxPosition = (Vector2)player.transform.position +
                                        dir * (weaponSize.x / 2 + stabOffset +
                                               stabDistance / stabTime * t);
            float attackAngle = Vector2.SignedAngle(Vector2.right, dir);

            weaponSpriteRenderer.transform.position = attackBoxPosition;
            
            Collider2D[] hitColliders =
                Physics2D.OverlapBoxAll(attackBoxPosition, weaponSize, attackAngle, layerMask);
            
            foreach (Collider2D col in hitColliders)
            {
                if (hitMonsters.Contains(col.gameObject)) continue;
                
                hitMonsters.Add(col.gameObject);
                Monster monster = col.gameObject.GetComponentInParent<Monster>();
                DamageMonster(monster, damage.Value, dir * knockback.Value);
                player.OnDealDamage.Invoke(damage.Value);
            }
            
            t += Time.deltaTime;
            yield return null;
        }

        weaponSpriteRenderer.enabled = false;
        StopCoroutine(Stab(timeSinceLastAttack, damage, knockback));
    }
    
    protected virtual void DamageMonster(Monster monster, float damage, Vector2 knockback) => monster.TakeDamage(damage, knockback);
}