using UnityEngine;

[CreateAssetMenu(fileName = "CarConfig", menuName = "Configs/CarConfigStats")]
public class CarConfigStats : ScriptableObject
{
    [Header("Engine Settings")]
    public UpgradeableStat accelerationTorque;
    public UpgradeableStat brakeTorque;
    public UpgradeableStat maxAngularVelocity;

    [Header("Tilt Settings")]
    public UpgradeableStat airTiltTorque;

    [Header("Suspension Settings")]
    public UpgradeableStat suspensionFrequency;
    public UpgradeableStat suspensionDamping;

    [Header("Tire Settings")]
    public UpgradeableStat tireGrip;   // ← NEW

    [Header("Drive Settings")]
    [Tooltip("0 = RWD, 0.5 = AWD, 1 = FWD")]
    public UpgradeableStat powerDistribution; // ← NEW

    [Header("Drag Settings")]
    public UpgradeableStat groundLinearDamping;
    public UpgradeableStat airLinearDamping;
    public UpgradeableStat angularDamping;
}
