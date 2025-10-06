using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public enum UpgradeType
{
    Engine,
    Wheels,
    Suspension,
    Turbo,
    FuelTank
}

[Serializable]
public class PlayerInventoryData
{
    public int coins;
    public int gems;
    public int fuel;
    public Dictionary<UpgradeType, int> upgrades = new Dictionary<UpgradeType, int>();
}

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    private PlayerInventoryData data;
    private string SavePath => Path.Combine(Application.persistentDataPath, "playerInventory.dat");

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Load();

        AddCoins(50);
        AddGems(10);
    }

    #region Currency
    public int Coins => data.coins;
    public int Gems => data.gems;
    public int Fuel => data.fuel;

    public void AddCoins(int amount)
    {
        data.coins += amount;
        Save();
    }

    public void AddGems(int amount)
    {
        data.gems += amount;
        Save();
    }

    public void AddFuel(int amount)
    {
        data.fuel += amount;
        Save();
    }

    public bool SpendCoins(int amount)
    {
        if (data.coins < amount) return false;
        data.coins -= amount;
        Save();
        return true;
    }

    public bool SpendGems(int amount)
    {
        if (data.gems < amount) return false;
        data.gems -= amount;
        Save();
        return true;
    }
    #endregion

    #region Upgrades
    public int GetUpgradeLevel(UpgradeType type)
    {
        if (data.upgrades.ContainsKey(type))
            return data.upgrades[type];
        return 0;
    }

    public void Upgrade(UpgradeType type)
    {
        if (!data.upgrades.ContainsKey(type))
            data.upgrades[type] = 0;

        data.upgrades[type]++;
        Save();
    }
    #endregion

    #region Save & Load
    public void Save()
    {
        try
        {
            using (FileStream file = File.Create(SavePath))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(file, data);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Save failed: " + ex.Message);
        }
    }

    public void Load()
    {
        if (File.Exists(SavePath))
        {
            try
            {
                using (FileStream file = File.Open(SavePath, FileMode.Open))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    data = (PlayerInventoryData)bf.Deserialize(file);

                    Debug.Log($"Load successful : {JsonUtility.ToJson(data)}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Load failed: " + ex.Message);
                data = new PlayerInventoryData();
            }
        }
        else
        {
            data = new PlayerInventoryData();
        }
    }

    public void ResetData()
    {
        data = new PlayerInventoryData();
        Save();
    }
    #endregion
}
