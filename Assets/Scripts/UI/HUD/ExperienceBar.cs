using UnityEngine;
using UnityEngine.UI;

public class ExperienceBar : PointBar
{
    [SerializeField] private Slider experienceBarSlider;
    // [SerializeField] private Slider delayedBarSlider;
    // [SerializeField] private float delay = 0.5f;

    private float targetXPFillAmount;
    private float currentXPVelocity;
    // private float targetDelayedFillAmount;
    // private float currentDelayedVelocity;

    protected override void UpdateDisplay()
    {
        base.UpdateDisplay();

        experienceBarSlider.value = currentPoints / maxPoints;

        targetXPFillAmount = experienceBarSlider.value;

        // delayedBarSlider.value =
        //     Mathf.SmoothDamp(delayedBarSlider.value, targetXPFillAmount, ref currentDelayedVelocity, delay);
    }
}