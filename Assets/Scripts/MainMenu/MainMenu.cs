using System;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private CharacterSelector characterSelector;

    [SerializeField] private LocalizedText startText;
    [SerializeField] private LocalizedText exitText;
    [SerializeField] private LocalizedText settingsText;
    [SerializeField] private LocalizedText back1Text;
    [SerializeField] private LocalizedText back2Text;
    [SerializeField] private LocalizedText changeLanguageText;

    private void Start()
    {
        characterSelector.Init();
        OnLanguageChanged();
        LocalizationManager.OnLanguageChange += OnLanguageChanged;
    }

    private void OnDestroy()
    {
        LocalizationManager.OnLanguageChange -= OnLanguageChanged;
    }

    private void OnLanguageChanged()
    {
        startText.Localize("start_Key", "");
        exitText.Localize("exit_Key", "");
        settingsText.Localize("settings_Key", "");
        back1Text.Localize("back_Key", "");
        back2Text.Localize("back_Key", "");
        changeLanguageText.Localize("changelanguage_Key", "");
    }
}