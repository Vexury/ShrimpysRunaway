using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Button playButton;
    [SerializeField] private TMP_Text playButtonLabel;
    [SerializeField] private TMP_Text bestDistanceLabel;

    [Header("Global Leaderboard")]
    [SerializeField] private Transform leaderboardContainer;
    [SerializeField] private LeaderboardRow leaderboardRowPrefab;

    private string[] leaderboardNames = Array.Empty<string>();
    private bool leaderboardReady = false;
    private string originalName;

    private void Start()
    {
        if (bestDistanceLabel != null)
            bestDistanceLabel.text = $"{HighscoreManager.BestDistance:0} m";

        if (nameInput != null)
        {
            nameInput.text = HighscoreManager.PlayerName;
            nameInput.onValueChanged.AddListener(OnNameChanged);
        }

        originalName = HighscoreManager.PlayerName;

        if (playButton != null) playButton.interactable = false;
        if (playButtonLabel != null) playButtonLabel.text = "Loading...";

        if (DreamLoService.Instance != null)
            DreamLoService.Instance.FetchLeaderboard(OnLeaderboardFetched);
        else
        {
            leaderboardReady = true;
            SetPlayButtonState(HighscoreManager.PlayerName);
        }
    }

    private void OnLeaderboardFetched(DreamLoService.Entry[] entries)
    {
        leaderboardNames = new string[entries.Length];
        for (int i = 0; i < entries.Length; i++)
        {
            leaderboardNames[i] = entries[i].Name;
            Debug.Log($"[MainMenu] Leaderboard entry {i}: '{entries[i].Name}'");
        }

        leaderboardReady = true;
        Debug.Log($"[MainMenu] Leaderboard ready. {leaderboardNames.Length} names loaded.");
        PopulateLeaderboard(entries);
        SetPlayButtonState(nameInput != null ? nameInput.text : HighscoreManager.PlayerName);
    }

    private void OnDestroy()
    {
        if (nameInput != null)
            nameInput.onValueChanged.RemoveListener(OnNameChanged);
    }

    private void OnNameChanged(string value)
    {
        HighscoreManager.PlayerName = value;
        SetPlayButtonState(value);
    }

    private void SetPlayButtonState(string name)
    {
        if (!leaderboardReady) return;
        bool hasName = !string.IsNullOrWhiteSpace(name);
        bool isReserved = IsLeaderboardName(name);
        bool ready = hasName && !isReserved;
        Debug.Log($"[MainMenu] SetPlayButtonState: name='{name}' hasName={hasName} isReserved={isReserved} ready={ready}");

        if (playButton != null) playButton.interactable = ready;
        if (playButtonLabel != null)
        {
            if (!hasName)        playButtonLabel.text = "Enter name to Play";
            else if (isReserved) playButtonLabel.text = "Name taken";
            else                 playButtonLabel.text = "Play";
        }
    }

    private bool IsLeaderboardName(string name)
    {
        string savedName = originalName;
        Debug.Log($"[MainMenu] IsLeaderboardName: checking '{name}' against {leaderboardNames.Length} entries, savedName='{savedName}'");
        foreach (var entry in leaderboardNames)
        {
            if (!string.Equals(entry, name, StringComparison.OrdinalIgnoreCase)) continue;
            if (string.Equals(entry, savedName, StringComparison.OrdinalIgnoreCase)) continue;
            Debug.Log($"[MainMenu] '{name}' is taken by leaderboard entry '{entry}'");
            return true;
        }
        return false;
    }

    private void PopulateLeaderboard(DreamLoService.Entry[] entries)
    {
        if (leaderboardContainer == null || leaderboardRowPrefab == null) return;
        foreach (Transform child in leaderboardContainer) Destroy(child.gameObject);
        for (int i = 0; i < entries.Length; i++)
            Instantiate(leaderboardRowPrefab, leaderboardContainer).Populate(entries[i].Name, entries[i].Score, i + 1);
    }

    public void OnPlayClicked() => SceneController.Instance.LoadNextScene();

    public void OnSettingsClicked() { }

    public void OnQuitClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
