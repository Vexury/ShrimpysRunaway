using System;
using System.Collections;
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

    [Header("Shop")]
    [SerializeField] private ShopScreen shopScreen;

    [Header("Global Leaderboard")]
    [SerializeField] private Transform leaderboardContainer;
    [SerializeField] private LeaderboardRow leaderboardRowPrefab;

    [Header("FakeUpgrade")]
    [SerializeField] private GameObject crownImage;
    [SerializeField] private TMP_Text fakeUpgradeTextA;
    [SerializeField] private TMP_Text fakeUpgradeTextB;

    [Header("Audio")]
    [SerializeField] private AudioClip menuMusicLoopClip;
    [SerializeField] private AudioClip menuAmbienceLoopClip;
    [SerializeField] private float audioFadeInDuration = 2f;

    private const float LeaderboardRefreshInterval = 10f;

    private string[] leaderboardNames = Array.Empty<string>();
    private bool leaderboardReady = false;
    private string originalName;
    private string pendingName;
    private Coroutine refreshCoroutine;

    private void Start()
    {
        if (bestDistanceLabel != null)
            bestDistanceLabel.text = $"{HighscoreManager.BestDistance:0} m";

        RefreshFakeUpgrade();

        pendingName = HighscoreManager.PlayerName;

        if (nameInput != null)
        {
            nameInput.text = pendingName;
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

        if (menuMusicLoopClip != null) AudioManager.Instance.FadeInMusic(menuMusicLoopClip, audioFadeInDuration);
        if (menuAmbienceLoopClip != null) AudioManager.Instance.FadeInAmbience(menuAmbienceLoopClip, audioFadeInDuration);
    }

    private void OnLeaderboardFetched(DreamLoService.Entry[] entries)
    {
        leaderboardNames = new string[entries.Length];
        for (int i = 0; i < entries.Length; i++)
            leaderboardNames[i] = entries[i].Name;

        leaderboardReady = true;
        PopulateLeaderboard(entries);
        SetPlayButtonState(nameInput != null ? nameInput.text : HighscoreManager.PlayerName);

        if (refreshCoroutine == null)
            refreshCoroutine = StartCoroutine(RefreshLoop());
    }

    private IEnumerator RefreshLoop()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(LeaderboardRefreshInterval);
            if (DreamLoService.Instance != null)
                DreamLoService.Instance.FetchLeaderboard(OnLeaderboardFetched);
        }
    }

    private void OnDestroy()
    {
        if (nameInput != null)
            nameInput.onValueChanged.RemoveListener(OnNameChanged);
        if (refreshCoroutine != null)
            StopCoroutine(refreshCoroutine);
    }

    private void OnNameChanged(string value)
    {
        pendingName = value;
        SetPlayButtonState(value);
    }

    private void SetPlayButtonState(string name)
    {
        if (!leaderboardReady) return;
        bool hasName = !string.IsNullOrWhiteSpace(name);
        bool isReserved = IsLeaderboardName(name);
        bool ready = hasName && !isReserved;
        //Debug.Log($"[MainMenu] SetPlayButtonState: name='{name}' hasName={hasName} isReserved={isReserved} ready={ready}");

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
        //Debug.Log($"[MainMenu] IsLeaderboardName: checking '{name}' against {leaderboardNames.Length} entries, savedName='{savedName}'");
        foreach (var entry in leaderboardNames)
        {
            if (!string.Equals(entry, name, StringComparison.OrdinalIgnoreCase)) continue;
            if (string.Equals(entry, savedName, StringComparison.OrdinalIgnoreCase)) continue;
            //Debug.Log($"[MainMenu] '{name}' is taken by leaderboard entry '{entry}'");
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

    public void OnPlayClicked()
    {
        HighscoreManager.PlayerName = pendingName;
        SceneController.Instance.LoadNextScene(fade: true);
    }

    public void OnShopClicked()
    {
        shopScreen.OnPurchase = RefreshFakeUpgrade;
        shopScreen.Show();
    }

    public void OnShopClosed()
    {
        shopScreen.Hide();
        RefreshFakeUpgrade();
    }

    private void RefreshFakeUpgrade()
    {
        bool hasFakeUpgrade = UpgradeManager.FakeUpgradeLevel > 0;
        if (crownImage != null) crownImage.SetActive(hasFakeUpgrade);
        if (fakeUpgradeTextA != null) fakeUpgradeTextA.text = hasFakeUpgrade ? "Thank you for playing!" : fakeUpgradeTextA.text;
        if (fakeUpgradeTextB != null) fakeUpgradeTextB.text = hasFakeUpgrade ? "Thank you for playing, Shrimpy is proud of you!" : fakeUpgradeTextB.text;
    }

    public void OnSettingsClicked() { }

    public void OnQuitClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
