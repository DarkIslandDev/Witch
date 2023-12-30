using UnityEngine;

public enum CoinType
{
    Coin = 1,
    Bag = 50
}

public class Coin : Collectable
{
    [Header("Coin dependencies")] 
    [SerializeField] protected CoinBlueprint coinBlueprint;
    
    protected SpriteRenderer spriteRenderer;
    protected CoinType coinType;
    
    public CoinType CoinType => coinType;

    protected override void Awake()
    {
        base.Awake();
        zPositioner = gameObject.AddComponent<ZPositioner>();
    }
    

    public void Setup(Vector2 position, CoinType coinType = CoinType.Coin, bool spawnAnimation = true,
        bool collectableDuringSpawn = true)
    {
        this.coinType = coinType;
        spriteRenderer.sprite = coinBlueprint.coinSprites[coinType];
        transform.position = position;
        base.Setup(spawnAnimation, collectableDuringSpawn);
    }
    
    protected override void OnCollected()
    {
        entityManager.DespawnCoin(this);
    }
}