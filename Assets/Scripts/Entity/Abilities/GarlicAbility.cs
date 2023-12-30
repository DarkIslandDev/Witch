using System;
using UnityEngine;

public class GarlicAbility : Ability
{
    [Header("Garlic stats")] 
    [SerializeField] protected LayerMask layerMask;
    [SerializeField] protected UpgradeableDamage damage;
    [SerializeField] protected UpgradeableAOE raduis;
    [SerializeField] protected UpgradeableDamageRate damageRate;
    [SerializeField] protected UpgradeableKnockback knockback;
    private float timeSinceLastAttack;
    private FastList<GameObject> hitMonsters;
    private CircleCollider2D damageCollider;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        damageCollider = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public override void Init(AbilityManager abilityManager, EntityManager entityManager, Player player)
    {
        base.Init(abilityManager, entityManager, player);
        transform.SetParent(player.transform);
        transform.localPosition = Vector3.zero;
    }

    protected override void Use()
    {
        base.Use();
        damageCollider.radius = raduis.Value;
        spriteRenderer.transform.localScale = Vector3.one * raduis.Value * 2;
    }

    private void Update()
    {
        timeSinceLastAttack += Time.deltaTime;
        
        if (timeSinceLastAttack >= 1 / damageRate.Value)
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, raduis.Value, layerMask);
            foreach (Collider2D collider in hitColliders)
            {
                Damage(collider.GetComponentInParent<IDamageable>());
            }

            timeSinceLastAttack = Mathf.Repeat(timeSinceLastAttack, 1 / damageRate.Value);
        }
    }

    private void Damage(IDamageable damageable)
    {
        Vector2 knockbackDirection = (damageable.transform.position - transform.position).normalized;
        damageable.TakeDamage(damage.Value, knockback.Value * knockbackDirection);
        player.OnDealDamage?.Invoke(damage.Value);
    }

    private void DeregisterMonster(Monster monster) => hitMonsters.Remove(monster.gameObject);

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!hitMonsters.Contains(collider.gameObject) && (layerMask & (1 << collider.gameObject.layer)) != 0)
        {
            hitMonsters.Add(collider.gameObject);
            Monster monster = collider.gameObject.GetComponentInParent<Monster>();
            monster.OnKilled.AddListener(DeregisterMonster);
            Damage(monster);
        }
    }
}