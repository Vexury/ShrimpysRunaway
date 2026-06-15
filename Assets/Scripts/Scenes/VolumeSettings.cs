using UnityEngine;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider ambienceSlider;
    [SerializeField] Slider sfxSlider;

    private void Start()
    {
        musicSlider.value    = AudioManager.Instance.MusicVolume;
        ambienceSlider.value = AudioManager.Instance.AmbienceVolume;
        sfxSlider.value      = AudioManager.Instance.SFXVolume;

        musicSlider.onValueChanged.AddListener(AudioManager.Instance.SetMusicVolume);
        ambienceSlider.onValueChanged.AddListener(AudioManager.Instance.SetAmbienceVolume);
        sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);
    }
}
