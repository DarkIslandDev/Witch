using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Player : IDamageable
{
    public InputManager inputManager;
    public PlayerMovement playerMovement;
    public PlayerCamera playerCamera;
    public PlayerInventory playerInventory;
    public PlayerAnimator playerAnimator;
    public PlayerUI playerUI;
    
    protected AbilityManager abilityManager;
    protected EntityManager entityManager;
    protected StatisticManager statisticManager;
    protected ZPositioner zPositioner;

    protected Collider2D playerHitBox;
    [SerializeField] protected Transform centerTransform;
    [SerializeField] protected Collider2D collectableCollider;
    [SerializeField] protected Collider2D meleeHitBoxCollider;
    [SerializeField] protected ParticleSystem dustParticles;
    [SerializeField] protected ParticleSystem deathParticles;
    [SerializeField] protected ParticleSystem shieldParticles;
    [SerializeField] protected Material defaultMaterial;
    [SerializeField] protected Material hitMaterial;
    [SerializeField] protected Material deathMaterial;
    public SpriteRenderer spriteRenderer;
    
    [SerializeField] protected CharacterBlueprint playerBlueprint;
    protected UpgradeableMovementSpeed movementSpeed;
    protected UpgradeableArmor armor;

    protected bool alive = true;
    protected bool godMode = false;
    protected int currentLevel = 1;
    protected float currentExpirience = 0;
    protected float nextLevelExpirience = 5;
    public float expirienceToNextLevel = 5;
    protected float currentHealth;
    protected float maxHealth;
    protected Vector2 lookDirection => inputManager.movementInput;
    protected CoroutineQueue coroutineQueue;
    protected Coroutine hitAnimationCoroutine = null;
    
    public UnityEvent<float> OnDealDamage { get; } = new UnityEvent<float>();
    public UnityEvent OnDeath { get; } = new UnityEvent();

    public Transform CenterTransform => centerTransform;
    public Collider2D CollectableCollider => collectableCollider;
    public CharacterBlueprint PlayerBlueprint => playerBlueprint;
    public int CurrentLevel => currentLevel;
    public float CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public float Luck => playerBlueprint.luck;
    public bool IsLeft => playerAnimator.isLeft;
    public Vector2 LookDirection => lookDirection;
    public Vector2 Velocity => playerMovement.rigidbody.velocity;

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        
        inputManager.HandleMovement(playerBlueprint.moveSpeed);
        inputManager.HandleQuickSlots();
        inputManager.PauseGame();
    }

    private void FixedUpdate()
    {
        float fixedDeltaTime = Time.fixedDeltaTime;
        
        inputManager.HandleCamera();
    }

    private void LateUpdate()
    {
        float deltaTime = Time.deltaTime;

        inputManager.xInput = false;
        inputManager.bInput = false;
        inputManager.aInput = false; 
        inputManager.yInput = false;
        inputManager.startInput = false;
    }

    public virtual void Init(EntityManager entityManager, AbilityManager abilityManager, StatisticManager statisticManager)
    {
        this.entityManager = entityManager;
        this.abilityManager = abilityManager;
        this.statisticManager = statisticManager;
        
        inputManager = GetComponent<InputManager>();
        playerMovement = GetComponent<PlayerMovement>();
        playerCamera = GetComponent<PlayerCamera>();
        playerInventory = GetComponent<PlayerInventory>();
        playerUI = GetComponent<PlayerUI>();
        playerAnimator = GetComponentInChildren<PlayerAnimator>();
        playerHitBox = spriteRenderer.gameObject.AddComponent<BoxCollider2D>();

        playerInventory.Init();

        inputManager.player = this;
        zPositioner = gameObject.AddComponent<ZPositioner>();
        
        OnDealDamage?.AddListener(statisticManager.IncreaseDamageDealt);

        coroutineQueue = new CoroutineQueue(this);
        coroutineQueue.StartLoop();

        currentHealth = playerBlueprint.hp;
        maxHealth = playerBlueprint.hp;
        playerUI.healthBar.Setup(currentHealth, 0, maxHealth, true);
        playerUI.levelBar.Setup(currentExpirience, 0, nextLevelExpirience, true);
        currentLevel = 1;

        UpdateLevelDisplay();

        movementSpeed = new UpgradeableMovementSpeed
        {
            Value = playerBlueprint.moveSpeed
        };
        abilityManager.RegisterUpgradeableValue(movementSpeed, true);
        UpdateMoveSpeed();
        armor = new UpgradeableArmor
        {
            Value = playerBlueprint.armor
        };
        abilityManager.RegisterUpgradeableValue(armor, true);
        
        zPositioner.Init(transform);
    }

    public void GainExp(float exp)
    {
        if(alive) coroutineQueue.EnqueueCoroutine(GainExpCoroutine(exp));
    }

    private IEnumerator GainExpCoroutine(float exp)
    {
        if (alive)
        {
            while (currentExpirience + exp >= nextLevelExpirience)
            {
                float expDiff = nextLevelExpirience - currentExpirience;
                currentExpirience += expDiff;
                exp -= expDiff;
                playerUI.levelBar.Setup(currentExpirience, 0, nextLevelExpirience);

                yield return LevelUpCoroutine();

                float prevLevelExp = nextLevelExpirience;
                expirienceToNextLevel += playerBlueprint.LevelToExpIncrease(currentLevel);
                nextLevelExpirience += expirienceToNextLevel;
                playerUI.levelBar.Setup(currentExpirience, prevLevelExp, nextLevelExpirience);
            }

            currentExpirience += exp;
            playerUI.levelBar.AddPoints(exp);
        }
    }

    private IEnumerator LevelUpCoroutine()
    {
        if (alive)
        {
            currentLevel++;
            UpdateLevelDisplay();

            playerUI.abilitySelectionDialog.Open();

            while (playerUI.abilitySelectionDialog.MenuOpen)
            {
                yield return null;
            }
        }
    }

    private void UpdateLevelDisplay() => playerUI.levelText.text = $"{currentLevel}lv.";
    
    public override void KnockBack(Vector2 knockBack) => playerMovement.rigidbody.velocity += knockBack * Mathf.Sqrt(playerMovement.rigidbody.drag);

    public override void TakeDamage(float damage, Vector2 knockBack = default(Vector2))
    {
        if (alive)
        {
            if (!godMode)
            {
                if (armor.Value >= damage)
                {
                    damage = damage < 1 ? damage : 1;
                }
                else
                {
                    damage -= armor.Value;
                }
            
                currentHealth -= damage;
                playerUI.healthBar.SubstractPoints(damage);
                entityManager.SpawnDamageText(playerHitBox.bounds.max, damage, true);

                playerMovement.rigidbody.velocity += knockBack * Mathf.Sqrt(playerMovement.rigidbody.drag);
                statisticManager.IncreaseDamageTaken(damage);
            
                if (currentHealth <= 0)
                {
                    currentHealth = 0;
                    StartCoroutine(DeathAnimation());
                }
                else
                {
                    if(hitAnimationCoroutine != null) StopCoroutine(hitAnimationCoroutine);
                    hitAnimationCoroutine = StartCoroutine(HitAnimation());
                }
            }
        }
    }

    private IEnumerator HitAnimation()
    {
        spriteRenderer.sharedMaterial = hitMaterial;
        yield return new WaitForSeconds(0.15f);
        spriteRenderer.sharedMaterial = defaultMaterial;
    }

    private IEnumerator DeathAnimation()
    {
        alive = false;
        spriteRenderer.sharedMaterial = defaultMaterial;
        
        abilityManager.DestroyActiveAbilities();
        deathParticles.Play();
        float height = spriteRenderer.bounds.size.y;
        float t = 0;

        while (t < 1)
        {
            spriteRenderer.sharedMaterial = deathMaterial;
            deathParticles.transform.position = transform.position + Vector3.up * height * (1 - t);
            deathMaterial.SetFloat("_CutoffHeight", t);
            t += Time.deltaTime;
            yield return null;
        }

        deathMaterial.SetFloat("_Cutoff_height", 1.0f);
        
        yield return new WaitForSeconds(0.5f);
        
        OnDeath?.Invoke();
        spriteRenderer.enabled = false;
    }

    public void GainHealth(float health)
    {
        playerUI.healthBar.AddPoints(health);
        currentHealth += health;

        if (currentHealth >= maxHealth) currentHealth = maxHealth;
    }

    public void UpdateMoveSpeed() => playerMovement.rigidbody.drag = playerBlueprint.acceleration / (movementSpeed.Value * movementSpeed.Value);

    public void TakeGodMode(float amount)
    {
        StopCoroutine(GodMode(amount));
        StartCoroutine(GodMode(amount));
    }
    
    public IEnumerator GodMode(float amount)
    {
        godMode = true;
        shieldParticles.Play();

        yield return new WaitForSeconds(amount);

        shieldParticles.Stop();
        godMode = false;
    }

    public void TakeSpeedGain(float time, float amount)
    {
        StopCoroutine(SpeedGain(time, amount));
        StartCoroutine(SpeedGain(time, amount));
    }

    private IEnumerator SpeedGain(float time, float amount)
    {
        ChangeSpeed(amount, true);
        
        yield return new WaitForSeconds(time);
        
        ChangeSpeed(amount, false);
    }

    public void ChangeSpeed(float amount, bool gainSpeed)
    {
        if (gainSpeed)
        {
            movementSpeed.Value += amount;
        }
        else
        {
            movementSpeed.Value -= amount;
        }

        UpdateMoveSpeed();
    }
}
