using UnityEngine;

public enum GemType
{
    BlueXPGem = 1,
    RedXPGem = 10,
}

public class ExpGem : Collectable
{
    [Header("Gem Dependencies")] 
    [SerializeField] protected ExpGemBlueprint expGemBlueprint;
    protected GemType gemType;
    protected SpriteRenderer spriteRenderer;


    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void Setup(Vector2 position, GemType gemType = GemType.BlueXPGem, bool spawnAnimation = true)
    {
        transform.position = position;
        this.gemType = gemType;
        (Sprite sprite, Color color) = expGemBlueprint.gemSpritesAndColors[gemType];
        spriteRenderer.sprite = sprite;
        base.Setup(spawnAnimation);
    }
    
    protected override void OnCollected()
    {
        spriteRenderer.enabled = false;
        player.GainExp((float)gemType);
        entityManager.DespawnGem(this);
        spriteRenderer.enabled = true;
    }
}