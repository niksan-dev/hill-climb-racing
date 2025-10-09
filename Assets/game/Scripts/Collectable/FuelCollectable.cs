using UnityEngine;

public class FuelCollectable : CollectableBase
{

    void Start()
    {
        targetCanvas = UIManager.Instance.GetMainCanvas();
        targetIcon = UIManager.Instance.GetFuelIconTransform();
    }

    protected override void HandleCollect(GameObject collector)
    {
        // Refuel car
        //FuelSystem.Instance.Refill(value);
        Debug.Log($"Refilled Fuel: {value}");


        // Get pooled UI coin
        GameObject uiCoin = CoinPoolManager.Instance.Get(this.value);
        uiCoin.gameObject.SetActive(true);
        uiCoin.transform.SetParent(targetCanvas.transform, false);

        RectTransform uiRect = uiCoin.GetComponent<RectTransform>();
        uiRect.position = transform.position; //Camera.main.WorldToScreenPoint(transform.position);

        uiCoin.GetComponent<UICoinFly>().StartFly(targetIcon, value);

        EventBus.Publish(new EventItemCollected()
        {
            value = 1,
            item = HUDItem.FUEL
        });
    }
}
