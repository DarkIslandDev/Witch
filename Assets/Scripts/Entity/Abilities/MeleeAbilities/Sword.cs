using UnityEngine;

public class Sword : MonoBehaviour
{
    public SpriteRenderer weaponSpriteRenderer;
    public Vector2 weaponSize;

    public void Init()
    {
        weaponSpriteRenderer = GetComponent<SpriteRenderer>();
        weaponSize = weaponSpriteRenderer.bounds.size;
    }
}