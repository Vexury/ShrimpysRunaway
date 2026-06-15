using System;
using UnityEngine;

public class StreakManager : MonoBehaviour
{
    private static readonly string[] Ranks = { "F", "E", "D", "C", "B", "A", "S", "SS" };

    [SerializeField] private float decayDelay = 5f;
    [SerializeField] private string[] rankNames = new string[8];

    public static event Action<string, string> OnRankChanged;

    public string CurrentRank => Ranks[streakIndex];
    public string CurrentName => rankNames != null && streakIndex < rankNames.Length ? rankNames[streakIndex] : "";

    private int streakIndex;
    private float decayTimer;

    private void Start() => OnRankChanged?.Invoke(CurrentRank, CurrentName);

    private void OnEnable()
    {
        SwimRingObstacle.OnPassedThrough += OnPassedThrough;
        RollerController.OnObstacleHit   += OnObstacleHit;
    }

    private void OnDisable()
    {
        SwimRingObstacle.OnPassedThrough -= OnPassedThrough;
        RollerController.OnObstacleHit   -= OnObstacleHit;
    }

    private void OnPassedThrough()
    {
        streakIndex = Mathf.Min(streakIndex + 1, Ranks.Length - 1);
        decayTimer = decayDelay;
        OnRankChanged?.Invoke(CurrentRank, CurrentName);
    }

    private void OnObstacleHit()
    {
        streakIndex = Mathf.Max(streakIndex - 1, 0);
        decayTimer = decayDelay;
        OnRankChanged?.Invoke(CurrentRank, CurrentName);
    }

    private void Update()
    {
        if (streakIndex == 0) return;
        decayTimer -= Time.deltaTime;
        if (decayTimer <= 0f)
        {
            streakIndex = Mathf.Max(streakIndex - 1, 0);
            decayTimer = decayDelay;
            OnRankChanged?.Invoke(CurrentRank, CurrentName);
        }
    }
}
