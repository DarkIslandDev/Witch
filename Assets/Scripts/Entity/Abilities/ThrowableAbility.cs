using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableAbility : Ability
    {
        [Header("Throwable Stats")]
        [SerializeField] protected GameObject throwablePrefab;
        [SerializeField] protected LayerMask monsterLayer;
        [SerializeField] protected float throwRadius;
        [SerializeField] protected UpgradeableDamageRate throwRate;
        [SerializeField] protected UpgradeableDamage damage;
        [SerializeField] protected UpgradeableKnockback knockback;
        [SerializeField] protected UpgradeableWeaponCooldown cooldown;
        [SerializeField] protected UpgradeableProjectileCount throwableCount;
        
        protected float timeSinceLastAttack;
        protected int throwableIndex;

        protected override void Use()
        {
            base.Use();
            gameObject.SetActive(true);
            timeSinceLastAttack = cooldown.Value;
            throwableIndex = entityManager.AddPoolForThrowable(throwablePrefab);
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
            timeSinceLastAttack -= throwableCount.Value/throwRate.Value;
            for (int i = 0; i < throwableCount.Value; i++)
            {
                LaunchThrowable();
                yield return new WaitForSeconds(1/throwRate.Value);
            }
        }
        
        protected virtual void LaunchThrowable()
        {
            Throwable throwable = entityManager.SpawnThrowable(throwableIndex, player.CenterTransform.position, damage.Value, knockback.Value, 0, monsterLayer);
            throwable.Throw((Vector2)player.transform.position + Random.insideUnitCircle * throwRadius);
            throwable.OnHitDamageable.AddListener(player.OnDealDamage.Invoke);
        }
    }