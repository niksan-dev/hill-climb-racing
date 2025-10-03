using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private CarConfigStats carConfigStats;
    [SerializeField] private int playerCoins = 1000;

    public int PlayerCoins => playerCoins;

    public bool TryUpgrade(UpgradeableStat stat)
    {
        if (!stat.CanUpgrade) return false;
        if (playerCoins < stat.NextCost) return false;

        playerCoins -= stat.NextCost;
        stat.Upgrade();
        return true;
    }
}