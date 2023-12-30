using UnityEngine;

public class IntUpgradeAbility<T> : Ability where T : UpgradeableInt
{
    [SerializeField] protected int[] upgrades;
    public override string Description => GetUpgradeDescription();

    protected override void Use()
    {
        base.Use();
        gameObject.SetActive(true);
        abilityManager.UpgradeValue<T, int>(upgrades[level]);
    }

    protected override void Upgrade() => abilityManager.UpgradeValue<T, int>(upgrades[level]);

    public override bool RequirementsMet() => level < upgrades.Length;

    protected string GetUpgradeDescription() => DescriptionUtils.GetUpgradeDescription(this.abilityDescription, upgrades[level]);
}