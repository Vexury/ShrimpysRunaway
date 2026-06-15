using UnityEngine;

public class LevelProgression : MonoBehaviour
{
    [SerializeField] private TrackManager trackManager;
    [SerializeField] private float gracePeriod = 3f;

    [Header("Obstacle Chance")]
    [SerializeField] private float obstacleChanceMin = 0.1f;
    [SerializeField] private float obstacleChanceMax = 0.6f;
    [SerializeField] private AnimationCurve obstacleChanceCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [Header("Coin Chance")]
    [SerializeField] private float coinChanceMin = 0.25f;
    [SerializeField] private float coinChanceMax = 0.7f;
    [SerializeField] private AnimationCurve coinChanceCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [Header("Special Collectible Chance")]
    [SerializeField] private float specialChanceMin = 0.05f;
    [SerializeField] private float specialChanceMax = 0.2f;
    [SerializeField] private AnimationCurve specialChanceCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [Header("Spawn Interval (higher = sparser)")]
    [SerializeField] private float spawnIntervalMin = 5f;
    [SerializeField] private float spawnIntervalMax = 14f;
    [SerializeField] private AnimationCurve spawnIntervalCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

    private float elapsedTime;

    private void Awake()
    {
        float t = 0f;
        trackManager.ObstacleChance = Mathf.Lerp(obstacleChanceMin, obstacleChanceMax, obstacleChanceCurve.Evaluate(t));
        trackManager.CoinChance     = Mathf.Lerp(coinChanceMin,     coinChanceMax,     coinChanceCurve.Evaluate(t));
        trackManager.SpecialChance  = Mathf.Lerp(specialChanceMin,  specialChanceMax,  specialChanceCurve.Evaluate(t));
        trackManager.SpawnInterval  = Mathf.Lerp(spawnIntervalMin,  spawnIntervalMax,  spawnIntervalCurve.Evaluate(t));
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime < gracePeriod)
        {
            trackManager.SpawningEnabled = false;
            return;
        }

        float t = Mathf.InverseLerp(trackManager.InitialSpeed, trackManager.MaxSpeed, trackManager.WorldSpeed);
        trackManager.SpawningEnabled = true;
        trackManager.ObstacleChance = Mathf.Lerp(obstacleChanceMin, obstacleChanceMax, obstacleChanceCurve.Evaluate(t));
        trackManager.CoinChance     = Mathf.Lerp(coinChanceMin,     coinChanceMax,     coinChanceCurve.Evaluate(t));
        trackManager.SpecialChance  = Mathf.Lerp(specialChanceMin,  specialChanceMax,  specialChanceCurve.Evaluate(t));
        trackManager.SpawnInterval  = Mathf.Lerp(spawnIntervalMin,  spawnIntervalMax,  spawnIntervalCurve.Evaluate(t));
    }
}
