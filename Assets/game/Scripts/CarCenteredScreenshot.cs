using UnityEngine;
using System.IO;
using System.Collections;

public class CarCenteredScreenshot : MonoBehaviour
{
    [Header("Car Reference")]
    public Transform carTransform;   // Assign your car here
    public Camera mainCamera;        // Assign your main camera here

    [Header("Screenshot Settings")]
    public int captureWidth = 400;   // Width of the screenshot
    public int captureHeight = 300;  // Height of the screenshot
    public string saveFileName = "CarScreenshot.png";

    [Header("Trigger")]
    public KeyCode captureKey = KeyCode.P;  // Press this key to capture
    internal Texture2D _screenshot;


    internal IEnumerator TakeCarScreenshot()
    {
        if (carTransform == null || mainCamera == null)
        {
            Debug.LogWarning("Car or Camera not assigned!");
            yield break;
        }
        yield return new WaitForEndOfFrame();
        // Convert car position to screen coordinates
        Vector3 carScreenPos = mainCamera.WorldToScreenPoint(carTransform.position);

        // Define a rectangle around the car
        int x = Mathf.Clamp((int)(carScreenPos.x - captureWidth / 2), 0, Screen.width - captureWidth);
        int y = Mathf.Clamp((int)(carScreenPos.y - captureHeight / 2), 0, Screen.height - captureHeight);
        Rect captureRect = new Rect(x, y, captureWidth, captureHeight);

        // Read pixels from the screen
        Texture2D screenshot = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false);
        screenshot.ReadPixels(captureRect, 0, 0);
        screenshot.Apply();
        _screenshot = screenshot;
        // Save as PNG
        string path = Path.Combine(Application.dataPath, saveFileName);
        File.WriteAllBytes(path, screenshot.EncodeToPNG());
        Debug.Log("Screenshot saved to: " + path);

        // Destroy(screenshot);
    }
}
