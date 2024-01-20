using System.Collections;
using System.Linq;
using UnityEngine;

public class CrossAbility : Ability
{
    [Header("Cross stats")] 
    [SerializeField] protected GameObject crossPrefab;
    [SerializeField] protected LayerMask monsterLayer;
    [SerializeField] protected float throwRadius;
    [SerializeField] protected float throwTime = 1;
    [SerializeField] protected UpgradeableDamageRate throwRate;
    [SerializeField] protected UpgradeableDamage damage;
    [SerializeField] protected UpgradeableKnockback knockback;
    [SerializeField] protected UpgradeableWeaponCooldown cooldown;
    [SerializeField] protected UpgradeableProjectileCount crossCount;

    protected float timeSinceLastAttack;
    protected int crossIndex;

    protected override void Use()
    {
        base.Use();
        gameObject.SetActive(true);
        timeSinceLastAttack = cooldown.Value;
        crossIndex = entityManager.AddPoolForBoomerang(crossPrefab);
    }

    private void Update()
    {
        timeSinceLastAttack += Time.deltaTime;

        if (timeSinceLastAttack >= cooldown.Value)
        {
            timeSinceLastAttack = Mathf.Repeat(timeSinceLastAttack, cooldown.Value);
            StartCoroutine(Attack());
        }
    }

    protected virtual IEnumerator Attack()
    {
        timeSinceLastAttack -= crossCount.Value / throwRate.Value;

        for (int i = 0; i < crossCount.Value; i++)
        {
            ThrowBoomerang();
            yield return new WaitForSeconds(1 / throwRate.Value);
        }
    }

    protected virtual void ThrowBoomerang()
    {
        Boomerang boomerang = entityManager.SpawnBoomerang(crossIndex, player.CenterTransform.position, damage.Value,
            knockback.Value, throwRadius, throwTime, monsterLayer);
        Vector2 throwPosition;
        Monster[] nearbyEnemies = entityManager.Player.BoomerangCollider.gameObject.GetComponentsInParent<Monster>();

        if (nearbyEnemies.Length > 0)
        {
            throwPosition = nearbyEnemies[Random.Range(0, nearbyEnemies.Length)].transform.position;
        }
        else
        {
            throwPosition = (Vector2)player.transform.position + Random.insideUnitCircle.normalized * throwRadius;
        }
        
        boomerang.Throw(player.transform, throwPosition);
        boomerang.OnHitDamageable.AddListener(player.OnDealDamage.Invoke);
        
    }
}