public class Shield : Collectable
{
    protected override void OnCollected()
    {
        player.TakeGodMode(collectableType.cooldown);
        Destroy(gameObject);
    }

}