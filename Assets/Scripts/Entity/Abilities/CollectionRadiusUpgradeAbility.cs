using UnityEngine;

public class CollectionRadiusUpgradeAbility : Ability
{
    [SerializeField] protected UpgradeableAOE radius;
    
    private CircleCollider2D collectCollider;

    private void Awake()
    {
        collectCollider = player.CollectableCollider;
    }

    protected override void Use()
    {
        base.Use();
        gameObject.SetActive(true);
        IncreaseRadius();
    }

    protected override void Upgrade()
    {
        base.Upgrade();
        IncreaseRadius();
    }

    private void IncreaseRadius() => collectCollider.radius = radius.Value;
}