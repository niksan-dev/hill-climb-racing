using System.Collections;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    [SerializeField] private int coinValue = 5;
    [SerializeField] public GameObject uiCoinPrefab;
    [SerializeField] private float flyDuration = 0.6f;
    [SerializeField] private AnimationCurve moveCurve;  // easing curve
    [SerializeField] private AnimationCurve heightCurve; // arc curve
    private Canvas targetCanvas;       // your UI canvas
    private RectTransform targetIcon;  // coin UI icon
    private bool collected = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetCanvas = UIManager.Instance.GetMainCanvas();
        targetIcon = UIManager.Instance.GetCoinIconTransform();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("player") || collision.CompareTag("car") || collision.CompareTag("tire"))
        {
            if (collected) return;
            collected = true;
            SpawnUICoin();

            Destroy(gameObject);

        }
    }

    private void SpawnUICoin()
    {
        // Get screen position of world coin
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);

        // Spawn UI coin at that position inside canvas
        // GameObject uiCoin = // Get pooled UI coin
        GameObject uiCoin = CoinPoolManager.Instance.Get(this.coinValue);
        uiCoin.gameObject.SetActive(true);
        uiCoin.transform.SetParent(targetCanvas.transform, false);
        RectTransform uiRect = uiCoin.GetComponent<RectTransform>();
        uiRect.position = transform.position;

        // Start fly animation
        uiCoin.GetComponent<UICoinFly>().StartFly(targetIcon, coinValue);
    }
}
