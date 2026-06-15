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
        public float yOffset = 0f;
    }

    [Header("Coins")]
    [SerializeField] private CollectibleEntry[] coinPrefabs;
    [SerializeField] private int coinLineMin = 3;
    [SerializeField] private int coinLineMax = 5;

    [Header("Special Collectibles")]
    [SerializeField] private CollectibleEntry[] specialPrefabs;
    [SerializeField] private float specialCooldown = 10f;

    [Header("Obstacles")]
    [SerializeField] private CollectibleEntry[] obstaclePrefabs;

    [Header("Track")]
    [SerializeField] private float laneWidth = 2.5f;
    [SerializeField] private float lookaheadDistance = 60f;
    [SerializeField] private float despawnDistance = 10f;
    [SerializeField] private float initialClearDistance = 20f;

    private readonly List<GameObject> activeCollectibles = new();
    private readonly List<GameObject> activeObstacles = new();
    private readonly Dictionary<GameObject, Queue<GameObject>> _pools = new();
    private readonly Dictionary<GameObject, GameObject> _prefabLookup = new();
    private float furthestSpawnedZ;
    private float worldSpeed;
    private float specialTimer;

    public float WorldSpeed => worldSpeed;
    public float DistanceTravelled { get; private set; }
    public float InitialSpeed => initialSpeed;
    public float MaxSpeed => maxSpeed;
    public bool SpawningEnabled { get; set; } = true;
    public float ObstacleChance { get; set; } = 0f;
    public float CoinChance { get; set; } = 0f;
    public float SpecialChance { get; set; } = 0f;
    public float SpawnInterval { get; set; } = 8f;

    public void ResetSpeed() => worldSpeed *= 0.9f;

    private GameObject GetPooled(GameObject prefab)
    {
        if (_pools.TryGetValue(prefab, out Queue<GameObject> queue) && queue.Count > 0)
        {
            GameObject obj = queue.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        return Instantiate(prefab, transform);
    }

    private void ReturnPooled(GameObject instance)
    {
        if (!_prefabLookup.TryGetValue(instance, out GameObject prefab)) return;
        _prefabLookup.Remove(instance);
        instance.SetActive(false);
        Collectible col = instance.GetComponent<Collectible>();
        if (col != null) col.MagnetPulled = false;
        if (!_pools.TryGetValue(prefab, out Queue<GameObject> queue))
        {
            queue = new Queue<GameObject>();
            _pools[prefab] = queue;
        }
        queue.Enqueue(instance);
    }

    private void Start()
    {
        worldSpeed = initialSpeed;
        furthestSpawnedZ = initialClearDistance;
        FillAhead();
    }

    private void Update()
    {
        worldSpeed = Mathf.MoveTowards(worldSpeed, maxSpeed, acceleration * Time.deltaTime);
        DistanceTravelled += worldSpeed * Time.deltaTime;
        specialTimer += Time.deltaTime;
        float scroll = worldSpeed * Time.deltaTime;
        furthestSpawnedZ -= scroll;
        ScrollCollectibles(scroll);
        FillAhead();
        DespawnBehind();
    }

    private void FillAhead()
    {
        if (!SpawningEnabled) return;
        while (furthestSpawnedZ < lookaheadDistance)
        {
            furthestSpawnedZ += SpawnInterval;
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

        foreach (GameObject o in activeObstacles)
        {
            if (o == null) continue;
            Vector3 pos = o.transform.localPosition;
            pos.z -= scroll;
            o.transform.localPosition = pos;
        }
    }

    private void DespawnBehind()
    {
        for (int i = activeCollectibles.Count - 1; i >= 0; i--)
        {
            GameObject c = activeCollectibles[i];
            if (c == null || c.transform.localPosition.z < -despawnDistance)
            {
                if (c != null) ReturnPooled(c);
                activeCollectibles.RemoveAt(i);
            }
        }

        for (int i = activeObstacles.Count - 1; i >= 0; i--)
        {
            GameObject o = activeObstacles[i];
            if (o == null || o.transform.localPosition.z < -despawnDistance)
            {
                if (o != null) ReturnPooled(o);
                activeObstacles.RemoveAt(i);
            }
        }
    }

    private void SpawnStrip(float centerZ)
    {
        bool[] laneBlocked = new bool[5];

        // Obstacles
        if (obstaclePrefabs != null && obstaclePrefabs.Length > 0)
        {
            for (int lane = 0; lane < 5; lane++)
            {
                if (laneBlocked[lane]) continue;
                if (UnityEngine.Random.value > ObstacleChance) continue;

                float x = (lane - 2) * laneWidth;
                float zOffset = UnityEngine.Random.Range(-SpawnInterval * 0.3f, SpawnInterval * 0.3f);
                CollectibleEntry entry = PickWeighted(obstaclePrefabs);
                GameObject o = GetPooled(entry.prefab);
                _prefabLookup[o] = entry.prefab;
                o.transform.localPosition = new Vector3(x, entry.yOffset, centerZ + zOffset);
                activeObstacles.Add(o);
                laneBlocked[lane] = true;
                if (lane + 1 < 5) laneBlocked[lane + 1] = true;
            }
        }

        // Coin line
        if (coinPrefabs != null && coinPrefabs.Length > 0 && UnityEngine.Random.value <= CoinChance)
        {
            int coinLane = PickFreeLane(laneBlocked);
            if (coinLane >= 0)
            {
                laneBlocked[coinLane] = true;
                int count = UnityEngine.Random.Range(coinLineMin, coinLineMax + 1);
                float span = SpawnInterval * 0.8f;
                float startZ = centerZ - span * 0.5f;
                float step = count > 1 ? span / (count - 1) : 0f;
                CollectibleEntry coinEntry = PickWeighted(coinPrefabs);
                float x = (coinLane - 2) * laneWidth;
                for (int i = 0; i < count; i++)
                {
                    GameObject c = GetPooled(coinEntry.prefab);
                    _prefabLookup[c] = coinEntry.prefab;
                    c.transform.localPosition = new Vector3(x, coinEntry.yOffset, startZ + i * step);
                    activeCollectibles.Add(c);
                }
            }
        }

        // Special collectibles
        if (specialPrefabs != null && specialPrefabs.Length > 0 && specialTimer >= specialCooldown)
        {
            for (int lane = 0; lane < 5; lane++)
            {
                if (laneBlocked[lane]) continue;
                if (UnityEngine.Random.value > SpecialChance) continue;

                float x = (lane - 2) * laneWidth;
                float zOffset = UnityEngine.Random.Range(-SpawnInterval * 0.3f, SpawnInterval * 0.3f);
                CollectibleEntry entry = PickWeighted(specialPrefabs);
                GameObject c = GetPooled(entry.prefab);
                _prefabLookup[c] = entry.prefab;
                c.transform.localPosition = new Vector3(x, entry.yOffset, centerZ + zOffset);
                activeCollectibles.Add(c);
                specialTimer = 0f;
                break;
            }
        }
    }

    private int PickFreeLane(bool[] blocked)
    {
        int free = 0;
        for (int i = 0; i < blocked.Length; i++)
            if (!blocked[i]) free++;
        if (free == 0) return -1;

        int pick = UnityEngine.Random.Range(0, free);
        int found = 0;
        for (int i = 0; i < blocked.Length; i++)
        {
            if (!blocked[i])
            {
                if (found == pick) return i;
                found++;
            }
        }
        return -1;
    }

    private CollectibleEntry PickWeighted(CollectibleEntry[] entries)
    {
        float total = 0f;
        foreach (var entry in entries)
            total += entry.weight;

        float roll = UnityEngine.Random.Range(0f, total);
        float cumulative = 0f;
        foreach (var entry in entries)
        {
            cumulative += entry.weight;
            if (roll < cumulative)
                return entry;
        }

        return entries[entries.Length - 1];
    }

    private void OnDrawGizmos()
    {
        float zNear = -despawnDistance;
        float zFar = lookaheadDistance;
        float xMin = -2.5f * laneWidth;
        float xMax =  2.5f * laneWidth;

        // Lane separators
        Gizmos.color = new Color(1f, 1f, 0f, 0.25f);
        for (int i = 0; i <= 5; i++)
        {
            float x = (i - 2) * laneWidth - laneWidth * 0.5f;
            Gizmos.DrawLine(transform.TransformPoint(x, 0f, zNear),
                            transform.TransformPoint(x, 0f, zFar));
        }

        // Lane centre lines
        Gizmos.color = new Color(1f, 1f, 0f, 0.6f);
        for (int lane = 0; lane < 5; lane++)
        {
            float x = (lane - 2) * laneWidth;
            Gizmos.DrawLine(transform.TransformPoint(x, 0f, zNear),
                            transform.TransformPoint(x, 0f, zFar));
        }

        // Lookahead boundary
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.TransformPoint(xMin, 0f, zFar),
                        transform.TransformPoint(xMax, 0f, zFar));

        // Despawn boundary
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.TransformPoint(xMin, 0f, zNear),
                        transform.TransformPoint(xMax, 0f, zNear));

        // Strip lines
        float interval = Application.isPlaying ? SpawnInterval : 8f;
        if (interval <= 0f) return;
        Gizmos.color = new Color(0f, 0.8f, 1f, 0.5f);
        if (Application.isPlaying)
        {
            float z = furthestSpawnedZ;
            while (z > zNear)
            {
                Gizmos.DrawLine(transform.TransformPoint(xMin, 0f, z),
                                transform.TransformPoint(xMax, 0f, z));
                z -= interval;
            }
        }
        else
        {
            for (float z = interval; z < zFar; z += interval)
            {
                Gizmos.DrawLine(transform.TransformPoint(xMin, 0f, z),
                                transform.TransformPoint(xMax, 0f, z));
            }
        }
    }
}
