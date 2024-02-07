using System;
using UnityEngine;

public class MeleeMonster : Monster
{
    protected new MeleeMonsterBlueprint monsterBlueprint;
    protected float timeSinceLastAttack;

    public override void Setup(int monsterIndex, Vector2 position, MonsterBlueprint monsterBlueprint, float hpBuff = 0)
    {
        base.Setup(monsterIndex, position, monsterBlueprint, hpBuff);
        this.monsterBlueprint = (MeleeMonsterBlueprint)monsterBlueprint;
    }

    protected override void Update()
    {
        base.Update();

        timeSinceLastAttack += Time.deltaTime;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        Vector2 moveDirection = (player.transform.position - transform.position).normalized;
        rigidbody.velocity += moveDirection * (monsterBlueprint.acceleration * Time.deltaTime);
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (alive && ((monsterBlueprint.meleeLayer & (1 << other.gameObject.layer)) != 0) &&
            timeSinceLastAttack >= 1.0f / monsterBlueprint.atkSpeed)
        {
            player.TakeDamage(monsterBlueprint.atk);
            timeSinceLastAttack = Mathf.Repeat(timeSinceLastAttack, 1.0f / monsterBlueprint.atkSpeed);
        }
    }
}