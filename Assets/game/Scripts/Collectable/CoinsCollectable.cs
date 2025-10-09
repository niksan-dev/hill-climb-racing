using UnityEngine;

public class CoinCollectable : CollectableBase
{


    void Start()
    {
        targetCanvas = UIManager.Instance.GetMainCanvas();
        targetIcon = UIManager.Instance.GetCoinIconTransform();
    }

    protected override void HandleCollect(GameObject collector)
    {
        // Get pooled UI coin
        GameObject uiCoin = CoinPoolManager.Instance.Get(this.value);
        uiCoin.gameObject.SetActive(true);
        uiCoin.transform.SetParent(targetCanvas.transform, false);

        RectTransform uiRect = uiCoin.GetComponent<RectTransform>();
        uiRect.position = transform.position; //Camera.main.WorldToScreenPoint(transform.position);

        uiCoin.GetComponent<UICoinFly>().StartFly(targetIcon, value);


        EventBus.Publish(new EventItemCollected()
        {
            value = this.value,
            item = HUDItem.COIN
        });
    }
}
