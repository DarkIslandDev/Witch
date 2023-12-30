using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    [SerializeField] private LocalizedDropdown localizedDropdown;

    public void SwitchLanguage(int id)
    {
        var options = new List<string>()
        {
            "en_Key",
            "ru_Key"
        };
        
        localizedDropdown.Localize(options);

        LocalizationManager.SetLanguage(id);
    }
}