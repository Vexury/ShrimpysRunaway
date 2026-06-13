using System;
using System.Collections.Generic;
using UnityEngine;

public class TrackManager : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float initialSpeed = 6f;
    [SerializeField] private float acceleration = 1f;
    [SerializeField] private float maxSpeed = 20f;

    [Serializable]
    public class CollectibleEntry
    {
        public GameObject prefab;
        public float weight = 1f;
    }

    [Header("Collectibles")]
    [SerializeField] private CollectibleEntry[] collectiblePrefabs;
    [SerializeField][Range(0f, 1f)] private float collectibleChance = 0.5f;
    [SerializeField] private float collectibleY = 0.8f;
    [SerializeField] private float laneWidth = 2.5f;
    [SerializeField] private float spawnInterval = 8f;
    [SerializeField] private float lookaheadDistance = 60f;
    [SerializeField] private float despawnDistance = 10f;

    private readonly List<GameObject> activeCollectibles = new();
    private float furthestSpawnedZ;
    private float worldSpeed;

    public float WorldSpeed => worldSpeed;

    public void ResetSpeed() => worldSpeed = initialSpeed;

    private void Start()
    {
        worldSpeed = initialSpeed;
        furthestSpawnedZ = -spawnInterval;
        FillAhead();
    }

    private void Update()
    {
        worldSpeed = Mathf.MoveTowards(worldSpeed, maxSpeed, acceleration * Time.deltaTime);

        float scroll = worldSpeed * Time.deltaTime;
        furthestSpawnedZ -= scroll;
        ScrollCollectibles(scroll);
        FillAhead();
        DespawnBehind();
    }

    private void FillAhead()
    {
        while (furthestSpawnedZ < lookaheadDistance)
        {
            furthestSpawnedZ += spawnInterval;
            SpawnStrip(furthestSpawnedZ);
        }
    }

    private void ScrollCollectibles(float scroll)
    {
        foreach (GameObject c in activeCollectibles)
        {
            if (c == null) continue;
            Vector3 pos = c.transform.localPosition;
            pos.z -= scroll;
            c.transform.localPosition = pos;
        }
    }

    private void DespawnBehind()
    {
        for (int i = activeCollectibles.Count - 1; i >= 0; i--)
        {
            GameObject c = activeCollectibles[i];
            if (c == null || c.transform.localPosition.z < -despawnDistance)
            {
                if (c != null) Destroy(c);
                activeCollectibles.RemoveAt(i);
            }
        }
    }

    private void SpawnStrip(float centerZ)
    {
        if (collectiblePrefabs == null || collectiblePrefabs.Length == 0) return;

        for (int lane = 0; lane < 3; lane++)
        {
            if (UnityEngine.Random.value > collectibleChance) continue;

            float x = (lane - 1) * laneWidth;
            float zOffset = UnityEngine.Random.Range(-spawnInterval * 0.3f, spawnInterval * 0.3f);
            GameObject c = Instantiate(PickWeightedCollectible().prefab, transform);
            c.transform.localPosition = new Vector3(x, collectibleY, centerZ + zOffset);
            activeCollectibles.Add(c);
        }
    }

    private CollectibleEntry PickWeightedCollectible()
    {
        float total = 0f;
        foreach (var entry in collectiblePrefabs)
            total += entry.weight;

        float roll = UnityEngine.Random.Range(0f, total);
        float cumulative = 0f;
        foreach (var entry in collectiblePrefabs)
        {
            cumulative += entry.weight;
            if (roll < cumulative)
                return entry;
        }

        return collectiblePrefabs[collectiblePrefabs.Length - 1];
    }
}
