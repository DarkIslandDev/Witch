using UnityEngine;

public class FloatUpgradeAbility<T> : Ability where T : UpgradeableFloat
{
    [SerializeField] protected float[] upgrades;
    
    public override string Description => GetUpgradeDescriptions();

    protected override void Use()
    {
        base.Use();
        gameObject.SetActive(true);
        abilityManager.UpgradeValue<T, float>(upgrades[level]);
    }

    protected override void Upgrade() => abilityManager.UpgradeValue<T, float>(upgrades[level]);

    public override bool RequirementsMet() => level < upgrades.Length;

    protected new string GetUpgradeDescriptions() => DescriptionUtils.GetUpgradeDescription(this.abilityDescription, upgrades[level]);
}