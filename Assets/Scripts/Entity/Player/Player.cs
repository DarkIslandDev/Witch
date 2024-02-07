using System;
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
    [SerializeField] protected CircleCollider2D collectableCollider;
    [SerializeField] protected Collider2D boomerangCollider;
    [SerializeField] protected Collider2D meleeHitBoxCollider;
    [SerializeField] protected ParticleSystem dustParticles;
    [SerializeField] protected ParticleSystem deathParticles;
    [SerializeField] protected ParticleSystem shieldParticles;
    [SerializeField] protected Material defaultMaterial;
    [SerializeField] protected Material hitMaterial;
    [SerializeField] protected Material deathMaterial;
    [SerializeField] protected GameObject shadow;
    public SpriteRenderer spriteRenderer;

    [SerializeField] protected CharacterBlueprint playerBlueprint;
    protected UpgradeableHealth health;
    protected UpgradeableMovementSpeed movementSpeed;
    protected UpgradeableArmor armor;

    protected bool alive = true;
    protected bool canRevive = true;
    protected bool godMode = false;
    
    protected int currentLevel = 1;
    protected float currentExpirience = 0;
    protected float nextLevelExpirience = 5;
    public float expirienceToNextLevel = 5;
    
    protected float currentHealth;
    protected float maxHealth;
    protected float bonusHealth => playerBlueprint.bonusHP;
    protected Vector2 lookDirection => inputManager.movementInput;
    protected CoroutineQueue coroutineQueue;
    protected Coroutine hitAnimationCoroutine = null;

    public UnityEvent<float> OnDealDamage { get; } = new UnityEvent<float>();
    public UnityEvent OnDeath { get; } = new UnityEvent();
    
    public Transform CenterTransform => centerTransform;
    public CircleCollider2D CollectableCollider => collectableCollider;
    public Collider2D BoomerangCollider => boomerangCollider;
    public CharacterBlueprint PlayerBlueprint => playerBlueprint;
    public int CurrentLevel => currentLevel;
    public float CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public float BonusHealth => bonusHealth;
    public float Luck => playerBlueprint.luck;
    public bool IsLeft => playerAnimator.isLeft;
    public Vector2 LookDirection => lookDirection;
    public Vector2 Velocity => playerMovement.rigidbody.velocity;
    public bool CanRevive => canRevive;

    private void Update()
    {
        if (alive)
        {
            inputManager.HandleMovement(movementSpeed.Value);
            inputManager.PauseGame();
            inputManager.HandleQuickSlots();
        }
    }

    private void FixedUpdate()
    {
        if (alive) inputManager.HandleCamera();
    }

    private void LateUpdate()
    {
        inputManager.xInput = false;
        inputManager.bInput = false;
        inputManager.aInput = false;
        inputManager.yInput = false;
        inputManager.startInput = false;
    }

    public void Init(EntityManager entityManager, AbilityManager abilityManager, StatisticManager statisticManager)
    {
        alive = true;
        
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
        playerHitBox.isTrigger = true;

        centerTransform.position = transform.position + new Vector3(0, 0.5f, 0);
        
        playerInventory.Init();

        inputManager.player = this;
        zPositioner = gameObject.AddComponent<ZPositioner>();

        OnDealDamage?.AddListener(statisticManager.IncreaseDamageDealt);

        coroutineQueue = new CoroutineQueue(this);
        coroutineQueue.StartLoop();

        currentHealth = playerBlueprint.hp;
        maxHealth = playerBlueprint.hp;
        playerUI.healthBar.Setup(currentHealth, 0, maxHealth, true);
        
        currentLevel = 1;
        playerUI.levelBar.Setup(currentExpirience, 0, nextLevelExpirience, true);
        UpdateLevelDisplay();

        health = new UpgradeableHealth()
        {
            Value = playerBlueprint.hp
        };
        abilityManager.RegisterUpgradeableValue(health, true);
        
        movementSpeed = new UpgradeableMovementSpeed
        {
            Value = playerBlueprint.moveSpeed
        };
        abilityManager.RegisterUpgradeableValue(movementSpeed, true);
        IncreaseMovementSpeed();
        
        armor = new UpgradeableArmor
        {
            Value = playerBlueprint.armor
        };
        abilityManager.RegisterUpgradeableValue(armor, true);

        shadow.transform.localScale = new Vector3(spriteRenderer.bounds.size.x, shadow.transform.localScale.y,
            shadow.transform.localScale.z);
        shadow.SetActive(true);
        
        zPositioner.Init(transform);
    }

    public void GainExp(float exp)
    {
        if (alive) coroutineQueue.EnqueueCoroutine(GainExpCoroutine(exp));
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
            playerUI.levelBar.AddCurrentPoints(exp);
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

    public override void KnockBack(Vector2 knockBack)
    {
        if (alive) playerMovement.rigidbody.velocity += knockBack * Mathf.Sqrt(playerMovement.rigidbody.drag);
    }

    public override void TakeDamage(float damage, Vector2 knockBack = default)
    {
        if (!godMode)
        {
            if (alive)
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
                    if (hitAnimationCoroutine != null) StopCoroutine(hitAnimationCoroutine);
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

        if (!canRevive) abilityManager.DestroyActiveAbilities();
        
        deathParticles.Play();
        float height = spriteRenderer.bounds.size.y;
        float t = 0;

        while (t < 1)
        {
            spriteRenderer.sharedMaterial = deathMaterial;
            deathParticles.transform.position = transform.position + Vector3.up * (height * (1 - t));
            deathMaterial.SetFloat("_DissolveAmount", t);
            t += Time.deltaTime;
            yield return null;
        }

        deathMaterial.SetFloat("_DissolveAmount", 1.0f);

        yield return new WaitForSeconds(0.5f);

        OnDeath?.Invoke();
        spriteRenderer.enabled = false;
        deathParticles.Stop();
    }

    public void Revive()
    {
        alive = true;
        spriteRenderer.sharedMaterial = defaultMaterial;
        spriteRenderer.enabled = true;
        
        currentHealth = maxHealth / 2;
        playerUI.healthBar.AddCurrentPoints(currentHealth);
        
        canRevive = false;
    }

    public void GainHealth(float health)
    {
        playerUI.healthBar.AddCurrentPoints(health);
        currentHealth += health;

        if (currentHealth >= maxHealth) currentHealth = maxHealth;
    }

    public void GainMaxHealth(float maxHealth)
    {
        playerUI.healthBar.AddMaxPoints(maxHealth);
        this.maxHealth = maxHealth;
    }

    public void IncreaseMovementSpeed()
    {
        playerMovement.rigidbody.drag = playerBlueprint.acceleration / (movementSpeed.Value * movementSpeed.Value);
    }

    public IEnumerator ChangeMoveSpeed(float cooldown)
    {
        movementSpeed.Value *= 2;

        yield return new WaitForSeconds(cooldown);

        movementSpeed.Value /= 2;
    }

    public void TakeGodMode(float amount)
    {
        StopCoroutine(GodMode(0));
        StartCoroutine(GodMode(amount));
    }

    public void TakeGodMode(bool enebale)
    {
        if (enebale)
        {
            StartCoroutine(GodMode(float.PositiveInfinity));
        }
        else
        {
            StopCoroutine(GodMode(0));
        }
    }

    public IEnumerator GodMode(float amount)
    {
        godMode = true;
        shieldParticles.Play();

        yield return new WaitForSeconds(amount);

        shieldParticles.Stop();
        godMode = false;
    }
}