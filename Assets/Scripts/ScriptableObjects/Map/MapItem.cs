using UnityEngine;

[CreateAssetMenu(menuName = "MapItem/Map item", fileName = "Map Item", order = 0)]
public class MapItem : ScriptableObject
{
    public new string name;
    public Sprite sprite;
    public Vector2Int size;
    public PlacementType placementType;
    public bool addOffset;
    public int itemHealth;
    public bool NonDestructible;
}