using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : PointBar
{
    [SerializeField] private Slider redBarSlider;
    [SerializeField] private Slider delayedBarSlider;
    [SerializeField] private float delay = 0.5f;

    private float targetRedFillAmount;
    private float targetDelayedFillAmount;
    private float currentRedVelocity;
    private float currentDelayedVelocity;

    protected override void UpdateDisplay()
    {
        base.UpdateDisplay();
        
        redBarSlider.value = currentPoints / maxPoints;

        targetRedFillAmount = redBarSlider.value;

        delayedBarSlider.value = Mathf.SmoothDamp(delayedBarSlider.value, targetRedFillAmount, ref currentDelayedVelocity, delay);
    }
}