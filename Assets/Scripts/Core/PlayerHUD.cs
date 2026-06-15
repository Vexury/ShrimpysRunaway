using TMPro;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text coinLabel;
    [SerializeField] private TMP_Text distanceLabel;
    [SerializeField] private TMP_Text streakLabel;
    [SerializeField] private TMP_Text streakNameLabel;
    [SerializeField] private TrackManager trackManager;

    private void OnEnable()
    {
        Collectible.OnCollected += OnCollected;
        StreakManager.OnRankChanged += OnRankChanged;
    }

    private void OnDisable()
    {
        Collectible.OnCollected -= OnCollected;
        StreakManager.OnRankChanged -= OnRankChanged;
    }

    private void Start()
    {
        if (coinLabel != null) coinLabel.text = "0";
    }

    private void Update()
    {
        if (trackManager != null && distanceLabel != null)
            distanceLabel.text = $"{trackManager.DistanceTravelled:0} m";
    }

    private void OnCollected(CollectibleType type, int count)
    {
        if (type == CollectibleType.Coin && coinLabel != null)
            coinLabel.text = count.ToString();
    }

    private void OnRankChanged(string rank, string name)
    {
        if (streakLabel != null)     streakLabel.text     = rank;
        if (streakNameLabel != null) streakNameLabel.text = name;
    }
}
