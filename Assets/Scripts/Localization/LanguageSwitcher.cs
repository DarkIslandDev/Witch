// using System.Collections.Generic;
// using TMPro;
// using UnityEngine;
//
// [RequireComponent(typeof(TMP_Dropdown))]
// public class LanguageSwitcher : MonoBehaviour
// {
//     private TMP_Dropdown dropdown;
//
//     private void Awake()
//     {
//         dropdown = GetComponent<TMP_Dropdown>();
//
//         ChangeLanguage();
//         Data.OnLanguageChanged?.AddListener(ChangeLanguage);
//     }
//
//     public void ChangeLanguage()
//     {
//         if(dropdown == null) dropdown = GetComponent<TMP_Dropdown>();
//
//         dropdown.options = new List<TMP_Dropdown.OptionData>();
//         
//         foreach (string language in Data.LANGUAGES)
//         {
//             dropdown.options.Add(new TMP_Dropdown.OptionData(language));
//         }
//         
//         dropdown.onValueChanged.AddListener((int i) =>
//         {
//             string language = Data.LANGUAGES[i];
//             Data.CURRENT_LANGUAGE = language;
//             Data.OnLanguageChanged.Invoke();
//         });
//     }
// }