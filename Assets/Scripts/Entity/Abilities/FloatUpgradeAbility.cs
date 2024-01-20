using System.Collections.Generic;
using UnityEngine;

public class FloatUpgradeAbility<T> : Ability where T : UpgradeableFloat
{
    [SerializeField] protected List<float> upgrades;
    
    public override string Description => GetUpgradeDescription();

    protected override void Use()
    {
        base.Use();
        gameObject.SetActive(true);
        abilityManager.UpgradeValue<T, float>(upgrades[level]);
    }

    protected override void Upgrade() => abilityManager.UpgradeValue<T, float>(upgrades[level - 1]);

    public override bool RequirementsMet() => level < upgrades.Count;

    protected new string GetUpgradeDescription() => DescriptionUtils.GetUpgradeDescription(this.abilityDescription, upgrades[level]);
}