using System.Collections;
using UnityEngine;

public class GunAbility : ProjectileAbility
{
    [Header("Gun stats")] 
    [SerializeField] protected UpgradeableProjectileCount projectileCount;
    [SerializeField] protected UpgradeableDamageRate firerate;

    protected override void Attack()
    {
        StartCoroutine(FireClip());
    }

    protected IEnumerator FireClip()
    {
        int clipSize = projectileCount.Value;
        timeSinceLastAttack -= clipSize / firerate.Value;
        for (int i = 0; i < clipSize; i++)
        {
            LaunchProjectile();
            yield return new WaitForSeconds(1 / firerate.Value);
        }
    }
}