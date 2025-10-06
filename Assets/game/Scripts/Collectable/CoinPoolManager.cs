using UnityEngine;
using System.Collections.Generic;

public class CoinPoolManager : MonoBehaviour
{
    [System.Serializable]
    public class CoinPool
    {
        public int value;                 // 5, 25, 50, 100, 500, 101=>fuel, 102=>gems
        public GameObject prefab;         // UI prefab for this coin type
        public int initialSize = 10;      // how many to preload
    }

    public static CoinPoolManager Instance;

    [SerializeField] private List<CoinPool> poolsConfig;

    private Dictionary<int, Queue<GameObject>> poolDict = new();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Preload each pool
        foreach (var pool in poolsConfig)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.initialSize; i++)
            {
                GameObject obj = Instantiate(pool.prefab, transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDict[pool.value] = objectPool;
        }
    }

    public GameObject Get(int value)
    {
        if (!poolDict.ContainsKey(value))
        {
            Debug.LogWarning($"No pool configured for coin value {value}!");
            return null;
        }

        if (poolDict[value].Count == 0)
        {
            // Expand pool
            var prefab = poolsConfig.Find(p => p.value == value).prefab;
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            return obj;
        }
        else
        {
            return poolDict[value].Dequeue();
        }
    }

    public void Return(int value, GameObject obj)
    {
        obj.SetActive(false);
        poolDict[value].Enqueue(obj);
    }
}
