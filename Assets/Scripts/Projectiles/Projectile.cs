using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer projectileSpriteRenderer;
    [SerializeField] protected float maxDistance;
    [SerializeField] protected float rotationSpeed;
    [SerializeField] protected float airResistance;
    [SerializeField] protected ParticleSystem destructionParticleSystem;

    protected float despawnTime = 1;
    protected LayerMask targetLayer;
    protected float speed;
    protected float damage;
    protected float knockback;
    protected EntityManager entityManager;
    protected Player player;
    protected new Collider2D collider;
    protected ZPositioner zPositioner;
    protected Coroutine moveCoroutine;
    protected int projectileIndex;
    protected Vector2 direction;

    public UnityEvent<float> OnHitdamageable { get; private set; }

    protected virtual void Awake()
    {
        collider = GetComponent<Collider2D>();
        zPositioner = gameObject.AddComponent<ZPositioner>();
    }

    public virtual void Init(EntityManager entityManager, Player player)
    {
        this.entityManager = entityManager;
        this.player = player;
        zPositioner.Init(player.transform);
    }

    public virtual void Setup(int projectileIndex, Vector2 position, float damage, float knockback, float speed,
        LayerMask targetLayer)
    {
        transform.position = position;
        this.projectileIndex = projectileIndex;
        this.damage = damage;
        this.knockback = knockback;
        this.speed = speed;
        this.targetLayer = targetLayer;
        collider.enabled = true;
        OnHitdamageable = new UnityEvent<float>();
    }

    public virtual void Launch(Vector2 direction)
    {
        this.direction = direction.normalized;

        moveCoroutine = StartCoroutine(Move());
    }

    protected virtual IEnumerator Move()
    {
        float distanceTravelled = 0;
        float timeOffScreen = 0;
        while (distanceTravelled < maxDistance && timeOffScreen < despawnTime && speed > 0)
        {
            float step = speed * Time.deltaTime;
            transform.position += step * (Vector3)direction;
            distanceTravelled += step;
            // transform.RotateAround(transform.position, Vector3.back, Time.deltaTime * 100 * rotationSpeed);
            // transform.Rotate(new Vector3(0, 0, direction.x));
            transform.localRotation = Quaternion.RotateTowards(new Quaternion(transform.position.x, transform.position.y, transform.position.z, 0), new Quaternion(direction.x, direction.y, 0,0), 0);
            speed -= airResistance * Time.deltaTime;
            yield return null;
        }

        HitNothing();
    }

    protected virtual void HitDamageable(IDamageable damageable)
    {
        damageable.TakeDamage(damage, knockback * direction);
        OnHitdamageable?.Invoke(damage);
        DestroyProjectile();
    }

    protected virtual void HitNothing() => DestroyProjectile();

    protected virtual void DestroyProjectile() => StartCoroutine(DestroyProjectileAnimation());

    protected IEnumerator DestroyProjectileAnimation()
    {
        projectileSpriteRenderer.gameObject.SetActive(false);
        destructionParticleSystem.Play();
        yield return new WaitForSeconds(destructionParticleSystem.main.duration);
        projectileSpriteRenderer.gameObject.SetActive(true);
        entityManager.DespawnProjectile(projectileIndex, this);
    }

    protected void CollisionCheck(Collider2D collider)
    {
        if ((targetLayer & (1 << collider.gameObject.layer)) != 0)
        {
            this.collider.enabled = false;
            StopCoroutine(moveCoroutine);
            if (collider.transform.parent.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                HitDamageable(collider.gameObject.GetComponentInParent<IDamageable>());
            }
            else
            {
                HitNothing();
            }
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collider) => CollisionCheck(collider);
}