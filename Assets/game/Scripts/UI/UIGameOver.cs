using UnityEngine;
using UnityEngine.UI;

public class UIGameOver : MonoBehaviour
{
    [SerializeField] Image imageScreenshot;
    [SerializeField] Text textCoins;
    [SerializeField] Text textDistance;
    [SerializeField] Button buttonRestart;

    internal void SetupGameOverUI(GameOverEvent e)
    {
        if (imageScreenshot != null)
        {
            imageScreenshot.sprite = e.GetSprite();
        }

        if (textCoins != null)
        {
            textCoins.text = "Coins: " + e.coinsCollected.ToString();
        }

        if (textDistance != null)
        {
            textDistance.text = "Distance: " + e.distanceTraveled.ToString() + "m";
        }
    }

    void Awake()
    {
        buttonRestart.onClick.AddListener(OnRestartButtonClicked);
    }

    private void OnRestartButtonClicked()
    {
        // Restart the game or reload the scene
        Debug.Log("Restart button clicked!");
        this.gameObject.SetActive(false);
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
