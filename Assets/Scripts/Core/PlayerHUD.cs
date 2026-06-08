using TMPro;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private TMP_Text stateLabel;
    [SerializeField] private TMP_Text timerLabel;
    [SerializeField] private TMP_Text collectiblesLabel;
    [SerializeField] private TMP_Text winStatsLabel;
    [SerializeField] private RookController controller;

    private bool won;

    private void OnEnable()
    {
        GameTimer.OnWin        += OnWin;
        Collectible.OnCollected += OnCollected;
    }

    private void OnDisable()
    {
        GameTimer.OnWin        -= OnWin;
        Collectible.OnCollected -= OnCollected;
    }

    private void Start()
    {
        collectiblesLabel.text = "0";
        winStatsLabel.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (GameTimer.Instance != null)
            timerLabel.text = GameTimer.Instance.FormattedTime();

        if (won) return;

        string movement;
        if (controller.IsCrouching)
            movement = "Crouching";
        else if (controller.IsSprinting && controller.NormalizedSpeed > 0.05f)
            movement = "Sprinting";
        else if (controller.NormalizedSpeed > 0.05f)
            movement = "Walking";
        else
            movement = "Idle";

        stateLabel.text = !controller.IsGrounded ? movement + " + Jumping" : movement;
    }

    private void OnWin()
    {
        won = true;
        stateLabel.text = "You Win!";

        string time        = GameTimer.Instance != null ? GameTimer.Instance.FormattedTime() : "--";
        string collectibles = Collectible.CollectedCount.ToString();
        string detections  = EnemyNavMeshChaser.DetectionCount.ToString();
        winStatsLabel.gameObject.SetActive(true);
        winStatsLabel.text = $"Time: {time}\nCollectibles: {collectibles}\nDetected: {detections}x";
    }

    private void OnCollected(int count)
    {
        collectiblesLabel.text = $"{count}";
    }
}
