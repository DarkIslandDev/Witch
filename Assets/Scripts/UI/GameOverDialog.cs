using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private GameObject reviveButton;

    public void Open(bool levelPassed, bool canRevive, StatisticManager statisticManager)
    {
        statusText.Localize(levelPassed ? "win_Key" : "lose_Key");
        retryText.Localize("retry_key");
        exitText.Localize("exit_key");
        
        coinsGained.text = $"{statisticManager.CoinsGained}";
        enemiesRouted.text = statisticManager.MonstersKilled.ToString();
        damageDealt.text = statisticManager.DamageDealt.ToString();
        damageTaken.text = statisticManager.DamageTaken.ToString();
        
        background.SetActive(true);
        
        if(canRevive) reviveButton.SetActive(true);
        
        base.Open();
    }

    public override void Close()
    {
        base.Close();
        background.SetActive(false);
    }
}