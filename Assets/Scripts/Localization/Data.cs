// using System.Collections.Generic;
// using System.IO;
// using UnityEngine;
// using UnityEngine.Events;
//
// public static class Data
// {
//     public static string CURRENT_LANGUAGE = "ru";
//
//     private static Dictionary<string, Dictionary<string, string>> _LOCALIZATION;
//
//     public static Dictionary<string, Dictionary<string, string>> LOCALIZATION
//     {
//         get
//         {
//             if(_LANGUAGES == null) LoadLocalizationData();
//             return _LOCALIZATION;
//         }
//     }
//
//     private static string[] _LANGUAGES;
//
//     public static string[] LANGUAGES
//     {
//         get
//         {
//             if (_LANGUAGES == null) LoadLocalizationData();
//             return _LANGUAGES;
//         }
//     }
//     
//     private static UnityEvent _OnLanguageChanged;
//
//     public static UnityEvent OnLanguageChanged => _OnLanguageChanged ??= new UnityEvent();
//
//     private static string ReadFromFile(string path)
//     {
//         if (!File.Exists(path))
//         {
//             Debug.LogWarning("File not found!");
//         }
//         else
//         {
//             using StreamReader reader = new(path);
//             string json = reader.ReadToEnd();
//             return json;
//         }
//         
//         return "Ok!";
//     }
//     
//     public static void LoadLocalizationData()
//     {
//         _LOCALIZATION = new Dictionary<string, Dictionary<string, string>>();
//         string json = ReadFromFile(Path.Combine(Application.dataPath, "Scripts", "Localization", "Localization_data.json"));
//
//         LocalizationData d = JsonUtility.FromJson<LocalizationData>(json);
//         _LANGUAGES = d.languages;
//
//         foreach (LocalizationMapping map in d.table)
//         {
//             _LOCALIZATION[map.key] = new Dictionary<string, string>();
//             foreach (LocalizationValue val in map.values)
//             {
//                 _LOCALIZATION[map.key].Add(val.lang, val.value);
//             }
//         }
//     }
//     
//     public static string GetTranslate(string key) => _LOCALIZATION.TryGetValue(key, out Dictionary<string, string> value) ? value[CURRENT_LANGUAGE] : key;
// }