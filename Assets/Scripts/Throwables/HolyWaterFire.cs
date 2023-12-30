using System.Collections;
using UnityEngine;

public class HolyWaterFire : MonoBehaviour
{
    [SerializeField] protected ParticleSystem waterParticles;

    public IEnumerator Burn(HolyWaterThrowable holyWater, float damage, float knockback, float duration,
        float waterRadius, float waterDamageRate, LayerMask targetLayer)
    {
        float t = 0;
        while (t < 1.0f)
        {
            transform.localScale = Vector2.one * waterRadius * 2 * EasingUtils.EaseOutQuart(t);
            waterParticles.transform.localScale = transform.localScale;
            t += Time.deltaTime * 3;
            yield return null;
        }

        transform.localScale = Vector2.one * waterRadius * 2;
        waterParticles.transform.localScale = transform.localScale;
        
        int burnCount = Mathf.CeilToInt(duration * waterDamageRate);
        for (int i = 0; i < burnCount; i++)
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, waterRadius, targetLayer);
            foreach (Collider2D collider in hitColliders)
            {
                holyWater.Damage(collider.GetComponent<IDamageable>());
            }

            yield return new WaitForSeconds(1 / waterDamageRate);
        }

        t = 1;
        while (t > 0.0f)
        {
            transform.localScale = Vector2.one * waterRadius * 2 * EasingUtils.EaseOutQuart(t);
            waterParticles.transform.localScale = transform.localScale;
            t -= Time.deltaTime;
            yield return null;
        }
    }
}