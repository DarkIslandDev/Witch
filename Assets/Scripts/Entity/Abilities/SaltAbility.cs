using UnityEngine;

public class SaltAbility : Ability
{
    [Header("Salt stats")] 
    [SerializeField] protected SpriteRenderer saltCircleSprite;
    [SerializeField] protected LayerMask monsterLayer;
    [SerializeField] protected UpgradeableDamage damage;
    [SerializeField] protected UpgradeableAOE raduis;
    [SerializeField] protected UpgradeableProjectileCount projectileCount;
    [SerializeField] protected UpgradeableDamageRate damageRate;
    [SerializeField] protected UpgradeableKnockback knockback;
    
    private float timeSinceLastAttack;
    private FastList<GameObject> hitMonsters;
    // private List<GameObject> saltCircles;
    private PolygonCollider2D damageCollider;

    private void Awake()
    {
        damageCollider = GetComponentInChildren<PolygonCollider2D>();
    }

    public override void Init(AbilityManager abilityManager, EntityManager entityManager, Player player)
    {
        base.Init(abilityManager, entityManager, player);
        transform.SetParent(player.transform);
        transform.localPosition = new Vector3(0, 0.15f, 0);

    }

    protected override void Use()
    {
        base.Use();        
        gameObject.SetActive(true);
        // projectileCount.OnChanged?.AddListener(RefreshSaltCircles);
        hitMonsters = new FastList<GameObject>();
        // saltCircles = new List<GameObject>();
        
        IncreaseSaltCircle();
        
        // for (int i = 0; i < projectileCount.Value; i++)
        // {
        //     AddSaltCircle();
        // }
    }

    protected override void Upgrade()
    {
        base.Upgrade();
        IncreaseSaltCircle();
        
        // RefreshSaltCircles();
    }

    private void Update()
    {
        // saltCircleSprite.transform.Rotate(new Vector3(0,0,transform.rotation.z), 360);

        timeSinceLastAttack += Time.deltaTime;
        
        if (!(timeSinceLastAttack >= 1 / damageRate.Value)) return;
        
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, raduis.Value, monsterLayer);
        foreach (Collider2D col in hitColliders)
        {
            Damage(col.GetComponentInParent<IDamageable>());
        }
        
        timeSinceLastAttack = Mathf.Repeat(timeSinceLastAttack, 1 / damageRate.Value);
    }

    private void IncreaseSaltCircle()
    {
        saltCircleSprite.transform.localScale = Vector3.one * raduis.Value * 2;
        // damageCollider.radius = raduis.Value;
    }
    
    // private void RefreshSaltCircles()
    // {
    //     for (int i = saltCircles.Count; i < projectileCount.Value; i++)
    //     {
    //         AddSaltCircle();
    //     }
    // }
    //
    // private void AddSaltCircle()
    // {
    //     GameObject circle = Instantiate(saltCirclePrefab, transform);
    //     saltCircles.Add(circle);
    //     
    //     SpriteRenderer spriteRenderer = circle.GetComponent<SpriteRenderer>();
    //     spriteRenderer.transform.localScale = Vector3.one * raduis.Value * 2;
    //     
    //     damageCollider.radius = raduis.Value;
    // }

    private void Damage(IDamageable damageable)
    {
        Vector2 knockbackDirection = (damageable.transform.position - transform.position).normalized;
        damageable.TakeDamage(damage.Value, knockback.Value * knockbackDirection);
        player.OnDealDamage?.Invoke(damage.Value);
    }

    private void DeregisterMonster(Monster monster) => hitMonsters.Remove(monster.gameObject);

    private void OnTriggerStay2D(Collider2D collider)
    {
        if (!hitMonsters.Contains(collider.gameObject) && (monsterLayer & (1 << collider.gameObject.layer)) != 0)
        {
            hitMonsters.Add(collider.gameObject);
            Monster monster = collider.gameObject.GetComponentInParent<Monster>();
            monster.OnKilled.AddListener(DeregisterMonster);
            Damage(monster);
        }
    }
}