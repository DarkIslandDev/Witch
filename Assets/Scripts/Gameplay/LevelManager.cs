﻿using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum GameState
{
    Pause,
    Game,
    LevelUp,
    GameOver
}

public class LevelManager : MonoBehaviour
{
    public GameState gameState;
    
    [SerializeField] private LevelBlueprint levelBlueprint;
    [SerializeField] private Player player;
    [SerializeField] private EntityManager entityManager;
    [SerializeField] private AbilityManager abilityManager;
    [SerializeField] private AbilitySelectionDialog abilitySelectionDialog;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private StatisticManager statisticManager;
    [SerializeField] private GameOverDialog gameOverDialog;
    [SerializeField] private GameTimer gameTimer;
    [SerializeField] private PauseMenu pauseMenu;

    private float levelTime = 0;
    private float timeSinceLastMonsterSpawned;
    private float timeSinceLastChestSpawned;
    private bool miniBossSpawned = false;
    private bool finalBossSpawned = false;
    
    private void Awake()
    {
        Init(levelBlueprint);
    }
    
    public void Init(LevelBlueprint levelBlueprint)
    {
        this.levelBlueprint = levelBlueprint;
        levelTime = 0;
        
        entityManager.Init(this.levelBlueprint, player, playerInventory, statisticManager, abilitySelectionDialog);

        abilityManager.Init(this.levelBlueprint, entityManager, player, abilityManager);
        abilitySelectionDialog.Init(this, abilityManager, entityManager, player, pauseMenu);
        
        GameObject playerGo = Instantiate(player.PlayerBlueprint.playerGFXPrefab, player.transform);
        player.spriteRenderer = playerGo.GetComponent<SpriteRenderer>();
        player.Init(entityManager, abilityManager, statisticManager);
        player.OnDeath.AddListener(GameOver);
        
        entityManager.SpawnGemsAroundPlayer(this.levelBlueprint.initialGemCount, this.levelBlueprint.initialGemType);
        entityManager.SpawnChest(levelBlueprint.chestBlueprint);

        gameState = GameState.Game;
    }

    

    private void Update()
    {
        levelTime -= Time.deltaTime;
        gameTimer.SetTime(levelTime);

        if (levelTime < levelBlueprint.levelTime)
        {
            timeSinceLastMonsterSpawned += Time.deltaTime;
            float spawnRate = levelBlueprint.monsterSpawnTable.GetSpawnRate(levelTime / levelBlueprint.levelTime);
            float monsterSpawnDelay = spawnRate > 0 ? 1.0f / spawnRate : float.PositiveInfinity;

            if (timeSinceLastMonsterSpawned >= monsterSpawnDelay)
            {
                (int monsterIndex, float hpMultiplier) =
                    levelBlueprint.monsterSpawnTable.SelectMonsterWithHPMultiplier(levelTime /
                        levelBlueprint.levelTime);
                (int poolIndex, int blueprintIndex) = levelBlueprint.MonsterIndexMap[monsterIndex];
                MonsterBlueprint monsterBlueprint = levelBlueprint.monsters[poolIndex].monsterBlueprints[blueprintIndex];
                entityManager.SpawnMonsterRandomPosition(poolIndex, monsterBlueprint, monsterBlueprint.hp * hpMultiplier);
                timeSinceLastMonsterSpawned = Mathf.Repeat(timeSinceLastMonsterSpawned, monsterSpawnDelay);
            }
        }
        
        timeSinceLastChestSpawned += Time.deltaTime;
        if (timeSinceLastChestSpawned >= levelBlueprint.chestSpawnDelay)
        {
            for (int i = 0; i < levelBlueprint.chestSpawnAmount; i++)
            {
                entityManager.SpawnChest(levelBlueprint.chestBlueprint);
            }
            timeSinceLastChestSpawned = Mathf.Repeat(timeSinceLastChestSpawned, levelBlueprint.chestSpawnDelay);
        }
        
        // SwitchGameState();
    }

    public void SwitchGameState()
    {
        switch (gameState)
        {
            case GameState.Game:
                pauseMenu.Pause(false, 1);
                break;
            case GameState.Pause:
            case GameState.LevelUp:
            case GameState.GameOver:
                pauseMenu.Pause(true, 0);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private void GameOver()
    {
        gameState = GameState.GameOver;
        SwitchGameState();
        
        int coinCount = PlayerPrefs.GetInt("Coins");
        PlayerPrefs.SetInt("Coins", coinCount + statisticManager.CoinsGained);
        gameOverDialog.Open(false, statisticManager);
    }
    
    public void LevelPassed(Monster finalBossKilled)
    {
        gameState = GameState.GameOver;
        
        int coinCount = PlayerPrefs.GetInt("Coins");
        PlayerPrefs.SetInt("Coins", coinCount + statisticManager.CoinsGained);
        gameOverDialog.Open(true, statisticManager);
    }

    public void Restart()
    {
        gameState = GameState.Game;
        SwitchGameState();
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1;
        
        int coinCount = PlayerPrefs.GetInt("Coins");
        PlayerPrefs.SetInt("Coins", coinCount + statisticManager.CoinsGained);
        SceneManager.LoadScene("MainMenu");
    }
}