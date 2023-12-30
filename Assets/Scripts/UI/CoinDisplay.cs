using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class CoinDisplay : MonoBehaviour
{
    private TextMeshProUGUI coinText;

    private void Start()
    {
        coinText = GetComponent<TextMeshProUGUI>();
        coinText.text = PlayerPrefs.GetInt("Coins").ToString();
    }

    public void UpdateDisplay()
    {
        coinText.text = PlayerPrefs.GetInt("Coins") == 0 ? string.Empty : PlayerPrefs.GetInt("Coins").ToString();
    }
}