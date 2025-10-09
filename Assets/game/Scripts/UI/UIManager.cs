
using UnityEngine;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] GameObject InGameUI;
    [SerializeField] GameObject GameOverUI;
    [SerializeField] Canvas mainCanvas;
    [SerializeField] RectTransform coinHudIcon;
    [SerializeField] RectTransform gemHudIcon;
    [SerializeField] RectTransform fuelHudIcon;
    private void Awake()
    {
        Instance = this;
    }

    internal RectTransform GetCoinIconTransform()
    {
        return coinHudIcon;
    }

    internal RectTransform GetFuelIconTransform()
    {
        return fuelHudIcon;
    }

    internal RectTransform GetGemIconTransform()
    {
        return gemHudIcon;
    }

    internal Canvas GetMainCanvas()
    {
        return mainCanvas;
    }

    void OnEnable()
    {
        SetInGameUI(true);
        SetGameOverUI(false);
        EventBus.Subscribe<EventGameOver>(OnGameOver);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<EventGameOver>(OnGameOver);
    }

    void OnGameOver(EventGameOver e)
    {
        SetInGameUI(false);
        SetGameOverUI(true);

        Debug.Log("Game Over! Coins: " + e.coinsCollected + ", Distance: " + e.distanceTraveled);

        GameOverUI.GetComponent<UIGameOver>()?.SetupGameOverUI(e);
    }

    internal void SetInGameUI(bool state)
    {
        InGameUI.SetActive(state);
    }

    internal void SetGameOverUI(bool state)
    {
        GameOverUI.SetActive(state);
    }
}



public struct HeadCollidedEvent
{

}
