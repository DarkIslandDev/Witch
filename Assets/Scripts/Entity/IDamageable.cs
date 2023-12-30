using UnityEngine;

public abstract class IDamageable : MonoBehaviour
{
    public abstract void TakeDamage(float damage, Vector2 knockBack = default(Vector2));
    public abstract void KnockBack(Vector2 knockBack);
}