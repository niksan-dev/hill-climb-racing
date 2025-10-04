using UnityEngine;

public class RPMMeter
{
    private RectTransform needleTransform;
    private float minAngle = -90f; // Angle at 0
    private float maxAngle = 90;  // Angle at max RPM
    private float maxRPM = 8000f;  // Maximum RPM

    float currentRPM = 0f;

    internal void Initialize(float maxAngularVelocity, RectTransform needleTransform)
    {
        currentRPM = 0f;
        this.maxRPM = maxAngularVelocity * 60f / (2f * Mathf.PI);
        this.needleTransform = needleTransform;
    }

    internal void UpdateNeedle(float currentRPM)
    {
        float clampedRPM = Mathf.Clamp(currentRPM, 0, maxRPM);
        float angle = Mathf.Lerp(minAngle, maxAngle, clampedRPM / maxRPM);
        needleTransform.localEulerAngles = new Vector3(0, 0, -angle + 45);
    }
}
