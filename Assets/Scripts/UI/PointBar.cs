using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PointBar : MonoBehaviour
{
    [SerializeField] protected RectTransform barBackground;
    [SerializeField] protected RectTransform barFill;
    [SerializeField] [CanBeNull] protected TextMeshProUGUI text;

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
        UpdateDisplay();
    }

    public void AddPoints(float points)
    {
        currentPoints += points;
        if (currentPoints >= maxPoints) currentPoints = maxPoints;
        
        text.text = $"{currentPoints} / {maxPoints}";
        
        CheckPoints();
        UpdateDisplay();
    }

    public void SubstractPoints(float points)
    {
        currentPoints -= points;
        text.text = $"{currentPoints} / {maxPoints}";
        CheckPoints();
        UpdateDisplay();
    }

    public void SetPoints(float points)
    {
        currentPoints = points;
        text.text = $"{currentPoints} / {maxPoints}";
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