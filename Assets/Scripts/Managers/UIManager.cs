using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Pause")] 
    [SerializeField] private LocalizedText pauseText;
    [SerializeField] private LocalizedText continueOnPauseText;
    [SerializeField] private LocalizedText exitOnPauseText;

    private void Start()
    {
        LocalizeText();
        LocalizationManager.OnLanguageChange += OnLanguageChanged;
    }

    private void OnDestroy()
    {
        LocalizationManager.OnLanguageChange -= OnLanguageChanged;
    }

    private void OnLanguageChanged()
    {
        LocalizeText();
    }

    public void LocalizeText()
    {
        pauseText.Localize("pause_Key");
        exitOnPauseText.Localize("exit_Key");
        continueOnPauseText.Localize("continue_Key");
    }
}