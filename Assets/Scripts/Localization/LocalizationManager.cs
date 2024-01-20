using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

public class LocalizationManager : MonoBehaviour
{
    private static LocalizationManager instace;

    public static event LanguageChangeHandler OnLanguageChange;
    public delegate void LanguageChangeHandler();

    public static int SelectedLanguage { get; private set; }

    private static Dictionary<string, List<string>> localization;

    [SerializeField] private TextAsset textAsset;
    
    private void Awake()
    {
        if (instace == null)
        {
            instace = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(this);

        if (localization == null) LoadLocalization();
        
        SelectedLanguage = PlayerPrefs.GetInt("LanguageID");
    }

    public static void SetLanguage(int id)
    {
        SelectedLanguage = id;
        PlayerPrefs.SetInt("LanguageID", id);
        OnLanguageChange?.Invoke();
    }

    private void LoadLocalization()
    {
        localization = new Dictionary<string, List<string>>();

        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(textAsset.text);

        foreach (XmlNode key in xmlDocument["Keys"]!.ChildNodes)
        {
            string keyStr = key.Attributes["Name"].Value;

            List<string> values = (from XmlNode translate in key["Translate"]!.ChildNodes select translate.InnerText).ToList();

            localization[keyStr] = values;
        }
    }

    public static string GetTranslate(string key = null, int languageId = -1)
    {
        if (languageId == -1) languageId = SelectedLanguage;

        key = key!.ToLower();
        
        return localization.TryGetValue(key!, out List<string> value) ? value[languageId] : key;
    }
}

