using System.Collections.Generic;
using UnityEngine;

public class SandTrail : MonoBehaviour
{
    [SerializeField] private TrackManager trackManager;
    [SerializeField] private RollerController roller;
    [SerializeField] private GameObject markPrefab;
    [SerializeField] private float spawnEvery = 0.4f;
    [SerializeField] private float groundY = 0.01f;
    [SerializeField] private int maxMarks = 20;

    private readonly List<SandTrailMark> marks = new();
    private float accumulator;

    private void Update()
    {
        if (!roller.IsGrounded) return;

        accumulator += trackManager.WorldSpeed * Time.deltaTime;
        if (accumulator < spawnEvery) return;

        accumulator -= spawnEvery;

        GameObject obj = Instantiate(markPrefab);
        obj.transform.position = new Vector3(transform.position.x, groundY, 0f);
        SandTrailMark mark = obj.GetComponent<SandTrailMark>();
        mark.Init(trackManager);
        marks.Add(mark);

        if (marks.Count > maxMarks)
        {
            Destroy(marks[0].gameObject);
            marks.RemoveAt(0);
        }

        UpdateAlphas();
    }

    private void UpdateAlphas()
    {
        for (int i = 0; i < marks.Count; i++)
        {
            float t = marks.Count > 1 ? (float)i / (marks.Count - 1) : 1f;
            marks[i].SetAlpha(t);
        }
    }
}
