using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HealthBar : PointBar
{
    [SerializeField] private Slider healthBarSlider;
    [SerializeField] private Slider delayedBarSlider;
    [SerializeField] private float delay = 0.5f;

    private float targetHealthFillAmount;
    private float targetDelayedFillAmount;
    private float currentHealthVelocity;
    private float currentDelayedVelocity;

    protected override void UpdateDisplay()
    {
        base.UpdateDisplay();

        healthBarSlider.value = currentPoints / maxPoints;

        targetHealthFillAmount = healthBarSlider.value;

        delayedBarSlider.value =
            Mathf.SmoothDamp(delayedBarSlider.value, targetHealthFillAmount, ref currentDelayedVelocity, delay);
    }
}