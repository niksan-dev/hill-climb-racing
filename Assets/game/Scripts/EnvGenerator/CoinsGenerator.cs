using UnityEngine;
using UnityEngine.U2D; // For SpriteShapeController
using System.Collections.Generic;

public class CoinsGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteShapeController terrainShape;
    [SerializeField] private GameObject[] coinPrefabs;
    public GameObject fuelPrefab;
    public GameObject gemPrefab;
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
    public float fuelSpawnInterval = 50f;   // distance between fuel tanks
    public float gemSpawnInterval = 35f;   // distance between fuel tanks
    private float lastFuelX = 0f;

    private float lastGemX = 0;
    private List<Vector3> occupiedPositions = new List<Vector3>();

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
        occupiedPositions.Clear();
        lastFuelX = 0f;
        lastGemX = 0;
        SpawnCoinsAndFuel();
    }

    private void SpawnCoinsAndFuel()
    {
        if (terrainShape == null || coinPrefabs.Length == 0) return;

        var spline = terrainShape.spline;
        int pointCount = spline.GetPointCount();

        for (int i = 0; i < pointCount - 1; i++)
        {
            Vector3 start = spline.GetPosition(i);
            if (start.x < initialXValue) continue;

            Vector3 end = spline.GetPosition(i + 1);
            Vector3 direction = end - start;
            float slopeAngle = Vector2.Angle(Vector2.right, direction.normalized);

            // ✅ Only gentle slopes
            if (slopeAngle <= maxSlopeAngle)
            {
                float distance = Vector2.Distance(start, end);
                int coinSlots = Mathf.FloorToInt(distance / spacing);

                int index = 0;

                while (index < coinSlots)
                {
                    if (Random.value < spawnChance)
                    {
                        int clusterSize = Random.Range(minCluster, maxCluster + 1);


                        for (int j = 0; j < clusterSize && index < coinSlots; j++)
                        {
                            float t = (index + j) / (float)coinSlots;
                            Vector3 pos = Vector3.Lerp(start, end, t);
                            pos.x -= 3f;
                            pos.y += heightOffset;

                            // --- FUEL CHECK ---
                            if (pos.x - lastFuelX >= fuelSpawnInterval)
                            {
                                Vector3 fuelPos = pos;
                                if (!IsOccupied(fuelPos))
                                {
                                    GameObject fuel = Instantiate(fuelPrefab, fuelPos, Quaternion.identity, coinParent);
                                    occupiedPositions.Add(fuel.transform.position);
                                    lastFuelX = pos.x;
                                }
                            }
                            else if (pos.x - lastGemX >= gemSpawnInterval)
                            {
                                Vector3 gemPos = pos;
                                if (!IsOccupied(gemPos))
                                {
                                    GameObject gem = Instantiate(gemPrefab, gemPos, Quaternion.identity, coinParent);
                                    occupiedPositions.Add(gem.transform.position);
                                    lastGemX = pos.x;
                                }
                            }

                            // --- COINS ---
                            if (!IsOccupied(pos))
                            {
                                float roll = Random.value;
                                if (roll < 0.15f) // 15% chance → stack
                                {
                                    GameObject c5 = Instantiate(coinPrefabs[0], pos, Quaternion.identity, coinParent);
                                    occupiedPositions.Add(c5.transform.position);

                                    GameObject c25 = Instantiate(coinPrefabs[1], pos + Vector3.up * stackOffset, Quaternion.identity, coinParent);
                                    occupiedPositions.Add(c25.transform.position);

                                    GameObject c50 = Instantiate(coinPrefabs[2], pos + Vector3.up * stackOffset * 2f, Quaternion.identity, coinParent);
                                    occupiedPositions.Add(c50.transform.position);
                                }
                                else
                                {

                                    GameObject coin = Instantiate(coinPrefabs[0], pos, Quaternion.identity, coinParent);
                                    occupiedPositions.Add(coin.transform.position);

                                }
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

    private bool IsOccupied(Vector3 pos)
    {
        foreach (Vector3 p in occupiedPositions)
        {
            if (Vector2.Distance(p, pos) < 0.7f) // 2f = min spacing between coin/fuel
                return true;
        }
        return false;
    }
}