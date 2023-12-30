using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityCard : MonoBehaviour
{
    [SerializeField] private Image abilityImage;
    [SerializeField] private LocalizedText nameText;
    [SerializeField] private LocalizedText descriptionText;
    [SerializeField] private LocalizedText levelText;
    [SerializeField] private float appearSpeed = 3;
    private AbilityManager abilityManager;
    private AbilitySelectionDialog levelUpMenu;
    private Ability ability;

    public void Init(AbilitySelectionDialog levelUpMenu, AbilityManager abilityManager, Ability ability, float waitToAppear)
    {
        this.abilityManager = abilityManager;
        this.levelUpMenu = levelUpMenu;
        this.ability = ability;
        
        abilityImage.sprite = ability.AbilityImage;
        nameText.Localize(ability.AbilityName);
        descriptionText.Localize(ability.Description);
        
        if (!ability.Owned)
        {
            levelText.Localize("new_Key");
        }
        else
        {
            levelText.Localize("", $"{ability.Level} -> {(ability.Level + 1)}");
        }
        
        StartCoroutine(Appear(waitToAppear));
    }

    private IEnumerator Appear(float waitToAppear)
    {
        Vector3 initialScale = transform.localScale;
        transform.localScale = Vector3.zero;

        yield return new WaitForSecondsRealtime(waitToAppear);

        float t = 0;

        while (t < 1)
        {
            transform.localScale = Vector3.LerpUnclamped(Vector3.zero, new Vector3(1,1,1), EasingUtils.EaseOutBack(t));
            t += Time.unscaledDeltaTime * appearSpeed;
            yield return null;
        }
        
        transform.localScale = new Vector3(1,1,1);
    }
    
    public void Selected()
    {
        ability.Select();
        levelUpMenu.Close();
    }
}