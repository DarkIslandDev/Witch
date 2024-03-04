using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerUI : MonoBehaviour
{
    public PauseMenu pauseMenu; 
    
    [Header("Point bars and text")]
    public ExperienceBar levelBar;
    public TextMeshProUGUI levelText;
    public HealthBar healthBar;
    public TextMeshProUGUI healthText;

    public AbilitySelectionDialog abilitySelectionDialog;
}