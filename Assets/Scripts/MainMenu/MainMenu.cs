using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private CharacterSelector characterSelector;

    [SerializeField] private LocalizedText startText;
    [SerializeField] private LocalizedText exitText;
    [SerializeField] private LocalizedText changeLanguageText;
    [SerializeField] private LocalizedText characterSelectionText;
    [SerializeField] private LocalizedText[] settingsTexts;
    [SerializeField] private LocalizedText[] backTexts;

    private void Start()
    {
        characterSelector.Init();
        
        OnLanguageChange();
        LocalizationManager.OnLanguageChange += OnLanguageChange;
    }

    private void OnDestroy() => LocalizationManager.OnLanguageChange -= OnLanguageChange;

    private void OnLanguageChange()
    {
        startText.Localize("start_key");
        exitText.Localize("exit_key");
        changeLanguageText.Localize("change_language_key");
        characterSelectionText.Localize("character_selection_key");

        foreach (LocalizedText settingsText in settingsTexts) settingsText.Localize("settings_key");
        
        foreach (LocalizedText backText in backTexts) backText.Localize("back_key");
    }
}