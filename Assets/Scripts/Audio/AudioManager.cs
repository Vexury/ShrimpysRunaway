using UnityEngine;
using System.Collections;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambienceSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Volume Settings")]
    [Range(0f, 1f)] [SerializeField] private float masterVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float musicVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float ambienceVolume = 1f;
    [Range(0f, 1f)] [SerializeField] private float sfxVolume = 1f;

    private Coroutine musicFadeCoroutine;
    private Coroutine ambienceFadeCoroutine;

    protected override void Awake()
    {
        base.Awake();

        // Create audio sources if they don't exist
        if (musicSource == null)
        {
            musicSource = CreateAudioSource("MusicSource");
            musicSource.loop = true;
        }

        if (ambienceSource == null)
        {
            ambienceSource = CreateAudioSource("AmbienceSource");
            ambienceSource.loop = true;
        }

        if (sfxSource == null)
        {
            sfxSource = CreateAudioSource("SFXSource");
        }

        LoadVolumes();
        ApplyVolumes();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Only apply if we're in play mode and sources exist
        if (Application.isPlaying && musicSource != null && ambienceSource != null && sfxSource != null)
        {
            ApplyVolumes();
        }
    }
#endif

    private AudioSource CreateAudioSource(string name)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(transform);
        AudioSource source = obj.AddComponent<AudioSource>();
        source.playOnAwake = false;
        return source;
    }

    #region Music

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void UnpauseMusic()
    {
        musicSource.UnPause();
    }

    public void FadeOutMusic(float duration)
    {
        if (musicFadeCoroutine != null) StopCoroutine(musicFadeCoroutine);
        musicFadeCoroutine = StartCoroutine(FadeOutCoroutine(musicSource, duration));
    }

    public void FadeInMusic(AudioClip clip, float duration)
    {
        if (musicFadeCoroutine != null) StopCoroutine(musicFadeCoroutine);
        musicSource.clip = clip;
        musicSource.volume = 0f;
        musicSource.Play();
        musicFadeCoroutine = StartCoroutine(FadeInCoroutine(musicSource, duration, musicVolume * masterVolume));
    }

    #endregion

    #region Ambience

    public void PlayAmbience(AudioClip clip, bool loop = true)
    {
        if (ambienceSource.clip == clip && ambienceSource.isPlaying) return;

        ambienceSource.clip = clip;
        ambienceSource.loop = loop;
        ambienceSource.Play();
    }

    public void StopAmbience()
    {
        ambienceSource.Stop();
    }

    public void PauseAmbience()
    {
        ambienceSource.Pause();
    }

    public void UnpauseAmbience()
    {
        ambienceSource.UnPause();
    }

    public void FadeOutAmbience(float duration)
    {
        if (ambienceFadeCoroutine != null) StopCoroutine(ambienceFadeCoroutine);
        ambienceFadeCoroutine = StartCoroutine(FadeOutCoroutine(ambienceSource, duration));
    }

    public void FadeInAmbience(AudioClip clip, float duration)
    {
        if (ambienceFadeCoroutine != null) StopCoroutine(ambienceFadeCoroutine);
        ambienceSource.clip = clip;
        ambienceSource.volume = 0f;
        ambienceSource.Play();
        ambienceFadeCoroutine = StartCoroutine(FadeInCoroutine(ambienceSource, duration, ambienceVolume * masterVolume));
    }

    #endregion

    #region SFX

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume * masterVolume);
    }

    public void PlaySFX(AudioClip clip, float volumeScale)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, volumeScale * sfxVolume * masterVolume);
    }

    public void PlaySFXAtPoint(AudioClip clip, Vector3 position)
    {
        if (clip == null) return;
        AudioSource.PlayClipAtPoint(clip, position, sfxVolume * masterVolume);
    }

    public void PlaySFXWithPitchVariation(AudioClip clip, float minPitch = 0.9f, float maxPitch = 1.1f, float volumeScale = 1f)
    {
        if (clip == null) return;

        GameObject tempAudio = new GameObject("TempAudio");
        AudioSource tempSource = tempAudio.AddComponent<AudioSource>();
        tempSource.clip = clip;
        tempSource.volume = Mathf.Clamp01(sfxVolume * masterVolume * volumeScale);
        float pitch = Random.Range(minPitch, maxPitch);
        tempSource.pitch = pitch;
        tempSource.Play();

        Destroy(tempAudio, clip.length / Mathf.Abs(pitch));
    }

    #endregion

    #region Volume Control

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        ApplyVolumes();
        SaveVolumes();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        ApplyVolumes();
        SaveVolumes();
    }

    public void SetAmbienceVolume(float volume)
    {
        ambienceVolume = Mathf.Clamp01(volume);
        ApplyVolumes();
        SaveVolumes();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        ApplyVolumes();
        SaveVolumes();
    }

    private void ApplyVolumes()
    {
        musicSource.volume = musicVolume * masterVolume;
        ambienceSource.volume = ambienceVolume * masterVolume;
        sfxSource.volume = sfxVolume * masterVolume;
    }

    private void SaveVolumes()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("AmbienceVolume", ambienceVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }

    private void LoadVolumes()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        ambienceVolume = PlayerPrefs.GetFloat("AmbienceVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }

    #endregion

    #region Fade Coroutines

    private IEnumerator FadeOutCoroutine(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        source.Stop();
        source.volume = startVolume;
    }

    private IEnumerator FadeInCoroutine(AudioSource source, float duration, float targetVolume)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(0f, targetVolume, elapsed / duration);
            yield return null;
        }

        source.volume = targetVolume;
    }

    #endregion
}