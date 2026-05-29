using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider uiSlider;

    private const float DEFAULT_VOLUME = 0.75f;
    private void Awake()
    {
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        uiSlider.onValueChanged.AddListener(SetUIVolume);
    }
    private void Start()
    {
        LoadVolumes();
    }
    private void OnDestroy()
    {
        musicSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.RemoveAllListeners();
        uiSlider.onValueChanged.RemoveAllListeners();
    }
    public void SetMusicVolume(float volume)
    {
        ApplyVolume("Music", "musicVolume", volume);
    }
    public void SetSFXVolume(float volume)
    {
        ApplyVolume("SFX", "sfxVolume", volume);
    }
    public void SetUIVolume(float volume)
    {
        ApplyVolume("UI", "uiVolume", volume);
    }
    private void ApplyVolume(string mixerParam, string prefsKey, float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        audioMixer.SetFloat(mixerParam, Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat(prefsKey, volume);
    }
    private void LoadVolumes()
    {
        float music = PlayerPrefs.GetFloat("musicVolume", DEFAULT_VOLUME);
        float sfx = PlayerPrefs.GetFloat("sfxVolume", DEFAULT_VOLUME);
        float ui = PlayerPrefs.GetFloat("uiVolume", DEFAULT_VOLUME);

        musicSlider.value = music;
        sfxSlider.value = sfx;
        uiSlider.value = ui;
    }
}