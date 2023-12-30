using UnityEngine;

[CreateAssetMenu(menuName = "Blueprints/Character", fileName = "CharacterBlueprint", order = 0)]
public class CharacterBlueprint : ScriptableObject
{
    public new string name;
    public Sprite characterSprite;
    public bool owned = false;
    public int cost;
    public float hp;
    public float atk;
    public float recovery;
    public int armor;
    public float moveSpeed;
    public float atkSpeed;
    public float luck;
    public float acceleration;
    public GameObject playerGFXPrefab;
    public GameObject[] startingAbilities;
    
    public float LevelToExpIncrease(int level)
    {
        return level switch
        {
            < 10 => 10,
            < 20 => 13,
            < 30 => 16,
            _ => 20
        };
    }
}