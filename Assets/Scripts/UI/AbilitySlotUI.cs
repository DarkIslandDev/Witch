using System;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySlotUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField][CanBeNull] private TextMeshProUGUI level;
    protected bool isEmpty;

    public bool IsEmpty => isEmpty;

    public void Setup(Ability ability)
    {
        icon.sprite = ability.AbilityImage;
        if (level != null) level.text = ability.Level > 0 ? ability.Level.ToString() : string.Empty;
        
        
        icon.enabled = true;
    }

    public void UpdateTextLevel(string level) => this.level.text = level;
}