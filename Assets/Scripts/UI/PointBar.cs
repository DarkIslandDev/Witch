using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PointBar : MonoBehaviour
{
    [SerializeField] protected RectTransform barBackground;
    [SerializeField] protected RectTransform barFill;
    [SerializeField] protected TextMeshProUGUI text;
    [SerializeField] protected bool displayText;

    protected UnityEvent onEmpty;
    protected UnityEvent onFull;

    protected float currentPoints;
    protected float minPoints;
    protected float maxPoints;
    protected bool clamp;
    
    public float CurrentPoints { get => currentPoints; set => currentPoints = value; }

    public void Setup(float currentPoints, float minPoints, float maxPoints, bool clamp = true)
    {
        this.currentPoints = currentPoints;
        this.minPoints = minPoints;
        this.maxPoints = maxPoints;
        this.clamp = clamp;

        if (displayText ==false) text.text = string.Empty;
        
        UpdateDisplay();
    }

    public void AddCurrentPoints(float points)
    {
        currentPoints += points;
        if (currentPoints >= maxPoints) currentPoints = maxPoints;
        
        if (displayText) text.text = $"{currentPoints:N0} / {maxPoints:N0}";
        
        CheckPoints();
        UpdateDisplay();
    }

    public void AddMaxPoints(float points)
    {
        maxPoints += points;

        if (displayText) text.text = $"{currentPoints:N0} / {maxPoints:N0}";

        CheckPoints();
        UpdateDisplay();
    }
    
    public void SubstractPoints(float points)
    {
        currentPoints -= points;
        if (displayText) text.text = $"{currentPoints:N0} / {maxPoints:N0}";
        CheckPoints();
        UpdateDisplay();
    }

    public void SetPoints(float points)
    {
        currentPoints = points;
        if (displayText) text.text = $"{currentPoints:N0} / {maxPoints:N0}";
        CheckPoints();
        UpdateDisplay();
    }
    

    public void UpdateDisplay() => barFill.sizeDelta = new Vector2(barBackground.rect.width * (currentPoints - minPoints)/(maxPoints - minPoints), barFill.sizeDelta.y);

    private void CheckPoints()
    {
        if (currentPoints >= maxPoints)
        {
            onFull?.Invoke();
            if (clamp) currentPoints = maxPoints;
        }
        else if (currentPoints <= minPoints)
        {
            onEmpty?.Invoke();
            if (clamp) currentPoints = minPoints;
        }
    }
}