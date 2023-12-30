using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private LevelBlueprint levelBlueprint;
    [SerializeField] private Player player;
    [SerializeField] private EntityManager entityManager;
    [SerializeField] private AbilityManager abilityManager;
    [SerializeField] private AbilitySelectionDialog abilitySelectionDialog;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private StatisticManager statisticManager;
    [SerializeField] private GameOverDialog gameOverDialog;
    [SerializeField] private GameTimer gameTimer;

    private float levelTime = 0;
    private float timeSinceLastMonsterSpawned;
    private float timeSinceLastChestSpawned;
    private bool miniBossSpawned = false;
    private bool finalBossSpawned = false;

    public void Init(LevelBlueprint levelBlueprint)
    {
        this.levelBlueprint = levelBlueprint;
        levelTime = 0;
        
        entityManager.Init(this.levelBlueprint, player, playerInventory, statisticManager, abilitySelectionDialog);
        
        abilityManager.Init(this.levelBlueprint, entityManager, player, abilityManager);
        abilitySelectionDialog.Init(abilityManager, entityManager, player);
        
        GameObject playerGo = Instantiate(player.PlayerBlueprint.playerGFXPrefab, player.transform.position, quaternion.identity, player.transform);
        player.spriteRenderer = playerGo.GetComponent<SpriteRenderer>();
        player.Init(entityManager, abilityManager, statisticManager);
        player.OnDeath.AddListener(GameOver);

        entityManager.SpawnGemsAroundPlayer(this.levelBlueprint.initialGemCount, this.levelBlueprint.initialGemType);
        entityManager.SpawnChest(levelBlueprint.chestBlueprint);
        
    }

    private void Awake()
    {
        Init(levelBlueprint);
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
    }

    private void GameOver()
    {
        Time.timeScale = 0;
        int coinCount = PlayerPrefs.GetInt("Coins");
        PlayerPrefs.SetInt("Coins", coinCount + statisticManager.CoinsGained);
        gameOverDialog.Open(false, statisticManager);
    }
    
    public void LevelPassed(Monster finalBossKilled)
    {
        Time.timeScale = 0;
        int coinCount = PlayerPrefs.GetInt("Coins");
        PlayerPrefs.SetInt("Coins", coinCount + statisticManager.CoinsGained);
        gameOverDialog.Open(true, statisticManager);
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
}