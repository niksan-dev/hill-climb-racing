using UnityEngine;
public class InputController : MonoBehaviour
{
    // UI Input values
    [HideInInspector] public float gasInput;    // +1 when pressed
    [HideInInspector] public float brakeInput;  // +1 when pressed

    // Or you can use bools:
    public bool gasPressed;
    public bool brakePressed;

    // Internally convert to one axis:
    internal float _moveInput;
    float uiAxis = 0f;
    void Update()
    {
        // Combine UI input with keyboard fallback

        if (gasPressed) uiAxis += 0.01f;
        else
            uiAxis -= 0.01f;

        uiAxis = Mathf.Clamp(uiAxis, 0, 1f);
        _moveInput = uiAxis;



        // ... rest of Update() logic (ground checks etc.)
    }

    // Called by UI buttons
    public void GasPressedDown() => gasPressed = true;
    public void GasReleased() => gasPressed = false;
    public void BrakePressedDown() => brakePressed = true;
    public void BrakeReleased() => brakePressed = false;
}
