using UnityEngine;

[System.Serializable]
public class UpgradeableStat
{
    [Tooltip("Base value before upgrades")]
    public float baseValue = 1f;

    [Tooltip("Increment per upgrade level")]
    public float upgradeStep = 0.1f;

    [Tooltip("Current upgrade level (0 = no upgrade)")]
    [Range(0, 10)] public int level = 0;

    [Header("Cost Settings")]
    [Tooltip("Base cost of first upgrade")]
    public int baseCost = 100;

    [Tooltip("Cost increases per level (e.g. +50 each level)")]
    public int costStep = 50;

    [Tooltip("Maximum upgrade level")]
    public int maxLevel = 10;

    /// <summary> Returns current upgraded value </summary>
    public float Value => baseValue + (upgradeStep * level);

    /// <summary> Returns cost for next upgrade </summary>
    public int NextCost => (level < maxLevel) ? baseCost + (costStep * level) : -1;

    /// <summary> Can we still upgrade? </summary>
    public bool CanUpgrade => level < maxLevel;

    /// <summary> Upgrade stat (if not maxed) </summary>
    public void Upgrade() { if (CanUpgrade) level++; }
}
