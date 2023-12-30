using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    #region Instance

    public static MapManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    #endregion

    public Tilemap floorLayout;
}