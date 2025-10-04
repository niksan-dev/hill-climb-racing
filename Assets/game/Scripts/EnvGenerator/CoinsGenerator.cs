using UnityEngine;
using UnityEngine.U2D; // For SpriteShapeController
using System.Collections.Generic;

public class CoinsGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteShapeController terrainShape;
    [SerializeField] private GameObject[] coinPrefabs;
    [SerializeField] private Transform coinParent;

    [Header("Settings")]
    [SerializeField] private float initialXValue = 10f;
    [SerializeField] private float spacing = 3f;        // distance between coins
    [SerializeField] private float heightOffset = 1.5f; // lift coins above terrain
    [SerializeField] private float maxSlopeAngle = 60f; // max slope angle
    [SerializeField, Range(0f, 1f)] private float spawnChance = 0.7f; // probability of cluster spawning
    [SerializeField] private int minCluster = 3;        // min coins in a cluster
    [SerializeField] private int maxCluster = 7;
    [SerializeField] private float stackOffset = 1.2f; // vertical spacing for stacked coins
    private void Start()
    {
        // SpawnCoins();
    }



    [ContextMenuItem("Reset to Default", "ResetHealth")]
    public int health = 100;

    private void ResetHealth()
    {
        for (int i = coinParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(coinParent.GetChild(i).gameObject);
        }
        SpawnCoins();
    }



    private void SpawnCoins()
    {
        if (terrainShape == null || coinPrefabs.Length == 0) return;

        var spline = terrainShape.spline;
        int pointCount = spline.GetPointCount();

        for (int i = 0; i < pointCount - 1; i++)
        {
            Vector3 start = spline.GetPosition(i);

            if (start.x < initialXValue)
                continue;

            Vector3 end = spline.GetPosition(i + 1);

            Vector3 direction = end - start;
            float slopeAngle = Vector2.Angle(Vector2.right, direction.normalized);

            // ✅ Only upper/gentle slopes
            if (slopeAngle <= maxSlopeAngle)
            {
                float distance = Vector2.Distance(start, end);
                int coinSlots = Mathf.FloorToInt(distance / spacing);

                int index = 0;
                while (index < coinSlots)
                {
                    // 🎲 Roll dice to decide if a cluster should spawn
                    if (Random.value < spawnChance)
                    {
                        int clusterSize = Random.Range(minCluster, maxCluster + 1);

                        for (int j = 0; j < clusterSize && index < coinSlots; j++)
                        {
                            float t = (index + j) / (float)coinSlots;
                            Vector3 pos = Vector3.Lerp(start, end, t);
                            pos.x -= 3f;
                            pos.y += heightOffset;

                            // 🎲 Decide: normal coin OR stack
                            float roll = Random.value;

                            if (roll < 0.15f) // 15% chance → stack
                            {
                                // Bottom: 5 coin
                                GameObject c5 = Instantiate(coinPrefabs[0], pos, Quaternion.identity, coinParent);

                                // Middle: 25 coin
                                GameObject c25_0 = Instantiate(coinPrefabs[1], pos + Vector3.up * stackOffset, Quaternion.identity, coinParent);
                                GameObject c25_1 = Instantiate(coinPrefabs[1], pos + Vector3.up * stackOffset, Quaternion.identity, coinParent);
                                // Top: 50 coin
                                GameObject c50 = Instantiate(coinPrefabs[2], pos + Vector3.up * stackOffset * 2f, Quaternion.identity, coinParent);
                            }
                            else
                            {
                                // Just a single 5 coin
                                Instantiate(coinPrefabs[0], pos, Quaternion.identity, coinParent);
                            }
                        }

                        index += clusterSize;
                    }
                    else
                    {
                        int gap = Random.Range(2, 5);
                        index += gap;
                    }
                }
            }
        }
    }
}
