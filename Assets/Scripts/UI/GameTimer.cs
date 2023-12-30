using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class GameTimer : MonoBehaviour
{
    private TextMeshProUGUI timerText;

    private void Awake()
    {
        timerText = GetComponent<TextMeshProUGUI>();
    }

    public void SetTime(float t)
    {
        System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(t);
        timerText.text = timeSpan.ToString(@"mm\:ss");
    }
}