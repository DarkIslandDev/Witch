using System.Collections;
using UnityEngine;

public class HolyWaterThrowable : Throwable
{
    [SerializeField] protected HolyWaterFire holyWater;
    [SerializeField] protected ParticleSystem waterExplosion;

    protected float duration;
    protected float waterRadius;
    protected float waterDamageRate;

    public void SetupWater(float duration, float waterRadius, float waterDamageRate)
    {
        this.duration = duration;
        this.waterRadius = waterRadius;
        this.waterDamageRate = waterDamageRate;
    }

    protected override void Explode()
    {
        StartCoroutine(Burn());
    }

    protected IEnumerator Burn()
    {
        throwableSpriteRenderer.enabled = false;
        holyWater.gameObject.SetActive(true);
        waterExplosion.Play();

        yield return StartCoroutine(holyWater.Burn(this, duration, waterRadius, waterDamageRate, targetLayer));
        
        holyWater.gameObject.SetActive(false);

        throwableSpriteRenderer.enabled = true;
        DestroyThrowable();
    }
    
    public void Damage(IDamageable damageable)
    {
        Vector2 knockbackDirection = (damageable.transform.position - transform.position).normalized;
        damageable.TakeDamage(damage, knockback * knockbackDirection);
        player.OnDealDamage.Invoke(damage);
    }
}