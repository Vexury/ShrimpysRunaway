using System.Collections.Generic;
using UnityEngine;

public class PalmTreeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject palmPrefab;
    [SerializeField] private TrackManager trackManager;
    [SerializeField] private float sideXMin = 6.5f;
    [SerializeField] private float sideXMax = 12f;

    private const float StripInterval = 8f;
    private const float LookaheadDistance = 60f;
    private const float DespawnDistance = 10f;

    private readonly List<GameObject> activeTrees = new();
    private readonly Queue<GameObject> _pool = new();
    private float furthestSpawnedZ;

    private GameObject GetTree()
    {
        if (_pool.Count > 0)
        {
            GameObject t = _pool.Dequeue();
            t.SetActive(true);
            return t;
        }
        return Instantiate(palmPrefab, transform);
    }

    private void ReturnTree(GameObject tree)
    {
        tree.SetActive(false);
        _pool.Enqueue(tree);
    }

    private void Start()
    {
        furthestSpawnedZ = -StripInterval;
        FillAhead();
    }

    private void Update()
    {
        float scroll = trackManager.WorldSpeed * Time.deltaTime;
        furthestSpawnedZ -= scroll;
        ScrollTrees(scroll);
        FillAhead();
        DespawnBehind();
    }

    private void FillAhead()
    {
        while (furthestSpawnedZ < LookaheadDistance)
        {
            furthestSpawnedZ += StripInterval;
            SpawnStrip(furthestSpawnedZ);
        }
    }

    private void SpawnStrip(float centerZ)
    {
        int count = Random.Range(1, 3);
        for (int side = -1; side <= 1; side += 2)
        {
            for (int i = 0; i < count; i++)
            {
                float x = side * Random.Range(sideXMin, sideXMax);
                float z = centerZ + Random.Range(-StripInterval * 0.3f, StripInterval * 0.3f);

                GameObject tree = GetTree();
                tree.transform.position = new Vector3(x, palmPrefab.transform.position.y, z);
                tree.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                tree.transform.localScale = Vector3.one * Random.Range(0.8f, 1.4f);
                activeTrees.Add(tree);
            }
        }
    }

    private void ScrollTrees(float scroll)
    {
        foreach (GameObject tree in activeTrees)
        {
            if (tree == null) continue;
            tree.transform.position += Vector3.back * scroll;
        }
    }

    private void DespawnBehind()
    {
        for (int i = activeTrees.Count - 1; i >= 0; i--)
        {
            GameObject tree = activeTrees[i];
            if (tree == null || tree.transform.position.z < -DespawnDistance)
            {
                if (tree != null) ReturnTree(tree);
                activeTrees.RemoveAt(i);
            }
        }
    }
}
