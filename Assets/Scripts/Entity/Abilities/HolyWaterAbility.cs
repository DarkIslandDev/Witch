using UnityEngine;

public class HolyWaterAbility : ThrowableAbility
{
    [Header("Holy Water Stats")] 
    [SerializeField] protected UpgradeableDuration duration;
    [SerializeField] protected UpgradeableAOE waterRadius;
    [SerializeField] protected UpgradeableDamageRate waterDamageRate;

    protected override void LaunchThrowable()
    {
        HolyWaterThrowable throwable = (HolyWaterThrowable)entityManager.SpawnThrowable(throwableIndex,
            player.CenterTransform.position, damage.Value, knockback.Value, 0, monsterLayer);
        throwable.SetupWater(duration.Value, waterRadius.Value, waterDamageRate.Value);

        Vector2 trowPosition = (Vector2)player.transform.position + Random.insideUnitCircle * throwRadius;
        throwable.Throw(trowPosition);
        throwable.OnHitDamageable.AddListener(player.OnDealDamage.Invoke);
    }
}