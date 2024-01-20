using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Boomerang : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer boomerangSpriteRenderer;
    [SerializeField] protected float rotationSpeed = 2;
    [SerializeField] protected float damageDelay = 0.1f;

    protected Vector2 initialScale;
    protected float throwTime = 1;
    protected float maxDistance;
    protected float radius;
    protected LayerMask targetLayer;
    protected float damage;
    protected float knockback;
    protected EntityManager entityManager;
    protected Player player;
    protected ZPositioner zPositioner;
    protected int boomerangIndex;
    protected TrailRenderer trailRenderer = null;

    public UnityEvent<float> OnHitDamageable { get; private set; }
    public float Range => maxDistance;

    protected virtual void Awake()
    {
        radius = Mathf.Max(boomerangSpriteRenderer.bounds.size.x, boomerangSpriteRenderer.bounds.size.y) / 2;
        initialScale = boomerangSpriteRenderer.transform.localScale;
        zPositioner = gameObject.AddComponent<ZPositioner>();
        TryGetComponent<TrailRenderer>(out trailRenderer);
    }

    public virtual void Init(EntityManager entityManager, Player player)
    {
        this.entityManager = entityManager;
        this.player = player;
        zPositioner.Init(player.transform);
    }

    public virtual void Setup(int boomerangIndex, Vector2 position, float damage, float knockback, float throwDistance,
        float throwTime, LayerMask targetLayer)
    {
        transform.position = position;
        trailRenderer?.Clear();
        this.boomerangIndex = boomerangIndex;
        this.damage = damage;
        this.knockback = knockback;
        this.targetLayer = targetLayer;
        this.maxDistance = throwDistance;
        this.throwTime = throwTime;
        OnHitDamageable = new UnityEvent<float>();
    }

    public virtual void Throw(Transform returnTransform, Vector2 toPosition)
    {
        Vector2 direction = (toPosition - (Vector2)transform.position);
        direction.Normalize();
        StartCoroutine(ThrowRoutine(returnTransform, direction, maxDistance));
    }

    private IEnumerator ThrowRoutine(Transform returnTransform, Vector2 direction, float throwDistance)
    {
        Dictionary<GameObject, float> hitMonsterTimes = new Dictionary<GameObject, float>();

        Vector2 prevPosition = transform.position;
        Vector2 a = transform.position;
        Vector2 b = (Vector2)transform.position + direction * throwDistance / 2 +
                    new Vector2(direction.y, -direction.x) * throwDistance;
        Vector2 c = (Vector2)transform.position + direction * throwDistance;
        Vector2 d = (Vector2)transform.position + direction * throwDistance / 2 -
                    new Vector2(direction.y, -direction.x) * throwDistance;

        float t = 0;
        while (t < 1)
        {
            Vector2 ab = Vector2.Lerp(a, b, t);
            Vector2 bc = Vector2.Lerp(b, c, t);
            Vector3 abc = Vector3.Lerp(ab, bc, t);

            transform.position = Vector2.Lerp(a, c, EasingUtils.EaseOutQuad(t));

            Vector2 circleCastDir = (Vector2)transform.position - prevPosition;
            RaycastHit2D[] raycastHits = Physics2D.CircleCastAll(prevPosition, radius, circleCastDir.normalized,
                circleCastDir.magnitude, targetLayer);
            foreach (RaycastHit2D raycastHit in raycastHits)
            {
                GameObject hitGameObject = raycastHit.collider.gameObject;
                if (!hitMonsterTimes.ContainsKey(hitGameObject) || hitMonsterTimes[hitGameObject] > damageDelay)
                {
                    hitMonsterTimes[hitGameObject] = 0.0f;
                    IDamageable damageable = hitGameObject.GetComponentInParent<IDamageable>();
                    damageable.TakeDamage(damage, circleCastDir.normalized * knockback);
                    OnHitDamageable?.Invoke(damage);
                }
            }

            prevPosition = transform.position;

            boomerangSpriteRenderer.transform.RotateAround(boomerangSpriteRenderer.transform.position, Vector3.back,
                Time.deltaTime * 100 * rotationSpeed);

            boomerangSpriteRenderer.transform.localScale = Vector3.Lerp(Vector3.zero, initialScale, t * 5);

            GameObject[] keys = hitMonsterTimes.Keys.ToArray();
            foreach (GameObject key in keys)
            {
                hitMonsterTimes[key] += Time.deltaTime;
            }

            t += Time.deltaTime / throwTime;
            yield return null;
        }

        t = 0;
        while (t < 1)
        {
            a = returnTransform.position;
            Vector2 dir = c - a;
            Vector2 cd = Vector2.Lerp(c, d, EasingUtils.EaseInQuad(t));
            Vector2 da = Vector2.Lerp(d, a, EasingUtils.EaseInQuad(t));
            Vector3 cda = Vector2.Lerp(cd, da, EasingUtils.EaseInQuad(t));

            transform.position = Vector2.Lerp(c, a, EasingUtils.EaseInQuad(t));
            

            Vector2 circleCastDir = (Vector2)transform.position - prevPosition;
            RaycastHit2D[] raycastHits = Physics2D.CircleCastAll(prevPosition, radius, circleCastDir.normalized,
                circleCastDir.magnitude, targetLayer);
            foreach (RaycastHit2D raycastHit in raycastHits)
            {
                GameObject hitGameObject = raycastHit.collider.gameObject;
                if (!hitMonsterTimes.ContainsKey(hitGameObject) || hitMonsterTimes[hitGameObject] > damageDelay)
                {
                    hitMonsterTimes[hitGameObject] = 0.0f;
                    IDamageable damageable = hitGameObject.GetComponentInParent<IDamageable>();
                    damageable.TakeDamage(damage); 
                    OnHitDamageable?.Invoke(damage);
                }
            }

            prevPosition = transform.position;

            boomerangSpriteRenderer.transform.RotateAround(boomerangSpriteRenderer.transform.position, Vector3.back,
                Time.deltaTime * 100 * rotationSpeed);

            boomerangSpriteRenderer.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, (t - 0.8f) * 10);

            GameObject[] keys = hitMonsterTimes.Keys.ToArray();
            foreach (GameObject key in keys)
            {
                hitMonsterTimes[key] += Time.deltaTime;
            }

            t += Time.deltaTime / throwTime;
            yield return null;
        }

        DestroyThrowable();
    }

    protected virtual void DestroyThrowable()
    {
        StartCoroutine(DestroyThrowableAnimation());
    }

    protected IEnumerator DestroyThrowableAnimation()
    {
        boomerangSpriteRenderer.enabled = false;
        yield return new WaitForSeconds(0.0f);
        boomerangSpriteRenderer.enabled = true;
        entityManager.DespawnBoomerang(boomerangIndex, this);
    }
}