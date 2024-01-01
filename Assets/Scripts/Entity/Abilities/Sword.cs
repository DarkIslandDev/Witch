using System.Collections;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public SpriteRenderer weaponSpriteRenderer;
    
    [SerializeField] private Animator swordAnimator;
    [SerializeField] protected float stabOffset;
    [SerializeField] protected float stabDistance;
    [SerializeField] protected float stabTime;
    
    protected Player player;
    private SwordAbility swordAbility;
    private LayerMask layerMask;
    public Vector2 weaponSize;
    protected FastList<GameObject> hitMonsters;

    [HideInInspector] public bool isLeft;

    public void Init(SwordAbility swordAbility, LayerMask layerMask, Player player)
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
            
            Collider2D[] hitColliders =
                Physics2D.OverlapBoxAll(attackBoxPosition, weaponSize, attackAngle, layerMask);

            // transform.position = attackBoxPosition;
            // transform.localRotation =
            //     Quaternion.Euler(player.IsLeft ? 180 : 0, 0, player.IsLeft ? 90 : 0);
            //
            // transform.localRotation = Quaternion.Euler(0, player.IsLeft ? 180 : 0, 0);
            
            swordAnimator.SetFloat("Speed", t * 15);

            foreach (Collider2D collider in hitColliders)
            {
                if (!hitMonsters.Contains(collider.gameObject))
                {
                    hitMonsters.Add(collider.gameObject);
                    Monster monster = collider.gameObject.GetComponentInParent<Monster>();
                    DamageMonster(monster, damage.Value, dir * knockback.Value);
                    player.OnDealDamage.Invoke(damage.Value);
                }
            }
            
            t += Time.deltaTime;
            yield return null;
        }
        
        
        swordAnimator.SetFloat("Speed", 0);
        weaponSpriteRenderer.enabled = false;
        StopCoroutine(Stab(timeSinceLastAttack, damage, knockback));
    }
    
    protected virtual void DamageMonster(Monster monster, float damage, Vector2 knockback) => monster.TakeDamage(damage, knockback);
}