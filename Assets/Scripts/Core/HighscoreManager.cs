using UnityEngine;

public static class HighscoreManager
{
    private const string KeyBestDistance = "BestDistance";
    private const string KeyPlayerName   = "PlayerName";

    public static float BestDistance
    {
        get => PlayerPrefs.GetFloat(KeyBestDistance, 0f);
        private set { PlayerPrefs.SetFloat(KeyBestDistance, value); PlayerPrefs.Save(); }
    }

    public static string PlayerName
    {
        get => PlayerPrefs.GetString(KeyPlayerName, "");
        set
        {
            string sanitized = value.Trim();
            if (sanitized.Length > 10) sanitized = sanitized[..10];
            PlayerPrefs.SetString(KeyPlayerName, sanitized);
            PlayerPrefs.Save();
        }
    }

    public static bool SubmitRun(float distanceMetres)
    {
        bool newBest = distanceMetres > BestDistance;
        if (newBest) BestDistance = distanceMetres;
        return newBest;
    }
}
