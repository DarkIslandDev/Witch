using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TitleBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private float timeToChangeColor = 5f;
    [SerializeField] private float timer;

    private void Awake()
    {
        titleText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        
        if(timer <= 0)
        {
            SetRandomColor();
            timer = timeToChangeColor;
        }
    }

    private void SetRandomColor()
    {
        Color randomColor = new Color(Random.value, Random.value, Random.value);

        titleText.color = randomColor;
    }
}