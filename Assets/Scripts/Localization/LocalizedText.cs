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
        Init();
        LocalizationManager.OnLanguageChange += OnLanguageChange;
    }

    // private void OnDestroy() => LocalizationManager.OnLanguageChange -= OnLanguageChange;

    private void Init()
    {
        text = GetComponent<TextMeshProUGUI>();
        key = text.text;
    }
    
    private void OnLanguageChange()
    {
        Localize();
    }

    public void Localize(string newKey = null, [CanBeNull] string additional = null)
    {
        Init();

        if (newKey != null) key = newKey;

        text.text = $"{LocalizationManager.GetTranslate(key)} {additional}";
    }
}
