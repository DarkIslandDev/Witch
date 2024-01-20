using System.Collections.Generic;
using UnityEngine;

public class AbilitySelectionDialog : DialogBox
{
    [SerializeField] private Transform abilityCardsParent;
    [SerializeField] private GameObject abilityCardPrefab;
    [SerializeField] private GameObject particles;
    [SerializeField] private ChestBlueprint failsafeChestBlueprint;
    [SerializeField] private float cardPopupDelay = 0.1f;
    [SerializeField] private LocalizedText skipButton;
    [SerializeField] private LocalizedText refreshAbilitiesButton;

    private LevelManager levelManager;
    private AbilityManager abilityManager;
    private EntityManager entityManager;
    private Player player;
    private PauseMenu pauseMenu;
    private List<AbilityCard> abilityCards;
    private List<Ability> displayedAbilities;
    private bool menuOpen = false;

    public bool MenuOpen => menuOpen;
    
    public void Init(LevelManager levelManager, AbilityManager abilityManager, EntityManager entityManager, Player player, PauseMenu pauseMenu)
    {
        this.levelManager = levelManager;
        this.abilityManager = abilityManager;
        this.entityManager = entityManager;
        this.player = player;
        this.pauseMenu = pauseMenu;
    }

    public void Open(bool failsafe = true)
    {
        player.inputManager.startInput = true;
        levelManager.gameState = GameState.LevelUp;
        levelManager.SwitchGameState();
        
        base.Open();
        menuOpen = true;
        
        skipButton.Localize("skip_key");
        refreshAbilitiesButton.Localize("refresh_abilities_key");
        
        // particles.SetActive(true);

        displayedAbilities = abilityManager.SelectAbilities();
        
        if (displayedAbilities.Count > 0)
        {
            Populate(displayedAbilities);
        }
        else
        {
            if (failsafe)
            {
                entityManager.SpawnChest(failsafeChestBlueprint, (Vector2)player.transform.position + Vector2.up);
            }
            Close();
        }
    }
    
    public void RefreshAbilities()
    {
        if (abilityCards == null) abilityCards = new List<AbilityCard>();
        
        List<Ability> abilities = abilityManager.SelectAbilities();
        
        int i = 0;

        for (; i < abilities.Count; i++)
        {
            if (i >= abilityCards.Count)
            {
                abilityCards.Add(Instantiate(abilityCardPrefab, abilityCardsParent).GetComponent<AbilityCard>());
            }
            
            abilityCards[i].Init(this, abilityManager, abilities[i], cardPopupDelay * i);
            abilityCards[i].gameObject.SetActive(true);
        }

        for (; i < abilityCards.Count; i++)
        {
            abilityCards[i].gameObject.SetActive(false);
        }
    }

    private void Populate(List<Ability> abilities)
    {
        if (abilityCards == null) abilityCards = new List<AbilityCard>();

        int i = 0;
        for (; i < abilities.Count; i++)
        {
            if (i >= abilityCards.Count)
            {
                abilityCards.Add(Instantiate(abilityCardPrefab, abilityCardsParent).GetComponent<AbilityCard>());
            }
            
            abilityCards[i].Init(this, abilityManager, abilities[i], cardPopupDelay * i);
            abilityCards[i].gameObject.SetActive(true);
        }
        
        for (; i < abilityCards.Count; i++)
        {
            abilityCards[i].gameObject.SetActive(false);
        }
    }

    public override void Close()
    {
        abilityManager.ReturnAbilities(displayedAbilities);
        menuOpen = false;
        player.inputManager.startInput = false;
        
        levelManager.gameState = GameState.Game;
        levelManager.SwitchGameState();
        base.Close();
    }

    

    public bool HasAvailableAbilities() => abilityManager.HasAvailableAbilities();
}