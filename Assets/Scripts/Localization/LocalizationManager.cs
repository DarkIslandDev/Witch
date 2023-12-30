using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using TextAsset = UnityEngine.TextAsset;

public class LocalizationManager : MonoBehaviour
{
    private static LocalizationManager instace;
    
    public static int SelectedLanguage { get; private set; }

    public static event LanguageChangeHandler OnLanguageChange;
    public delegate void LanguageChangeHandler();
    
    private static Dictionary<string, List<string>> localization;

    [SerializeField] private TextAsset textFile;

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

        if (localization == null)
        {
            LoadLocalization();
        }
        
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
        xmlDocument.LoadXml(textFile.text);

        foreach (XmlNode key in xmlDocument["Keys"].ChildNodes)
        {
            string keyStr = key.Attributes["Name"].Value;

            List<string> values = new List<string>();
            foreach (XmlNode translate in key["Translate"].ChildNodes)
            {
                values.Add(translate.InnerText);
            }

            localization[keyStr] = values;
        }
    }
    
    public static string GetTranslate(string key, int languageId = -1)
    {
        if (languageId == -1) languageId = SelectedLanguage;

        if (localization.TryGetValue(key, out var value)) return value[languageId];
        
        return key;
    }
}