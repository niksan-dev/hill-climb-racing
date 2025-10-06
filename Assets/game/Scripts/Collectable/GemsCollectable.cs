using UnityEngine;

public class GemCollectable : CollectableBase
{
    void Start()
    {
        targetCanvas = UIManager.Instance.GetMainCanvas();
        targetIcon = UIManager.Instance.GetGemIconTransform();
    }
    protected override void HandleCollect(GameObject collector)
    {
        // Add gems to inventory or UI
        //PlayerInventory.Instance.AddGems(value);
        Debug.Log($"Collected {value} Gems!");

        // Get pooled UI coin
        GameObject uiCoin = CoinPoolManager.Instance.Get(this.value);
        uiCoin.gameObject.SetActive(true);
        uiCoin.transform.SetParent(targetCanvas.transform, false);

        RectTransform uiRect = uiCoin.GetComponent<RectTransform>();
        uiRect.position = transform.position; //Camera.main.WorldToScreenPoint(transform.position);

        uiCoin.GetComponent<UICoinFly>().StartFly(targetIcon, value);
    }
}
