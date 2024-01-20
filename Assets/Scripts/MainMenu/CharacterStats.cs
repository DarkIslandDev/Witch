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

        hpText.Localize("hp_key", characterBlueprint.hp.ToString());
        armorText.Localize("armor_key", characterBlueprint.armor.ToString());
        atkText.Localize("atk_key", characterBlueprint.atk.ToString());
        mvspdText.Localize("mvspd_key", characterBlueprint.moveSpeed.ToString());
    }
}