using TMPro;
using UnityEngine;

public class LibraryTester : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private ObjectPool bulletPool;

    [Header("Test Audio")]
    [SerializeField] private AudioClip testSound;
    [SerializeField] private AudioClip testMusic;

    [System.Serializable]
    public class TestData
    {
        public int score = 0;
        public string playerName = "Test Player";
        public float health = 100f;
    }

    private TestData currentData = new TestData();

    private void Start()
    {
        UpdateStatus("System Ready! Click buttons to test.");
    }

    // Test Audio
    public void TestPlaySound()
    {
        AudioManager.Instance.PlaySFX(testSound);
        UpdateStatus("Played SFX!");
    }

    public void TestPlayMusic()
    {
        AudioManager.Instance.PlayMusic(testMusic);
        UpdateStatus("Playing Music!");
    }

    // Test Save System
    public void TestSave()
    {
        currentData.score++;
        SaveSystem.Save("TestData", currentData);
        UpdateStatus($"Saved! Score: {currentData.score}");
    }

    public void TestLoad()
    {
        currentData = SaveSystem.Load("TestData", new TestData());
        UpdateStatus($"Loaded! Score: {currentData.score}");
    }

    // Test Object Pool
    public void TestSpawnBullet()
    {
        GameObject bullet = bulletPool.Get(2f); // Auto-return after 2 seconds
        if (bullet != null)
        {
            bullet.transform.position = Random.insideUnitSphere * 3f;
            UpdateStatus("Spawned bullet from pool!");
        }
    }

    // Test Scene Controller
    public void TestReloadScene()
    {
        UpdateStatus("Reloading scene...");
        SceneController.Instance.ReloadCurrentScene();
    }

    // Helper
    private void UpdateStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = $"Status:\n{message}";
        }
        Debug.Log(message);
    }
}