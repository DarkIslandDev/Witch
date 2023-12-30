using System;
using UnityEngine;
using UnityEngine.Events;

public interface IUpgradeableValue
{
    int UpgradeCount { get; }

    void Register(AbilityManager abilityManager);
    void RegisterInUse();
    void Upgrade();
    string GetUpgradeDescription();
}

public abstract class UpgradeableValue<T> : IUpgradeableValue
{
    [SerializeField] protected T value;
    [SerializeField] protected T[] upgrades;
    protected AbilityManager abilityManager;
    protected int level = 0;

    protected virtual string UpgradeName { get; set; }

    public virtual T Value
    {
        get => value;
        set => this.value = value;
    }

    public int UpgradeCount { get => upgrades.Length; }

    public UnityEvent OnChanged { get; } = new UnityEvent();

    public void Register(AbilityManager abilityManager) => this.abilityManager = abilityManager;

    public abstract void RegisterInUse();

    public void Upgrade()
    {
        if (level < upgrades.Length)
        {
            Upgrade(upgrades[level++]);
        }
    }

    public abstract void Upgrade(T upgrade);

    public abstract string GetUpgradeDescription();
}

///////////////////////////////////////////////////////////////////
/// Улучшаемые float
///////////////////////////////////////////////////////////////////
public abstract class UpgradeableFloat : UpgradeableValue<float>
{
    public override void Upgrade(float upgrade)
    {
        value *= (1 + upgrade);
        OnChanged?.Invoke();
    }

    public override string GetUpgradeDescription()
    {
        if (level >= upgrades.Length || upgrades[level] == 0) return string.Empty;
        return DescriptionUtils.GetUpgradeDescription(LocalizationManager.GetTranslate(UpgradeName), upgrades[level]);
    }
}

[Serializable]
public class UpgradeableDamage : UpgradeableFloat
{
    protected override string UpgradeName { get; set; } = "atkpower_UpKey";

    public override void RegisterInUse()
    {
        abilityManager.DamageUpgradeablesCount++;
    }
}

[Serializable]
public class UpgradeableDamageRate : UpgradeableFloat
{
    protected override string UpgradeName { get; set; } = "atkspeed_UpKey";

    public override void RegisterInUse()
    {
        abilityManager.FireRateUpgradeablesCount++;
    }
}

[Serializable]
public class UpgradeableWeaponCooldown : UpgradeableFloat
{
    protected override string UpgradeName { get; set; } = "weaponcooldown_UpKey";

    public override void RegisterInUse()
    {
        abilityManager.WeaponCooldownUpgradeablesCount++;
    }
}

[Serializable]
public class UpgradeableRecoveryCooldown : UpgradeableFloat
{
    protected override string UpgradeName { get; set; } = "recoverycooldown_UpKey";

    public override void RegisterInUse()
    {
        abilityManager.RecoveryCooldownUpgradeablesCount++;
    }
}

[Serializable]
public class UpgradeableDuration : UpgradeableFloat
{
    protected override string UpgradeName { get; set; } = "duration_UpKey";

    public override void RegisterInUse()
    {
        abilityManager.DurationUpgradeablesCount++;
    }
}

[Serializable]
public class UpgradeableAOE : UpgradeableFloat
{
    protected override string UpgradeName { get; set; } = "aoe_UpKey";

    public override void RegisterInUse()
    {
        abilityManager.AOEUpgradeablesCount++;
    }
}

[Serializable]
public class UpgradeableKnockback : UpgradeableFloat
{
    protected override string UpgradeName { get; set; } = "knockback_UpKey";

    public override void RegisterInUse()
    {
        abilityManager.KnockbackUpgradeablesCount++;
    }
}

[Serializable]
public class UpgradeableProjectileSpeed : UpgradeableFloat
{
    protected override string UpgradeName { get; set; } = "projectilespeed_UpKey";

    public override void RegisterInUse()
    {
        abilityManager.ProjectileSpeedUpgradeablesCount++;
    }
}

[Serializable]
public class UpgradeableRecoveryChance : UpgradeableFloat
{
    protected override string UpgradeName { get; set; } = "recoverychance_UpKey";

    public override void RegisterInUse()
    {
        abilityManager.RecoveryChanceUpgradeablesCount++;
    }
}

[Serializable]
public class UpgradeableBleedDamage : UpgradeableFloat
{
    protected override string UpgradeName { get; set; } = "bleedingattackpower_UpKey";

    public override void RegisterInUse()
    {
        abilityManager.BleedDamageUpgradeablesCount++;
    }
}

[Serializable]
public class UpgradeableBleedRate : UpgradeableFloat
{
    protected override string UpgradeName { get; set; } = "bleedingrate_UpKey";

    public override void RegisterInUse()
    {
        abilityManager.BleedRateUpgradeablesCount++;
    }
}

[Serializable]
public class UpgradeableBleedDuration : UpgradeableFloat
{
    protected override string UpgradeName { get; set; } = "bleedingduration_UpKey";

    public override void RegisterInUse()
    {
        abilityManager.BleedDurationUpgradeablesCount++;
    }
}

[Serializable]
public class UpgradeableMovementSpeed : UpgradeableFloat
{
    protected override string UpgradeName { get; set; } = "movementspeed_UpKey";

    public override void RegisterInUse()
    {
        abilityManager.MovementSpeedUpgradeablesCount++;
    }
}

[Serializable]
public class UpgradeableRotationSpeed : UpgradeableFloat
{
    protected override string UpgradeName { get; set; } = "rotationspeed_UpKey";

    public override void RegisterInUse()
    {
        abilityManager.RotationSpeedUpgradeablesCount++;
    }
}
///////////////////////////////////////////////////////////////////
/// Улучшаемые int
///////////////////////////////////////////////////////////////////
public abstract class UpgradeableInt : UpgradeableValue<int>
{
    public override void Upgrade(int upgrade)
    {
        value += upgrade;
        OnChanged?.Invoke();
    }

    public override string GetUpgradeDescription()
    {
        if (level >= upgrades.Length || upgrades[level] == 0) return "";
        return DescriptionUtils.GetUpgradeDescription(LocalizationManager.GetTranslate(UpgradeName), upgrades[level]);
    }
}

[Serializable]
public class UpgradeableProjectileCount : UpgradeableInt
{
    [SerializeField] protected int projectilesPer = 1;
    public override int Value => projectilesPer * value;
    [SerializeField] protected override string UpgradeName { get; set; } = "projectilecount_UpKey";
    public override void RegisterInUse() => abilityManager.ProjectileCountUpgradeablesCount++;
    public override string GetUpgradeDescription()
    {
        if (level >= upgrades.Length || upgrades[level] == 0) return "";
        return DescriptionUtils.GetUpgradeDescription(LocalizationManager.GetTranslate(UpgradeName), upgrades[level] * projectilesPer);
    }
}

[Serializable]
public class UpgradeableRecovery : UpgradeableInt
{
    protected override string UpgradeName { get; set; } = "recovery_UpKey";
    public override void RegisterInUse() => abilityManager.RecoveryUpgradeablesCount++;
}

[SerializeField]
public class UpgradeableArmor : UpgradeableInt
{
    protected override string UpgradeName { get; set; } = "armor_UpKey";
    public override void RegisterInUse() => abilityManager.ArmorUpgradeablesCount++;
}