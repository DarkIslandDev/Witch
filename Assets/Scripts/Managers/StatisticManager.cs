using TMPro;
using UnityEngine;

public class StatisticManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI monstersKilledText;
    [SerializeField] private TextMeshProUGUI coinsGainedText;

    [SerializeField] private int monstersKilled = 0;
    [SerializeField] private float damageDealt = 0;
    [SerializeField] private float damageTaken = 0;
    [SerializeField] private int coinsGained = 0;
    
    public int MonstersKilled => monstersKilled; 
    public float DamageDealt => damageDealt;
    public float DamageTaken => damageTaken;
    public int CoinsGained => coinsGained;

    public void IncrementMonstersKilled()
    {
        monstersKilled++;
        monstersKilledText.text = monstersKilled.ToString();   
    }
    
    public void IncrementCoinsGained(int amount)
    {
        coinsGained++;
        coinsGainedText.text = coinsGained.ToString();   
    }

    public void IncreaseDamageDealt(float damage) => damageDealt += damage;
    
    public void IncreaseDamageTaken(float damage) => damageTaken += damage;
}