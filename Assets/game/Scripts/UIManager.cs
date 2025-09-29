using UnityEngine;


public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject InGameUI;
    [SerializeField] GameObject GameOverUI;


    void OnEnable()
    {
        SetInGameUI(true);
        SetGameOverUI(false);
        EventBus.Subscribe<GameOverEvent>(OnGameOver);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<GameOverEvent>(OnGameOver);
    }

    void OnGameOver(GameOverEvent e)
    {
        SetInGameUI(false);
        SetGameOverUI(true);

        Debug.Log("Game Over! Coins: " + e.coinsCollected + ", Distance: " + e.distanceTraveled);

        GameOverUI.GetComponent<UIGameOver>()?.SetupGameOverUI(e);
    }

    void SetInGameUI(bool state)
    {
        InGameUI.SetActive(state);
    }

    void SetGameOverUI(bool state)
    {
        GameOverUI.SetActive(state);
    }
}

public struct GameOverEvent
{
    public Texture2D screenshot;
    public int coinsCollected;
    public int distanceTraveled;

    public Sprite GetSprite()
    {
        Debug.Log("screenshot : " + screenshot);
        if (screenshot == null) return null;
        return Sprite.Create(screenshot, new Rect(0, 0, screenshot.width, screenshot.height), new Vector2(0.5f, 0.5f));
    }
}
