using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerUI : MonoBehaviour
{
    public PauseMenu pauseMenu; 
    
    [Header("Point bars and text")]
    public PointBar levelBar;
    public TextMeshProUGUI levelText;
    public PointBar healthBar;
    public TextMeshProUGUI healthText;

    public AbilitySelectionDialog abilitySelectionDialog;
}