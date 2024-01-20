using TMPro;
using UnityEngine;

public class GameOverDialog : DialogBox
{
    [SerializeField] private LocalizedText statusText;
    [SerializeField] private LocalizedText retryText;
    [SerializeField] private LocalizedText exitText;
    [SerializeField] private TextMeshProUGUI coinsGained;
    [SerializeField] private TextMeshProUGUI enemiesRouted;
    [SerializeField] private TextMeshProUGUI damageDealt;
    [SerializeField] private TextMeshProUGUI damageTaken;
    [SerializeField] private GameObject background;

    public void Open(bool levelPassed, StatisticManager statisticManager)
    {
        statusText.Localize(levelPassed ? "win_Key" : "lose_Key");
        retryText.Localize("retry_Key");
        exitText.Localize("exit_Key");
        
        coinsGained.text = $"{statisticManager.CoinsGained}";
        enemiesRouted.text = statisticManager.MonstersKilled.ToString();
        damageDealt.text = statisticManager.DamageDealt.ToString();
        damageTaken.text = statisticManager.DamageTaken.ToString();
        
        background.SetActive(true);
        
        base.Open();
    }

    public override void Close()
    {
        base.Close();
        background.SetActive(false);
    }
}