using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Instance

    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
        playerGO = Instantiate(playerPrefab);
        playerComponents = playerGO.GetComponent<PlayerComponents>();
    }

    #endregion

    public GameObject playerPrefab;
    public GameObject playerGO;
    private PlayerComponents playerComponents;
    public PlayerCamera playerCamera;

    public Transform itemsParentTransform;
    public List<GameObject> items;

    public TextMeshProUGUI gameTimer;
    

    private void Update()
    {
        gameTimer.text = Time.time.ToString("00:00");
    }
}