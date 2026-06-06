using UnityEngine;

public static class SaveSystem
{
    public static void Save<T>(string key, T data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();
    }

    public static T Load<T>(string key, T defaultValue = default)
    {
        if (!PlayerPrefs.HasKey(key))
            return defaultValue;

        try
        {
            return JsonUtility.FromJson<T>(PlayerPrefs.GetString(key));
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"SaveSystem: Failed to load '{key}': {e.Message}");
            return defaultValue;
        }
    }
}
