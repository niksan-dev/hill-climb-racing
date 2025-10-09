using UnityEngine;

public class HudFuel : HUDItemBase
{

    [SerializeField] FuelSystem fuelSystem;

    public override void Collect(EventItemCollected eventItemCollected)
    {
        if (eventItemCollected.item != this.itemType) return;
        // AddValue(eventItemCollected.value);
        //re-fill fuel tank
        fuelSystem.Refill(100);
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