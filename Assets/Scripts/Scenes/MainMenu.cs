using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void OnPlayClicked()
    {
        SceneController.Instance.LoadNextScene();
    }

    public void OnSettingsClicked()
    {
        // Open settings panel
    }

    public void OnQuitClicked()
    {
        Debug.Log("Quitting game...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}