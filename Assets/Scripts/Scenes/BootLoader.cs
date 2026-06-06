using System.Collections;
using UnityEngine;

public class BootLoader : MonoBehaviour
{
    [SerializeField] private float bootDelay = 0.5f; // Optional: show logo/splash

    private void Start()
    {
        // Optional: Initialize other systems here
        InitializeManagers();

        // Load first real scene
        StartCoroutine(LoadFirstScene());
    }

    private void InitializeManagers()
    {
        var _ = AudioManager.Instance;
        var __ = SceneController.Instance;
    }

    private IEnumerator LoadFirstScene()
    {
        // Optional: Show splash screen, company logo, etc.
        yield return new WaitForSeconds(bootDelay);

        // Load main menu
        SceneController.Instance.LoadNextScene();
    }
}