using UnityEngine;

public class ProjectileAbility : Ability
{
    [Header("Projectile stats")] 
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected LayerMask monsterLayer;
    [SerializeField] protected UpgradeableDamage damage;
    [SerializeField] protected UpgradeableProjectileSpeed speed;
    [SerializeField] protected UpgradeableKnockback knockback;
    [SerializeField] protected UpgradeableWeaponCooldown cooldown;

    protected float timeSinceLastAttack;
    protected int projectileIndex;

    protected override void Use()
    {
        base.Use();
        gameObject.SetActive(true);
        timeSinceLastAttack = cooldown.Value;
        projectileIndex = entityManager.AddPoolForProjectile(projectilePrefab);
    }

    protected virtual void Update()
    {
        timeSinceLastAttack += Time.deltaTime;

        if (timeSinceLastAttack >= cooldown.Value)
        {
            timeSinceLastAttack = Mathf.Repeat(timeSinceLastAttack, cooldown.Value);
            Attack();
        }
    }

    protected virtual void Attack()
    {
        LaunchProjectile();
    }

    protected virtual void LaunchProjectile()
    {
        Projectile projectile = entityManager.SpawnProjectile(projectileIndex, player.CenterTransform.position,
            damage.Value, knockback.Value, speed.Value, monsterLayer);
        projectile?.OnHitdamageable.AddListener(player.OnDealDamage.Invoke);
        projectile!.Launch(player.LookDirection);
    }
}