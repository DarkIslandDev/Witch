using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Dropdown))]
public class LocalizedDropdown : MonoBehaviour
{
    private TMP_Dropdown dropdown;
    private List<string> keys;

    private void Start()
    {
        Localize();
        LocalizationManager.OnLanguageChange += OnLanguageChanged;
    }

    private void OnDestroy() => LocalizationManager.OnLanguageChange -= OnLanguageChanged;

    private void OnLanguageChanged() => Localize();

    private void Init()
    {
        dropdown = GetComponent<TMP_Dropdown>();

        keys = new List<string>();

        foreach (TMP_Dropdown.OptionData option in dropdown.options)
        {
            keys.Add(option.text);
            dropdown.value = LocalizationManager.SelectedLanguage;
            option.text = LocalizationManager.SelectedLanguage.ToString();
            // dropdown.options[0].text = LocalizationManager.SelectedLanguage.ToString();
        }
    }

    public void Localize(List<string> newKeys = null)
    {
        if(dropdown == null) Init();

        if (newKeys != null) keys = newKeys;

        List<TMP_Dropdown.OptionData> options = keys.Select(key => new TMP_Dropdown.OptionData(LocalizationManager.GetTranslate(key))).ToList();
        
        dropdown.options = options;
        dropdown.RefreshShownValue();
    }
}