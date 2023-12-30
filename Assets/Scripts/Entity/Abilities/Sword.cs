using System.Collections;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public SpriteRenderer weaponSpriteRenderer;
    
    [SerializeField] private Animator swordAnimator;
    [SerializeField] private float rotationSpeed = 1;
    [SerializeField] protected float stabOffset;
    [SerializeField] protected float stabDistance;
    [SerializeField] protected float stabTime;
    
    protected Player player;
    private SwordAbility swordAbility;
    private LayerMask layerMask;
    protected Vector2 weaponSize;
    protected FastList<GameObject> hitMonsters;

    public void Init(SwordAbility swordAbility, LayerMask layerMask, Player player)
    {
        this.swordAbility = swordAbility;
        this.layerMask = layerMask;
        this.player = player;
        weaponSize = weaponSpriteRenderer.bounds.size;
        weaponSpriteRenderer.enabled = true;
    }

    public virtual IEnumerator Stab(float timeSinceLastAttack)
    {
        hitMonsters = new FastList<GameObject>();
        timeSinceLastAttack -= stabTime;
        float t = 0;
        weaponSpriteRenderer.enabled = true;
        Vector2 dir = player.IsLeft ? Vector2.left : Vector2.right;

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

            // foreach (Collider2D collider in hitColliders)
            // {
            //     if (!hitMonsters.Contains(collider.gameObject))
            //     {
            //         hitMonsters.Add(collider.gameObject);
            //         Monster monster = collider.gameObject.GetComponentInParent<Monster>();
            //         DamageMonster(monster, damage.Value, dir * knockback.Value);
            //         player.OnDealDamage.Invoke(damage.Value);
            //     }
            // }
            
            t += Time.deltaTime;
            yield return null;
        }
        
        // Vector2 initialScale = transform.localScale;
        //
        // transform.localScale = initialScale;
        weaponSpriteRenderer.enabled = false;
        
        swordAnimator.SetFloat("Speed", 0);
        StopCoroutine(Stab(timeSinceLastAttack));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((layerMask & (1 << other.gameObject.layer)) != 0)
        {
            swordAbility.Damage(other.gameObject.GetComponentInParent<Monster>());
        }
    }
}