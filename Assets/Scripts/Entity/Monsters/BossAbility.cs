using System.Collections;
using UnityEngine;

public abstract class BossAbility : MonoBehaviour
{
    [Header("Ability details")] 
    protected BossMonster monster;
    protected EntityManager entityManager;
    protected Player player;
    protected bool active = false;
    protected float useTime;

    public virtual void Init(BossMonster monster, EntityManager entityManager, Player player)
    {
        this.monster = monster;
        this.entityManager = entityManager;
        this.player = player;
    }

    public abstract IEnumerator Activate();

    public virtual void Deactivate()
    {
        active = false;
        StopAllCoroutines();
    }

    public abstract float Score();
}