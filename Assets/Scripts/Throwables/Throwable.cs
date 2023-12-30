using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Throwable : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer throwableSpriteRenderer;
    [SerializeField] protected float maxDistance;
    [SerializeField] protected int maxBounceCount = 4;
    [SerializeField] protected float throwHeight = 1;
    [SerializeField] protected float bounciness = 0.5f;
    [SerializeField] protected float delayTime = 1;
    [SerializeField] protected float initialRotationSpeed = 2;
    protected float rotationSpeed;
    protected LayerMask targetLayer;
    protected float damage;
    protected float knockback;
    protected EntityManager entityManager;
    protected Player player;
    protected Collider2D col;
    protected ZPositioner zPositioner;
    protected int throwableIndex;
    protected float throwTime;
    public UnityEvent<float> OnHitDamageable { get; private set; }

    public float ThrowTime => throwTime;

    public float Range => maxDistance;

    protected virtual void Awake()
    {
        col = GetComponent<Collider2D>();
        zPositioner = gameObject.AddComponent<ZPositioner>();
    }

    public virtual void Init(EntityManager entityManager, Player player)
    {
        this.entityManager = entityManager;
        this.player = player;
        zPositioner.Init(player.transform);
    }

    public virtual void Setup(int throwableIndex, Vector2 position, float damage, float knockback, float timeInAir,
        LayerMask targetLayer)
    {
        transform.position = position;
        this.throwableIndex = throwableIndex;
        this.damage = damage;
        this.knockback = knockback;
        this.targetLayer = targetLayer;
        col.enabled = true;
        OnHitDamageable = new UnityEvent<float>();
        throwTime = ComputeThrowTime();
    }

    public virtual void Throw(Vector2 toPosition)
    {
        Vector2 direction = (toPosition - (Vector2)transform.position);
        rotationSpeed = (direction.x > 0) ? initialRotationSpeed : -initialRotationSpeed;
        float throwDistance = direction.magnitude;
        if (throwDistance > maxDistance) throwDistance = maxDistance;
        direction.Normalize();
        StartCoroutine(ThrowRoutine(direction, throwDistance));
    }

    public virtual IEnumerator ThrowRoutine(Vector2 direction, float throwDistance)
    {
        float distance = 0;
        int bounceCount = 0;
        Vector3 initialPosition = transform.position;
        // Инициализируем Y и линейную скорость
        float vy = Mathf.Sqrt(2 * PhysicsConstants.g * throwHeight);
        float initialLinearVelocity = InitialVelocityForThrowDistance(throwDistance, vy);
        float linearSpeed = initialLinearVelocity;
        float y = 0;
        while (distance < throwDistance && linearSpeed > 0)
        {
            // Перемещаем позицию линейно
            Vector3 linearPosition = initialPosition + distance * (Vector3)direction;

            // Корректируем положение Y на основе физических уравнений для имитации подпрыгивания
            vy -= PhysicsConstants.g * Time.deltaTime;
            y += vy * Time.deltaTime;
            if (y < 0)
            {
                y = 0;
                vy = -vy * bounciness;
                bounceCount++;
                linearSpeed = (initialLinearVelocity * (maxBounceCount - bounceCount)) / maxBounceCount;
                rotationSpeed *= bounciness;
            }

            transform.position = linearPosition + Vector3.up * y;
            zPositioner.ManuallySetZByY(linearPosition.y);
            if (bounceCount >= maxBounceCount)
                break;

            // Вращение
            throwableSpriteRenderer.transform.RotateAround(throwableSpriteRenderer.transform.position, Vector3.back,
                Time.deltaTime * 100 * rotationSpeed);

            distance += linearSpeed * Time.deltaTime;
            yield return null;
        }

        zPositioner.AutomaticallySetZ();
        float t = 0;
        float rollingSpeed = 1.0f / maxBounceCount * initialLinearVelocity;
        while (t < delayTime)
        {
            Vector3 linearPosition = initialPosition + distance * (Vector3)direction;
            transform.position = linearPosition;
            t += Time.deltaTime;
            distance += rollingSpeed * Time.deltaTime;
            throwableSpriteRenderer.transform.RotateAround(throwableSpriteRenderer.transform.position, Vector3.back,
                Time.deltaTime * 100 * rotationSpeed);
            rollingSpeed *= 0.95f;
            rotationSpeed *= 0.99f;
            yield return null;
        }

        Explode();
    }

    private float InitialVelocityForThrowDistance(float throwDistance, float vy)
    {
        // Harmonic sequence for decreased speed and height after each bounce
        float H = 1;
        for (int i = 1; i <= maxBounceCount; i++)
        {
            H += Mathf.Pow(bounciness, i) * (1 - ((float)i) / maxBounceCount);
        }

        // Total speed needed to go throw distance given gravity, initial velocity and bounciness
        return PhysicsConstants.g * throwDistance / (2 * vy * H);
    }

    protected float ComputeThrowTime()
    {
        float vy = Mathf.Sqrt(2 * PhysicsConstants.g * throwHeight);
        float t = 0;
        for (int i = 1; i <= maxBounceCount; i++)
        {
            t += Mathf.Pow(bounciness, i) * 2 * vy / PhysicsConstants.g;
        }

        return t;
    }

    protected virtual void Explode()
    {
        DestroyThrowable();
    }

    protected virtual void DestroyThrowable()
    {
        StartCoroutine(DestroyThrowableAnimation());
    }

    protected IEnumerator DestroyThrowableAnimation()
    {
        throwableSpriteRenderer.enabled = false;
        //destructionParticleSystem.Play();
        yield return new WaitForSeconds(0.0f);
        throwableSpriteRenderer.enabled = true;
        entityManager.DespawnThrowable(throwableIndex, this);
    }
}