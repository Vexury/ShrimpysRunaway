#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class EditorPlayModeLoadFirstScene
{
    private const string PreviousSceneKey = "EditorPreviousScenePath";

    static EditorPlayModeLoadFirstScene()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            SessionState.SetString(PreviousSceneKey, EditorSceneManager.GetActiveScene().path);

            if (EditorBuildSettings.scenes.Length > 0)
            {
                var bootScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorBuildSettings.scenes[0].path);
                EditorSceneManager.playModeStartScene = bootScene;
            }
        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            EditorSceneManager.playModeStartScene = null;

            string previousPath = SessionState.GetString(PreviousSceneKey, string.Empty);
            if (!string.IsNullOrEmpty(previousPath))
            {
                EditorSceneManager.OpenScene(previousPath);
                SessionState.EraseString(PreviousSceneKey);
            }
        }
    }
}
#endif
