using UnityEngine;

public class PauseManager : Singleton<PauseManager>
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;
    [SerializeField] private GameObject pauseScreen;

    [Header("Settings")]
    [SerializeField] private bool manageCursor = true;

    public event System.Action OnPaused;
    public event System.Action OnResumed;

    private bool isPaused = false;
    private float previousTimeScale = 1f;

    private void OnEnable()
    {
        inputReader.PauseEvent += TogglePause;
    }

    private void OnDisable()
    {
        inputReader.PauseEvent -= TogglePause;
    }

    public void TogglePause()
    {
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    public void PauseGame()
    {
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        isPaused = true;

        if (manageCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (pauseScreen != null)
            pauseScreen.SetActive(true);

        OnPaused?.Invoke();
    }

    public void ResumeGame()
    {
        Time.timeScale = previousTimeScale;
        isPaused = false;

        if (manageCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (pauseScreen != null)
            pauseScreen.SetActive(false);

        OnResumed?.Invoke();
    }

    public bool IsPaused() => isPaused;
}
