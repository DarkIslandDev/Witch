using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum Rarity
{
    Common = 50,
    Uncommon = 25,
    Rare = 15,
    Epic = 9,
    Legendary = 1
}

public abstract class Ability : MonoBehaviour
{
    [Header("Детали Абилки")] [SerializeField]
    protected Sprite abilityImage;

    [SerializeField] protected new string abilityName;
    [TextArea(5, 5)] [SerializeField] protected string abilityDescription;
    [SerializeField] private Rarity rarity = Rarity.Common;
    [SerializeField] private AbilitySlotUI abilitySlotHUD;
    [SerializeField] private AbilitySlotUI abilitySlotPauseMenuUI;

    protected AbilityManager abilityManager;
    protected EntityManager entityManager;
    protected Player player;
    public List<IUpgradeableValue> upgradeableValues;

    protected int level = 0;
    protected int maxLevel = 0;
    protected bool owned = false;

    public int Level => level;
    public bool Owned => owned;
    public bool IsWeapon;
    public Sprite AbilityImage => abilityImage;
    public string AbilityName => abilityName;
    public float DropWeight => (float)rarity;

    public virtual string Description
    {
        get
        {
            if (!owned)
            {
                return LocalizationManager.GetTranslate(abilityDescription);
            }
            else
            {
                return GetUpgradeDescriptions();
            }
        }
    }

    public virtual void Init(AbilityManager abilityManager, EntityManager entityManager, Player player)
    {
        this.abilityManager = abilityManager;
        this.entityManager = entityManager;
        this.player = player;
        // Регистрация всех обновляемых полей, прикрепленных к этому объекту
        upgradeableValues = GetType()
            .GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
            .Where(fi => typeof(IUpgradeableValue).IsAssignableFrom(fi.FieldType))
            .Select(fi => fi.GetValue(this) as IUpgradeableValue)
            .ToList();
        upgradeableValues.ForEach(x => this.abilityManager.RegisterUpgradeableValue(x));

        if (upgradeableValues.Count > 0)
            maxLevel = upgradeableValues.Max(x => x.UpgradeCount) +
                       1; //  максимальный уровень = общее количество улучшений + 1 для начального уровня.
    }

    public virtual void Select()
    {
        if (!owned)
        {
            owned = true;
            level++;
            InstantiateAbilityIconOnHUD();
            Use();
        }
        else
        {
            level++;
            Upgrade();
            abilitySlotPauseMenuUI.UpdateTextLevel(level.ToString());
        }

    }

    private void InstantiateAbilityIconOnHUD()
    {
        GameObject goHUD = Instantiate(abilityManager.iconSlotHUDPrefab, IsWeapon ? abilityManager.weaponIconsHUDParent 
            : abilityManager.equipmentIconsHUDParent);
        abilitySlotHUD = goHUD.GetComponentInChildren<AbilitySlotUI>();
        abilitySlotHUD.Setup(this);
        
        GameObject goPM = Instantiate(abilityManager.iconSlotPausePanelPrefab,
            IsWeapon ? abilityManager.weaponIconsPausePanelParent : abilityManager.equipmentIconsPausePanelParent);
        abilitySlotPauseMenuUI = goPM.GetComponentInChildren<AbilitySlotUI>();
        abilitySlotPauseMenuUI.Setup(this);
    }

    protected virtual void Use() => upgradeableValues.ForEach(x => x.RegisterInUse());

    protected virtual void Upgrade() => upgradeableValues.ForEach(x => x.Upgrade());

    public virtual bool RequirementsMet() => level < maxLevel;

    protected string GetUpgradeDescriptions()
    {
        string description = "";
        upgradeableValues.ForEach(x => description += x.GetUpgradeDescription());
        return LocalizationManager.GetTranslate(description);
    }
}