using TMPro;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [SerializeField] private LocalizedText hpText;
    [SerializeField] private LocalizedText armorText;
    [SerializeField] private LocalizedText atkText;
    [SerializeField] private LocalizedText mvspdText;

    private CharacterBlueprint characterBlueprint;

    public void Init(CharacterBlueprint characterBlueprint)
    {
        this.characterBlueprint = characterBlueprint;

        hpText.Localize("hp_Key", characterBlueprint.hp.ToString());
        armorText.Localize("armor_Key", characterBlueprint.armor.ToString());
        atkText.Localize("atk_Key", characterBlueprint.atk.ToString());
        mvspdText.Localize("mvspd_Key", characterBlueprint.moveSpeed.ToString());
    }
}