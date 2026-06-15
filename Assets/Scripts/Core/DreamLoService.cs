using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DreamLoService : Singleton<DreamLoService>
{
    [Header("Leaderboard Keys")]
    [SerializeField] private string publicKey;
    [SerializeField] private string privateKey;
    [SerializeField] private int leaderboardCount = 10;

    protected override void Awake()
    {
        base.Awake();
        if (string.IsNullOrWhiteSpace(publicKey))  Debug.LogWarning("[DreamLo] Public key is not set.");
        if (string.IsNullOrWhiteSpace(privateKey)) Debug.LogWarning("[DreamLo] Private key is not set.");
    }

    public struct Entry
    {
        public string Name;
        public string Score;
    }

    public void Submit(string playerName, float distanceMetres)
    {
        StartCoroutine(SubmitCoroutine(playerName, Mathf.RoundToInt(distanceMetres).ToString()));
    }

    public void FetchLeaderboard(Action<Entry[]> onDone, float delay = 0f)
    {
        StartCoroutine(FetchCoroutine(onDone, delay));
    }

    private IEnumerator SubmitCoroutine(string playerName, string score)
    {
        string encodedName = UnityWebRequest.EscapeURL(playerName);
        string url = $"https://dreamlo.com/lb/{privateKey}/add-or-update/{encodedName}/{score}";
        using var req = UnityWebRequest.Get(url);
        yield return req.SendWebRequest();
    }

    private IEnumerator FetchCoroutine(Action<Entry[]> callback, float delay)
    {
        if (delay > 0f) yield return new WaitForSecondsRealtime(delay);
        string url = $"https://dreamlo.com/lb/{publicKey}/pipe/{leaderboardCount}";
        using var req = UnityWebRequest.Get(url);
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
            callback?.Invoke(ParsePipe(req.downloadHandler.text));
        else
            callback?.Invoke(Array.Empty<Entry>());
    }

    private Entry[] ParsePipe(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return Array.Empty<Entry>();
        var lines = raw.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var list = new List<Entry>(lines.Length);
        foreach (var line in lines)
        {
            var parts = line.Split('|');
            if (parts.Length >= 2)
                list.Add(new Entry { Name = parts[0], Score = parts[1] + " m" });
        }
        return list.ToArray();
    }
}
