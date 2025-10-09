public class HUDGem : HUDItemBase
{

    public override void Collect(EventItemCollected eventItemCollected)
    {
        if (eventItemCollected.item != this.itemType) return;
        AddValue(eventItemCollected.value);
        // Trigger coin collect animation, sound, etc.
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        EventBus.Subscribe<EventItemCollected>(Collect);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<EventItemCollected>(Collect);
    }
}