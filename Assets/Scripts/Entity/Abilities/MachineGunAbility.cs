using UnityEngine;

public class MachineGunAbility : GunAbility
{
    [Header("Machine Gun stats")] 
    [SerializeField] protected GameObject machineGun;
    [SerializeField] protected Transform launchTransform;
    [SerializeField] protected UpgradeableRotationSpeed rotationSpeed;
    [SerializeField] protected float gunRadius;

    protected Vector3 gunDirection = Vector2.right;

    protected override void Update()
    {
        base.Update();

        float reloadRotation = 0;
        float t = timeSinceLastAttack / cooldown.Value;

        if (t > 0 && t < 1) reloadRotation = t * 360;

        float theta = Time.time * rotationSpeed.Value;
        gunDirection = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), 0);
        machineGun.transform.position = player.CenterTransform.position + gunDirection * gunRadius;
        machineGun.transform.rotation = Quaternion.Euler(0,0,Mathf.Rad2Deg * theta - reloadRotation);
    }

    protected override void LaunchProjectile()
    {
        Projectile projectile = entityManager.SpawnProjectile(projectileIndex, launchTransform.position, damage.Value,
            knockback.Value, speed.Value, monsterLayer);
        projectile.OnHitdamageable?.AddListener(player.OnDealDamage.Invoke);
        projectile.Launch(gunDirection);
    }
}