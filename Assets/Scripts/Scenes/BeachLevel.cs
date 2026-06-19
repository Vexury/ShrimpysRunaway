using UnityEngine;

public class BeachLevel : MonoBehaviour
{
    [SerializeField] private AudioClip music;
    [SerializeField] private AudioClip musicLoop;
    [SerializeField] private double musicLoopOffset = 0.0;
    [SerializeField] private AudioClip ambience;

    [Header("Music Pitch by Speed")]
    [SerializeField] private TrackManager trackManager;
    [SerializeField] private float pitchMin = 1f;
    [SerializeField] private float pitchMax = 1.05f;
    [SerializeField] private AnimationCurve pitchCurve = new AnimationCurve(
        new Keyframe(0f, 0f), new Keyframe(0.5f, 0.8f), new Keyframe(1f, 1f));

    [Header("Death Fade")]
    [SerializeField] private float deathFadeDuration = 1f;
    [SerializeField] private AudioClip deathClip;

    private bool dead;

    private void OnEnable()  => PlayerHealth.OnDeath += OnDeath;
    private void OnDisable() => PlayerHealth.OnDeath -= OnDeath;

    private void Start()
    {
        if (AudioManager.Instance == null) return;

        if (music != null)
        {
            if (musicLoop != null)
                AudioManager.Instance.PlayMusicWithIntro(music, musicLoop, musicLoopOffset);
            else
                AudioManager.Instance.PlayMusic(music);
        }
        if (ambience != null) AudioManager.Instance.PlayAmbience(ambience);
    }

    private void Update()
    {
        if (dead || trackManager == null || AudioManager.Instance == null) return;

        float t = Mathf.InverseLerp(trackManager.InitialSpeed, trackManager.MaxSpeed, trackManager.WorldSpeed);
        AudioManager.Instance.SetMusicPitch(Mathf.Lerp(pitchMin, pitchMax, pitchCurve.Evaluate(t)));
    }

    private void OnDeath()
    {
        dead = true;
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.FadeOutMusicWithPitch(deathFadeDuration);
        AudioManager.Instance.FadeOutAmbienceWithPitch(deathFadeDuration);
        AudioManager.Instance.StopLoopingSFX();
        if (deathClip != null) AudioManager.Instance.PlaySFX(deathClip);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopMusic();
            AudioManager.Instance.StopAmbience();
            AudioManager.Instance.StopLoopingSFX();
        }
        SceneController.Instance.LoadScene("MainMenu", fade: true);
    }
}
