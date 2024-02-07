public class SpeedAbility : FloatUpgradeAbility<UpgradeableMovementSpeed>
{
    public override void Select()
    {
        base.Select();
        player.IncreaseMovementSpeed();
    }

    public override bool RequirementsMet()
    {
        bool baseRequirementsMet = base.RequirementsMet();
        bool movementSpeedInUse = abilityManager.MovementSpeedUpgradeablesCount > 0;
        return baseRequirementsMet && movementSpeedInUse;
    }
}