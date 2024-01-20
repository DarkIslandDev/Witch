using System.Collections.Generic;
using UnityEngine;

public class IntUpgradeAbility<T> : Ability where T : UpgradeableInt
{
    [SerializeField] protected List<int> upgrades;
    public override string Description => GetUpgradeDescription();

    protected override void Use()
    {
        base.Use();
        gameObject.SetActive(true);
        abilityManager.UpgradeValue<T, int>(upgrades[level]);
    }

    protected override void Upgrade() => abilityManager.UpgradeValue<T, int>(upgrades[level - 1]);

    public override bool RequirementsMet() => level < upgrades.Count;

    protected string GetUpgradeDescription() => DescriptionUtils.GetUpgradeDescription(abilityDescription, upgrades[level]);
}