using System;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance { get; private set; }
    public static event Action OnWin;

    public float ElapsedTime { get; private set; }
    public bool IsRunning { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        IsRunning = true;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void Update()
    {
        if (IsRunning)
            ElapsedTime += Time.deltaTime;
    }

    public void Stop()
    {
        if (!IsRunning) return;
        IsRunning = false;
        OnWin?.Invoke();
        Time.timeScale = 0f;
    }

    public string FormattedTime()
    {
        int minutes      = (int)(ElapsedTime / 60f);
        int seconds      = (int)(ElapsedTime % 60f);
        int centiseconds = (int)((ElapsedTime * 100f) % 100f);
        return $"{minutes:00}:{seconds:00}.{centiseconds:00}";
    }
}
