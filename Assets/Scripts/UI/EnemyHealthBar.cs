using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private Transform healthBar;

    public void SetHealthValue(int max, int current)
    {
        float state = (float)current;
        state /= max;

        if (state < 0)
        {
            state = 0;
        }
        
        healthBar.localScale = new Vector3(state, 1, 1);
    }
}