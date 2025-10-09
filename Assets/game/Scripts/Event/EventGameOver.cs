using UnityEngine;
public struct EventGameOver
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
