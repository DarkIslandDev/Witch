using System;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedText : MonoBehaviour
{
    private TextMeshProUGUI text;
    private string key;

    private void Start()
    {
        LocalizationManager.OnLanguageChange += OnLanguageChanged;
    }

    private void OnDestroy()
    {
        LocalizationManager.OnLanguageChange -= OnLanguageChanged;
    }

    private void OnLanguageChanged()
    {
        Localize();
    }

    private void Init()
    {
        text = GetComponent<TextMeshProUGUI>();
        key = text.text;
    }
    
    public void Localize(string newKey = null, [CanBeNull] string additional = null)
    {
        if(text == null) Init();

        if (newKey != null) key = newKey;

        text.text = $"{LocalizationManager.GetTranslate(key)} {additional}";
    }
}