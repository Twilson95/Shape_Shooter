using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public Slider soundSlider;
    public Slider musicSlider;
    // public Slider graphicsSlider;
    public Toggle bloomToggle;
    public GameObject bloomObject;
    public AudioSource playerAudio;
    public MusicUpdater musicUpdater;
    public AudioSource musicAudio; 

    void Start()
    {
        // Load saved preferences and apply them to UI
        float soundValue = PlayerPrefs.GetFloat("soundVolume", 1f);
        soundSlider.value = soundValue;
        playerAudio.volume = 0.1f * soundValue;
        musicUpdater.bossVolume = 0.4f * soundValue;
        // bossAudio.volume = 0.1f * soundValue;
        soundSlider.onValueChanged.AddListener(OnSoundSliderChanged);

        float musicValue = PlayerPrefs.GetFloat("musicVolume", 1f);
        musicSlider.value = musicValue;
        musicAudio.volume = 0.5f * musicValue;
        musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);

        // graphicsSlider.value = PlayerPrefs.GetInt("graphicsQuality", 2);
        // graphicsSlider.onValueChanged.AddListener(OnGraphicsSliderChanged);

        int myBoolValueAsInt = PlayerPrefs.GetInt("bloomToggle", 0);
        bool myBoolValue = myBoolValueAsInt != 0;
        bloomToggle.isOn = myBoolValue;
        bloomToggle.onValueChanged.AddListener(OnBloomToggleChanged);
        bloomObject.SetActive(myBoolValue);

        int qualityLevel = QualitySettings.GetQualityLevel();

        // gameObject.SetActive(false);
    }

    public void OnSoundSliderChanged(float value)
    {
        playerAudio.volume = 0.1f * value;
        musicUpdater.bossVolume = 0.4f * value;
        // bossAudio.volume = 0.1f * value;
        PlayerPrefs.SetFloat("soundVolume", value);
    }

    public void OnMusicSliderChanged(float value)
    {
        musicAudio.volume = 0.5f * value;
        PlayerPrefs.SetFloat("musicVolume", value);
    }

    public void OnGraphicsSliderChanged(float value)
    {
        QualitySettings.SetQualityLevel((int)value, true);
        PlayerPrefs.SetInt("graphicsQuality", (int)value);
    }

    public void OnGraphicsButtonPressed(float value)
    {
        QualitySettings.SetQualityLevel((int)value, true);
        PlayerPrefs.SetInt("graphicsQuality", (int)value);
    }

    public void OnBloomToggleChanged(bool value)
    {
        bloomObject.SetActive(value);
        PlayerPrefs.SetInt("bloomToggle", value ? 1 : 0);
    }

    public void SaveOptions()
    {
        Time.timeScale = 1;
        PlayerPrefs.Save();
    }

    public void CloseOptions()
    {
        gameObject.SetActive(false);
    }

    void OnDisable() 
    {
        SaveOptions();
    }
}