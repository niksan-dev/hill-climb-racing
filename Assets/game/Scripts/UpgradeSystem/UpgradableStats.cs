using UnityEngine;

[System.Serializable]
public class UpgradeableStat
{
    [Tooltip("Base value before upgrades")]
    public float baseValue = 1f;

    [Tooltip("Increment per upgrade level")]
    public float upgradeStep = 0.1f;

    [Tooltip("Current upgrade level (0 = no upgrade)")]
    [Range(1, 10)] public int level = 0;

    /// <summary>
    /// Returns the upgraded value.
    /// </summary>
    public float Value => baseValue + (upgradeStep * level);
}
