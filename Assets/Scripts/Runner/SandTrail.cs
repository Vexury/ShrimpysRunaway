using System.Collections.Generic;
using UnityEngine;

public class SandTrail : MonoBehaviour
{
    [SerializeField] private TrackManager trackManager;
    [SerializeField] private RollerController roller;
    [SerializeField] private GameObject markPrefab;
    [SerializeField] private GameObject sandVFX;
    [SerializeField] private float spawnEvery = 0.4f;
    [SerializeField] private float groundY = 0.01f;
    [SerializeField] private int maxMarks = 20;

    private readonly List<SandTrailMark> marks = new();
    private readonly Queue<SandTrailMark> _pool = new();
    private float accumulator;

    private void Start()
    {
        for (int i = 0; i < maxMarks; i++)
        {
            GameObject obj = Instantiate(markPrefab);
            SandTrailMark mark = obj.GetComponent<SandTrailMark>();
            mark.Init(trackManager);
            obj.SetActive(false);
            _pool.Enqueue(mark);
        }
    }

    private void Update()
    {
        if (sandVFX != null) sandVFX.SetActive(roller.IsGrounded);

        if (!roller.IsGrounded) return;

        accumulator += trackManager.WorldSpeed * Time.deltaTime;
        if (accumulator < spawnEvery) return;

        accumulator -= spawnEvery;

        if (marks.Count >= maxMarks)
        {
            SandTrailMark oldest = marks[0];
            marks.RemoveAt(0);
            oldest.gameObject.SetActive(false);
            _pool.Enqueue(oldest);
        }

        SandTrailMark mark = _pool.Dequeue();
        mark.transform.position = new Vector3(transform.position.x, groundY, 0f);
        mark.gameObject.SetActive(true);
        marks.Add(mark);
    }
}
