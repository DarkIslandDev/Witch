using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    [Header("Monster spawning settings")] [SerializeField]
    private float monsterSpawnBufferDistance;

    [SerializeField] private float playerDirectionSpawnWeight;

    [Header("Chest spawning settings")] [SerializeField]
    private float chestSpawnRange = 5f;

    [Header("Object pool settings")] [SerializeField]
    private GameObject monsterPoolParent;

    private MonsterPool[] monsterPools;

    [SerializeField] private GameObject projectilePoolParent;
    private List<ProjectilePool> projectilePools;
    private Dictionary<GameObject, int> projectileIndexByPrefab;

    [SerializeField] private GameObject throwablePoolParent;
    private List<ThrowablePool> throwablePools;
    private Dictionary<GameObject, int> throwableIndexByPrefab;

    [SerializeField] private GameObject boomerangPoolParent;
    private List<BoomerangPool> boomerangPools;
    private Dictionary<GameObject, int> boomerangIndexByPrefab;

    [SerializeField] private CoinPool coinPool;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private ExpGemPool expGemPool;
    [SerializeField] private GameObject expGemPrefab;
    [SerializeField] private ChestPool chestPool;
    [SerializeField] private GameObject chestPrefab;
    [SerializeField] private DamageTextPool textPool;
    [SerializeField] private GameObject textPrefab;

    [Header("Dependencies")] [SerializeField]
    private SpriteRenderer flashSpriteRenderer;

    [SerializeField] private Camera playerCamera;
    [SerializeField] protected LevelManager levelManager;
    protected Player player;
    private StatisticManager statisticManager;
    private PlayerInventory playerInventory;
    private FastList<Monster> livingMonsters;
    private FastList<Collectable> magneticCollectables;
    public FastList<Chest> chests;
    private float timeSinceLastMonsterSpawned;
    private float timeSinceLastChestSpawned;
    private float screenWidthWorldSpace;
    private float screenHeightWorldSpace;
    private float screenDiagonalWorldSpace;
    private float minSpawnDistance;
    private Coroutine flashCoroutine;
    private Coroutine shockwave;

    public FastList<Monster> LivingMonsters => livingMonsters;
    public FastList<Collectable> MagneticCollectables => magneticCollectables;
    public Player Player => player;
    public PlayerInventory PlayerInventory => playerInventory;
    public AbilitySelectionDialog AbilitySelectionDialog { get; private set; }

    public void Init(LevelBlueprint levelBlueprint, Player player, PlayerInventory playerInventory,
        StatisticManager statisticManager, AbilitySelectionDialog abilitySelectionDialog)
    {
        this.player = player;
        this.statisticManager = statisticManager;
        this.playerInventory = playerInventory;
        AbilitySelectionDialog = abilitySelectionDialog;

        // Определить размер экрана в мировом пространстве, чтобы спавнить врагов за его пределами
        if (Camera.main != null) playerCamera = Camera.main;
        Vector2 bottomLeft = playerCamera.ViewportToWorldPoint(new Vector3(0, 0, playerCamera.nearClipPlane));
        Vector2 topRight = playerCamera.ViewportToWorldPoint(new Vector3(1, 1, playerCamera.nearClipPlane));
        screenWidthWorldSpace = topRight.x - bottomLeft.x;
        screenHeightWorldSpace = topRight.y - bottomLeft.y;
        screenDiagonalWorldSpace = (topRight - bottomLeft).magnitude;
        minSpawnDistance = screenDiagonalWorldSpace / 2;

        // Init FastList
        livingMonsters = new FastList<Monster>();
        magneticCollectables = new FastList<Collectable>();
        chests = new FastList<Chest>();

        // Инициализация пула монстров для каждого префаба монстров
        monsterPools = new MonsterPool[levelBlueprint.monsters.Length + 1];
        for (int i = 0; i < levelBlueprint.monsters.Length; i++)
        {
            monsterPools[i] = monsterPoolParent.AddComponent<MonsterPool>();
            monsterPools[i].Init(this, player, levelBlueprint.monsters[i].monsterPrefab);
        }

        monsterPools[monsterPools.Length - 1] = monsterPoolParent.AddComponent<MonsterPool>();
        monsterPools[monsterPools.Length - 1].Init(this, player, levelBlueprint.finalBoss.bossPrefab);

        // Инициализируем пул снарядов для каждого типа снарядов дальнего боя
        projectileIndexByPrefab = new Dictionary<GameObject, int>();
        projectilePools = new List<ProjectilePool>();

        // Инициализируем метательный пул для каждого метательного типа
        throwableIndexByPrefab = new Dictionary<GameObject, int>();
        throwablePools = new List<ThrowablePool>();

        // Инициализируем пул бумерангов для каждого типа бумерангов
        boomerangIndexByPrefab = new Dictionary<GameObject, int>();
        boomerangPools = new List<BoomerangPool>();

        // Инициализируем оставшиеся одноразовые пулы объектов
        expGemPool.Init(this, player, expGemPrefab);
        chestPool.Init(this, player, coinPrefab);
        chestPool.Init(this, player, chestPrefab);
        textPool.Init(this, player, textPrefab);
    }

    public void CollectAllCoinsAndGems()
    {
        foreach (Collectable collectable in magneticCollectables.ToList())
        {
            collectable.Collect();
        }
    }

    public void DamageAllVisibleEnemies(float damage)
    {
        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        // flashCoroutine = StartCoroutine(Flash());
        foreach (Monster monster in livingMonsters.ToList())
        {
            monster.TakeDamage(damage, Vector2.zero);
        }
    }

    public void KillAllMonsters()
    {
        foreach (Monster monster in livingMonsters.ToList().Where(monster => !(monster as BossMonster)))
        {
            StartCoroutine(monster.Killed(false));
        }
    }

    public bool TransformOnScreen(Transform transform, Vector2 buffer = default(Vector2))
    {
        return (
            transform.position.x > player.transform.position.x - screenWidthWorldSpace / 2 - buffer.x &&
            transform.position.x < player.transform.position.x + screenWidthWorldSpace / 2 + buffer.x &&
            transform.position.y > player.transform.position.y - screenHeightWorldSpace / 2 - buffer.y &&
            transform.position.y < player.transform.position.y + screenHeightWorldSpace / 2 + buffer.y
        );
    }

    #region Monster Spawning

    public Monster SpawnMonsterRandomPosition(int monsterPoolIndex, MonsterBlueprint monsterBlueprint, float hpBuff = 0)
    {
        Vector2 spawnPosition = (player.Velocity != Vector2.zero)
            ? GetRandomMonsterSpawnPositionPlayerVelocity()
            : GetRandomMonsterSpawnPosition();

        foreach (Room room in levelManager.DungeonGenerator.Rooms)
        {
            if (spawnPosition.x !>= (room.TileBounds.xMin) && spawnPosition.x !<= (room.TileBounds.xMax) &&
                spawnPosition.y !>= (room.TileBounds.yMin) && spawnPosition.y !<= (room.TileBounds.yMax))
            {
                DespawnMonster(monsterPoolIndex, monsterPools[monsterPoolIndex].Get(), false);
            }
        }
        
        return SpawnMonster(monsterPoolIndex, spawnPosition, monsterBlueprint, hpBuff);
    }

    public Monster SpawnMonster(int monsterPoolIndex, Vector2 position, MonsterBlueprint monsterBlueprint,
        float hpBuff = 0)
    {
        Monster newMonster = monsterPools[monsterPoolIndex].Get();
        newMonster.Setup(monsterPoolIndex, position, monsterBlueprint, hpBuff);
        return newMonster;
    }

    public void DespawnMonster(int monsterPoolIndex, Monster monster, bool killedByPlayer = true)
    {
        if (killedByPlayer) statisticManager.IncrementMonstersKilled();

        monsterPools[monsterPoolIndex].Release(monster);
    }

    private Vector2 GetRandomMonsterSpawnPosition()
    {
        Vector2[] sideDirections = { Vector2.left, Vector2.up, Vector2.right, Vector2.down };
        int sideIndex = Random.Range(0, 4);

        Room newRoom = levelManager.DungeonGenerator.Rooms[Random.Range(0, levelManager.DungeonGenerator.Rooms.Count)];
        Vector2Int newRoomPosition = new Vector2Int();
        
        if (!newRoom.SafeRoom)
        {
            newRoomPosition = newRoom.TilePositions[Random.Range(0, newRoom.TilePositions.Count)];
        }
        
        Vector2 spawnPosition = (Vector2)newRoomPosition;
        
        return spawnPosition;
    }

    private Vector2 GetRandomMonsterSpawnPositionPlayerVelocity()
    {
        Vector2[] sideDirections = new Vector2[] { Vector2.left, Vector2.up, Vector2.right, Vector2.down };

        float[] sideWeights = new float[]
        {
            Vector2.Dot(player.Velocity.normalized, sideDirections[0]),
            Vector2.Dot(player.Velocity.normalized, sideDirections[1]),
            Vector2.Dot(player.Velocity.normalized, sideDirections[2]),
            Vector2.Dot(player.Velocity.normalized, sideDirections[3])
        };
        float extraWeight = sideWeights.Sum() / playerDirectionSpawnWeight;
        int badSideCount = sideWeights.Count(x => x <= 0);
        for (int i = 0; i < sideWeights.Length; i++)
        {
            if (sideWeights[i] <= 0)
                sideWeights[i] = extraWeight / badSideCount;
        }

        float totalSideWeight = sideWeights.Sum();

        float rand = Random.Range(0f, totalSideWeight);
        float cumulative = 0;
        int sideIndex = 0;
        for (int i = 0; i < sideWeights.Length; i++)
        {
            cumulative += sideWeights[i];
            if (rand < cumulative)
            {
                sideIndex = i;
                break;
            }
        }
        
        
        Room newRoom = levelManager.DungeonGenerator.Rooms[Random.Range(0, levelManager.DungeonGenerator.Rooms.Count)];
        Vector2Int newRoomPosition = new Vector2Int();
        
        if (!newRoom.SafeRoom)
        {
            newRoomPosition = newRoom.TilePositions[Random.Range(0, newRoom.TilePositions.Count)];
        }
        
        float roomX = newRoomPosition.x;
        float roomY = newRoomPosition.y;
        Vector2 spawnPosition = new Vector2(roomX, roomY);
        
        return spawnPosition;
    }

    #endregion

    #region ExpGem Spawning

    public void SpawnGemsAroundPlayer(int gemCount, GemType gemType = GemType.BlueXPGem)
    {
        for (int i = 0; i < gemCount; i++)
        {
            Vector2 spawnDirection = Random.insideUnitCircle.normalized;
            Vector2 spawnPosition = (Vector2)player.transform.position +
                                    spawnDirection * Mathf.Sqrt(Random.Range(1, Mathf.Pow(minSpawnDistance, 2)));
            SpawnExpGem(spawnPosition, gemType, false);
        }
    }

    public ExpGem SpawnExpGem(Vector2 spawnPosition, GemType gemType, bool spawnAnimation = true)
    {
        ExpGem newGem = expGemPool.Get();
        newGem.Setup(spawnPosition, gemType, spawnAnimation);
        return newGem;
    }

    public void DespawnGem(ExpGem gem)
    {
        expGemPool.Release(gem);
    }

    #endregion

    #region Coin

    public Coin SpawnCoin(Vector2 position, CoinType coinType = CoinType.Coin, bool spawnAnimation = true)
    {
        Coin newCoin = coinPool.Get();
        newCoin.Setup(position, coinType, spawnAnimation);
        return newCoin;
    }

    public void DespawnCoin(Coin coin, bool pickedUpByPlayer = true)
    {
        if (pickedUpByPlayer)
        {
            statisticManager.IncrementCoinsGained((int)coin.CoinType);
        }

        coinPool.Release(coin);
    }

    #endregion

    #region Chest Spawning

    public Chest SpawnChest(ChestBlueprint chestBlueprint)
    {
        Chest newChest = chestPool.Get();
        newChest.Setup(chestBlueprint);

        bool overlapsOtherChest = false;
        int tries = 0;

        do
        {
            Vector2 spawnDirection = Random.insideUnitCircle.normalized;
            Vector2 spawnPosition = (Vector2)player.transform.position + spawnDirection *
                (minSpawnDistance + monsterSpawnBufferDistance + Random.Range(0, chestSpawnRange));
            Vector2 room = levelManager.DungeonGenerator.SpawnPositions[Random.Range(0, levelManager.DungeonGenerator.SpawnPositions.Count)];

            
            newChest.transform.position = room;

            overlapsOtherChest = chests.Any(chest => Vector2.Distance(chest.transform.position, room) < 0.5f);
        } while (overlapsOtherChest && tries++ < 100);

        chests.Add(newChest);
        return newChest;
    }

    public Chest SpawnChest(ChestBlueprint chestBlueprint, Vector2 position)
    {
        Chest newChest = chestPool.Get();
        newChest.transform.position = position;
        newChest.Setup(chestBlueprint);
        chests.Add(newChest);
        return newChest;
    }

    public void DespawnChest(Chest chest)
    {
        chests.Remove(chest);
        chestPool.Release(chest);
    }

    #endregion

    #region Text Spawning

    public DamageText SpawnDamageText(Vector2 position, float damage, bool isPlayer)
    {
        DamageText newText = textPool.Get();
        newText.Setup(position, damage, isPlayer);
        return newText;
    }

    public void DespawnDamageText(DamageText text) => textPool.Release(text);

    #endregion

    #region Projectile Spawning

    public Projectile SpawnProjectile(int projectileIndex, Vector2 position, float damage, float knockback, float speed,
        LayerMask targetLayer)
    {
        Projectile projectile = projectilePools[projectileIndex].Get();
        projectile.Setup(projectileIndex, position, damage, knockback, speed, targetLayer);
        return projectile;
    }

    public void DespawnProjectile(int projectileIndex, Projectile projectile)
    {
        projectilePools[projectileIndex].Release(projectile);
    }

    public int AddPoolForProjectile(GameObject projectilePrefab)
    {
        if (!projectileIndexByPrefab.ContainsKey(projectilePrefab))
        {
            projectileIndexByPrefab[projectilePrefab] = projectilePools.Count;
            ProjectilePool projectilePool = projectilePoolParent.AddComponent<ProjectilePool>();
            projectilePool.Init(this, player, projectilePrefab);
            projectilePools.Add(projectilePool);
            return projectilePools.Count - 1;
        }

        return projectileIndexByPrefab[projectilePrefab];
    }

    #endregion

    #region Throwable Spawning

    public Throwable SpawnThrowable(int throwableIndex, Vector2 position, float damage, float knockback, float speed,
        LayerMask targetLayer)
    {
        Throwable throwable = throwablePools[throwableIndex].Get();
        throwable.Setup(throwableIndex, position, damage, knockback, speed, targetLayer);
        return throwable;
    }

    public void DespawnThrowable(int throwableIndex, Throwable throwable) =>
        throwablePools[throwableIndex].Release(throwable);

    public int AddPoolForThrowable(GameObject throwablePrefab)
    {
        if (!throwableIndexByPrefab.ContainsKey(throwablePrefab))
        {
            throwableIndexByPrefab[throwablePrefab] = throwablePools.Count;
            ThrowablePool throwablePool = throwablePoolParent.AddComponent<ThrowablePool>();
            throwablePool.Init(this, player, throwablePrefab);
            throwablePools.Add(throwablePool);
            return throwablePools.Count - 1;
        }

        return throwableIndexByPrefab[throwablePrefab];
    }

    #endregion

    #region Boomerangs

    public Boomerang SpawnBoomerang(int boomerangIndex, Vector2 position, float damage, float knockback,
        float throwDistance, float throwTime, LayerMask targetLayer)
    {
        Boomerang boomerang = boomerangPools[boomerangIndex].Get();
        boomerang.Setup(boomerangIndex, position, damage, knockback, throwDistance, throwTime, targetLayer);
        return boomerang;
    }

    public void DespawnBoomerang(int boomerangIndex, Boomerang boomerang) =>
        boomerangPools[boomerangIndex].Release(boomerang);

    public int AddPoolForBoomerang(GameObject boomerangPrefab)
    {
        if (!boomerangIndexByPrefab.ContainsKey(boomerangPrefab))
        {
            boomerangIndexByPrefab[boomerangPrefab] = boomerangPools.Count;
            BoomerangPool boomerangPool = boomerangPoolParent.AddComponent<BoomerangPool>();
            boomerangPool.Init(this, player, boomerangPrefab);
            boomerangPools.Add(boomerangPool);
            return boomerangPools.Count - 1;
        }

        return boomerangIndexByPrefab[boomerangPrefab];
    }

    #endregion
}