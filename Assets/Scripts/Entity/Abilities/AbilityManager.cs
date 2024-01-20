using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AbilityManager : MonoBehaviour
{
    private Player player;
    private LevelBlueprint levelBlueprint;
    private WeightedAbilities newAbilities;
    private WeightedAbilities ownedAbilities;
    public List<Ability> ownedWeaponAbilities;
    public List<Ability> ownedEquipmentAbilities;
    private FastList<IUpgradeableValue> registeredUpgradeableValues;
    
    public int DamageUpgradeablesCount { get; set; } = 0;
    public int KnockbackUpgradeablesCount { get; set; } = 0;
    public int WeaponCooldownUpgradeablesCount { get; set; } = 0;
    public int RecoveryCooldownUpgradeablesCount { get; set; } = 0;
    public int AOEUpgradeablesCount { get; set; } = 0;
    public int ProjectileSpeedUpgradeablesCount { get; set; } = 0;
    public int ProjectileCountUpgradeablesCount { get; set; } = 0;
    public int RecoveryUpgradeablesCount { get; set; } = 0;
    public int RecoveryChanceUpgradeablesCount { get; set; } = 0;
    public int BleedDamageUpgradeablesCount { get; set; } = 0;
    public int BleedRateUpgradeablesCount { get; set; } = 0;
    public int BleedDurationUpgradeablesCount { get; set; } = 0;
    public int HealthUpgradeablesCount { get; set; } = 0;
    public int MovementSpeedUpgradeablesCount { get; set; } = 0;
    public int ArmorUpgradeablesCount { get; set; } = 0;
    public int FireRateUpgradeablesCount { get; set; } = 0;
    public int DurationUpgradeablesCount { get; set; } = 0;
    public int RotationSpeedUpgradeablesCount { get; set; } = 0;

    public GameObject iconSlotHUDPrefab;
    public Transform weaponIconsHUDParent;
    public Transform equipmentIconsHUDParent;
    
    public GameObject iconSlotPausePanelPrefab;
    public Transform weaponIconsPausePanelParent;
    public Transform equipmentIconsPausePanelParent;

    public void Init(LevelBlueprint levelBlueprint, EntityManager entityManager, Player player, AbilityManager abilityManager)
    {
        this.levelBlueprint = levelBlueprint;
        this.player = player;

        registeredUpgradeableValues = new FastList<IUpgradeableValue>();

        ownedAbilities = new WeightedAbilities();
        foreach (Ability ability in player.PlayerBlueprint.startingAbilities.Select(abilityPrefab =>
                     Instantiate(abilityPrefab, transform).GetComponent<Ability>()))
        {
            ability.Init(abilityManager, entityManager, player);
            ability.Select();
            ownedAbilities.Add(ability);
        }

        newAbilities = new WeightedAbilities();
        foreach (Ability ability in levelBlueprint.abilityPrefabs.SelectMany(abilityPrefab =>
                     from ownedAbility in player.PlayerBlueprint.startingAbilities
                     where abilityPrefab !=
                           ownedAbility
                     select Instantiate(abilityPrefab, transform).GetComponent<Ability>()))
        {
            ability.Init(abilityManager, entityManager, player);
            newAbilities.Add(ability);
        }
    }

    public void RegisterUpgradeableValue(IUpgradeableValue upgradeableValue, bool inUse = false)
    {
        upgradeableValue.Register(this);
        registeredUpgradeableValues.Add(upgradeableValue);
        if (inUse) upgradeableValue.RegisterInUse();
    }

    public void UpgradeValue<T, TValue>(TValue value) where T : IUpgradeableValue
    {
        UpgradeableValue<TValue>[] upgradeableValues =
            registeredUpgradeableValues.OfType<T>().ToArray() as UpgradeableValue<TValue>[];

        foreach (UpgradeableValue<TValue> upgradeableValue in upgradeableValues!)
        {
            upgradeableValue.Upgrade(value);
        }
    }

    /// <summary>
    /// Выбор Абилки
    /// </summary>
    public List<Ability> SelectAbilities()
    {
        List<Ability> selectedAbilities = new List<Ability>();

        // Определить какие способности доступны в данный момент (соответствуют ли их требования)
        WeightedAbilities availableOwnedAbilities = ExtractAvailableAbilities(ownedAbilities);
        WeightedAbilities availableNewAbilities = ExtractAvailableAbilities(newAbilities);

        // Определить сколько абилок будет выбрано в целом (3 - 4)
        int selectedAbilitiesCount = 3 + (ResolveChance(FourthChance) ? 1 : 0);

        // Попытка показать игроку до двух предметов, которые у него уже есть (чтобы он мог их улучшить)
        int ownedAbilitiesCount = availableOwnedAbilities.Count < 2 ? availableOwnedAbilities.Count : 2;

        for (int i = 0; i < ownedAbilitiesCount; i++)
        {
            if (ResolveChance(OwnedChance)) selectedAbilities.Add(PullAbility(availableOwnedAbilities));
        }

        //  Удаляем способности из пула доступных если собственные способности достигли максимума
        if (ownedWeaponAbilities.Count == 6)
        {
            availableNewAbilities?.Remove(availableNewAbilities.Find(x => x.IsWeapon));
        }
        
        if (ownedEquipmentAbilities.Count == 6)
        {
            availableNewAbilities?.Remove(availableNewAbilities.Find(x => !x.IsWeapon));
        }
        
        // Выбираем оставшиеся способности из пула доступных способностей
        int availableAbilitiesCount = selectedAbilitiesCount - selectedAbilities.Count;

        if (availableAbilitiesCount > availableNewAbilities!.Count)
        {
            availableAbilitiesCount = availableNewAbilities.Count;
        }
        
        for (int i = 0; i < availableAbilitiesCount; i++)
        {
            selectedAbilities.Add(PullAbility(availableNewAbilities));
        }

        // Если возможно, заполните все оставшиеся незаполненные места собственными способностями
        for (int i = selectedAbilities.Count;
             i < selectedAbilitiesCount && i - selectedAbilities.Count < availableOwnedAbilities.Count;
             i++)
        {
            selectedAbilities.Add(PullAbility(availableOwnedAbilities));
        }

        // Возвращаем все оставшиеся доступные способности, которые не были выбраны, обратно в новый пул способностей
        foreach (Ability ability in availableNewAbilities) newAbilities.Add(ability);
        foreach (Ability ability in availableOwnedAbilities) ownedAbilities.Add(ability);

        return selectedAbilities;
    }

    public void ReturnAbilities(List<Ability> abilities)
    {
        foreach (Ability ability in abilities)
        {
            if (ability.Owned)
            {
                ownedAbilities.Add(ability);
            }
            else
            {
                newAbilities.Add(ability);
            }
        }
    }

    public void DestroyActiveAbilities()
    {
        foreach (Ability ability in ownedAbilities)
        {
            Destroy(ability.gameObject);
        }
    }

    public bool HasAvailableAbilities() => ownedAbilities.Any(ability => ability.RequirementsMet()) ||
                                           newAbilities.Any(ability => ability.RequirementsMet());

    private WeightedAbilities ExtractAvailableAbilities(WeightedAbilities abilities)
    {
        WeightedAbilities availableAbilities = new WeightedAbilities();

        foreach (Ability ability in abilities)
        {
            if (ability.RequirementsMet())
            {
                availableAbilities.Add(ability);
            }
        }

        foreach (Ability ability in availableAbilities)
        {
            abilities.Remove(ability);
        }

        return availableAbilities;
    }

    /// <summary>
    /// Извлекает способность из списка заданных способностей и ее веса.
    /// </summary>
    private Ability PullAbility(WeightedAbilities abilities)
    {
        while (true)
        {
            float rand = Random.Range(0f, abilities.Weight);
            float cumulative = 0;
            foreach (Ability ability in abilities)
            {
                cumulative += ability.DropWeight;
                if (rand < cumulative)
                {
                    abilities.Remove(ability);
                    return ability;
                }
            }

            Debug.LogError("Не удалось вытащить абилку");
            return null;
        }
    }

    /// <summary>
    /// Вероятность появления способности, уже принадлежащей игроку.
    /// (чтобы его можно было обновить)
    /// </summary>
    private float OwnedChance()
    {
        float x = player.CurrentLevel % 2 == 0 ? 2 : 1;
        return 1 + 0.3f * x - 1 / player.Luck;
    }

    /// <summary>
    /// Вероятность появления четвертой способности/возможности улучшения.
    /// </summary>
    private float FourthChance() => 1 - 1 / player.Luck;

    /// <summary>
    /// Отвечает, выпадет ли четвертая абилка
    /// </summary>
    private static bool ResolveChance(Func<float> chanceFunction) => Random.Range(0.0f, 1.0f) < chanceFunction();
}