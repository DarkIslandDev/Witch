using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class Monster : IDamageable
{
    [SerializeField] protected Material defaultMaterial;
    [SerializeField] protected Material hitMaterial;
    [SerializeField] protected Material deathMaterial;
    [SerializeField] protected ParticleSystem deathParticles;
    [SerializeField] protected GameObject shadow;
    protected BoxCollider2D monsterHitBox;
    protected CircleCollider2D monsterLegsCollider;
    protected int monsterIndex;
    protected MonsterBlueprint monsterBlueprint;
    protected MonsterAnimator monsterAnimator;
    protected SpriteRenderer monsterSpriteRenderer;
    protected ZPositioner zPositioner;
    protected float currentHealth;
    protected EntityManager entityManager;
    protected Player player;
    protected new Rigidbody2D rigidbody;
    protected bool knockedBack = false;
    protected Coroutine hitAnimationCoroutine = null;
    protected bool alive;
    protected Transform centerTransform;

    public UnityEvent<Monster> OnKilled { get; } = new UnityEvent<Monster>();
    public Vector2 Position => transform.position;
    public Vector2 Size => monsterLegsCollider.bounds.size;

    protected virtual void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        monsterLegsCollider = GetComponent<CircleCollider2D>();
        monsterAnimator = GetComponentInChildren<MonsterAnimator>();
        monsterSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        zPositioner = gameObject.AddComponent<ZPositioner>();
        
        monsterHitBox = monsterSpriteRenderer.gameObject.AddComponent<BoxCollider2D>();
        monsterHitBox.isTrigger = true;
    }

    public virtual void Init(EntityManager entityManager, Player player)
    {
        this.entityManager = entityManager;
        this.player = player;
        zPositioner.Init(player.transform);
    }

    public virtual void Setup(int monsterIndex, Vector2 position, MonsterBlueprint monsterBlueprint, float hpBuff = 0)
    {
        this.monsterIndex = monsterIndex;
        this.monsterBlueprint = monsterBlueprint;
        rigidbody.position = position;
        transform.position = position;
        
        monsterSpriteRenderer.sharedMaterial = defaultMaterial;
        shadow.SetActive(true);

        currentHealth = monsterBlueprint.hp + hpBuff;
        alive = true;

        entityManager.LivingMonsters.Add(this);

        monsterHitBox.enabled = true;
        monsterHitBox.size = monsterSpriteRenderer.bounds.size;
        monsterHitBox.offset = Vector2.up * monsterHitBox.size.y / 2;
        monsterLegsCollider.radius = monsterHitBox.size.x / 2.5f;
        
        if (centerTransform == null)
        {
            centerTransform = (new GameObject("Center Transform")).transform;
            centerTransform.SetParent(transform);
            centerTransform.position = transform.position + (Vector3)monsterHitBox.offset;
        }

        float spd = Random.Range(monsterBlueprint.moveSpeed - 0.1f, monsterBlueprint.moveSpeed + 0.1f);
        rigidbody.drag = monsterBlueprint.acceleration / (spd * spd);

        rigidbody.velocity = Vector2.zero;

        StopAllCoroutines();
    }

    protected virtual void Update()
    {
        monsterSpriteRenderer.flipX = ((player.transform.position.x - rigidbody.position.x) < 0);
    }

    protected virtual void FixedUpdate() { }

    public override void TakeDamage(float damage, Vector2 knockBack = default)
    {
        if (alive)
        {
            entityManager.SpawnDamageText(monsterHitBox.transform.position, damage, false);
            currentHealth -= damage;

            if (hitAnimationCoroutine != null) StopCoroutine(hitAnimationCoroutine);

            if (knockBack != default)
            {
                rigidbody.velocity += knockBack * Mathf.Sqrt(rigidbody.drag);
                knockedBack = true;
            }

            if (currentHealth > 0)
            {
                hitAnimationCoroutine = StartCoroutine(HitAnimation());
            }
            else
            {
                StartCoroutine(Killed());
            }
        }
    }

    protected IEnumerator HitAnimation()
    {
        monsterSpriteRenderer.sharedMaterial = hitMaterial;

        yield return new WaitForSeconds(0.15f);

        monsterSpriteRenderer.sharedMaterial = defaultMaterial;
        knockedBack = false;
    }

    public virtual IEnumerator Killed(bool killedByPlayer = true)
    {
        alive = false;
        monsterHitBox.enabled = false;

        entityManager.LivingMonsters.Remove(this);

        if (killedByPlayer) DropLoot();
        
        yield return HitAnimation();
        
        deathParticles?.Play();
        
        monsterSpriteRenderer.material = deathMaterial;
        
        float t = 1;
        while (t > 0)
        {
            monsterSpriteRenderer.material.SetFloat("_Dissolve", t);
            t -= Time.deltaTime / 2;
            shadow.SetActive(false);
            
            if(t == 0) yield return null;
        }
        
        yield return new WaitForSeconds(0.2f);

        OnKilled?.Invoke(this);
        OnKilled?.RemoveAllListeners();
        entityManager.DespawnMonster(monsterIndex, this, true);
    }

    protected virtual void DropLoot()
    {
        if (monsterBlueprint.gemLootTable.TryDropLoot(out GemType gemType))
            entityManager.SpawnExpGem((Vector2)transform.position, gemType);

        if (monsterBlueprint.coinLootTable.TryDropLoot(out CoinType coinType))
            entityManager.SpawnCoin((Vector2)transform.position, coinType);
    }

    public override void KnockBack(Vector2 knockBack) => rigidbody.velocity += knockBack * Mathf.Sqrt(rigidbody.drag);
}