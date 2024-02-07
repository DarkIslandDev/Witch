using UnityEngine;

public class HealthUpgradeAbility : FloatUpgradeAbility<UpgradeableHealth>
{
    public override void Select()
    {
        base.Select();
        
        float maxHealth = Mathf.Round((100 + player.BonusHealth) * upgrades[level - 1]);
        player.GainMaxHealth(maxHealth);
    }
}
