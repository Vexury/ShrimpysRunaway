using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>
{
    private bool isLoading = false;

    public void LoadScene(string sceneName)
    {
        if (!isLoading)
            SceneManager.LoadScene(sceneName);
    }

    public void LoadScene(int sceneIndex)
    {
        if (!isLoading)
            SceneManager.LoadScene(sceneIndex);
    }

    public void LoadNextScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            LoadScene(nextSceneIndex);
        else
            Debug.LogWarning("No next scene! This is the last scene in Build Settings.");
    }

    public void LoadSceneAsync(string sceneName)
    {
        if (!isLoading)
            StartCoroutine(LoadSceneAsyncCoroutine(sceneName));
    }

    public void ReloadCurrentScene()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator LoadSceneAsyncCoroutine(string sceneName)
    {
        isLoading = true;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
            yield return null;

        isLoading = false;
    }
}
