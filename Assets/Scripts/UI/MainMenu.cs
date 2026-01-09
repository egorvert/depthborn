using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using Cinemachine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	[Header("Scene to load")]
	public string gameSceneName = "Game";
	
	[Header("Sensitivity Setting")]
	[SerializeField] private TMP_Text senseTextValue = null;
	[SerializeField] private Slider senseSlider = null;
	[SerializeField] private string sensitivityPrefKey = "MouseSensitivity";
    [SerializeField] private float defaultSensitivity = 1.0f;
    private float currentSensitivity;
	
	[Header("Master Volume Setting")]
	[SerializeField] private TMP_Text masterVolumeTextValue = null;
	[SerializeField] private Slider masterVolumeSlider = null;
	[SerializeField] private string masterVolumePrefKey = "MasterVolume";
	[SerializeField] private float defaultVolume = 1.0f;
	private float currentVolume;
	
	[Header("Music Volume Setting")]
    [SerializeField] private TMP_Text musicTextValue = null;
    [SerializeField] private Slider musicSlider = null;
    [SerializeField] private string musicParameter = "MusicVolume";
    [SerializeField] private string musicPrefKey = "MusicVolume";
    private float currentMusicVolume = 1.0f;

	[Header("Effect Volume Setting")]
	[SerializeField] private TMP_Text sfxTextValue = null;
    [SerializeField] private Slider sfxSlider = null;
    [SerializeField] private AudioMixer mainAudioMixer;
    [SerializeField] private string sfxParameter = "EffectVolume";
    [SerializeField] private string sfxPrefKey = "EffectVolume";
    private float currentSFXVolume = 1.0f;
	
	[Header("UI Panels")]
	public GameObject pauseMenuUI;
	public GameObject pauseButtonGroup;
	public GameObject settingsButtonGroup;
	
	public Animator pauseMenuAnimator;
	public float fadeOutDuration = 0.5f;
		
    // Start is called before the first frame update
    void Start()
    {		
		Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
				
		// Update UI to show current settings
		if (PlayerPrefs.HasKey(sensitivityPrefKey))
		{
			currentSensitivity = PlayerPrefs.GetFloat(sensitivityPrefKey);
		}
		else
		{
			currentSensitivity = defaultSensitivity;
		}

		if (senseSlider != null){
			senseSlider.value = currentSensitivity;
			
			senseSlider.onValueChanged.AddListener(SetSensitivity);
		}
		
		if (senseTextValue != null)
			senseTextValue.text = currentSensitivity.ToString("0.0");
		
		// MASTER VOLUME LOGIC
		if (PlayerPrefs.HasKey(masterVolumePrefKey))
		{
			currentVolume = PlayerPrefs.GetFloat(masterVolumePrefKey);
		}
		else
		{
			currentVolume = defaultVolume;
		}

		if (masterVolumeSlider != null)
		{
			masterVolumeSlider.value = currentVolume;
			masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
		}

		UpdateMasterVolumeText(currentVolume);		
		AudioListener.volume = currentVolume;
		
		// SFX VOLUME LOGIC
		if (PlayerPrefs.HasKey(sfxPrefKey))
		{
			currentSFXVolume = PlayerPrefs.GetFloat(sfxPrefKey);
		}
		else
		{
			currentSFXVolume = 1.0f;
		}

		if (sfxSlider != null)
		{
			sfxSlider.value = currentSFXVolume;
			sfxSlider.onValueChanged.AddListener(SetSFXVolume);
		}

		UpdateSFXText(currentSFXVolume);
		SetMixerVolume(sfxParameter, currentSFXVolume);
		
		// MUSIC VOLUME LOGIC
        if (PlayerPrefs.HasKey(musicPrefKey))
        {
            currentMusicVolume = PlayerPrefs.GetFloat(musicPrefKey);
        }
        else
        {
            currentMusicVolume = 1.0f;
        }

        if (musicSlider != null)
        {
            musicSlider.value = currentMusicVolume;
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        UpdateMusicText(currentMusicVolume);
        SetMixerVolume(musicParameter, currentMusicVolume);
	}
	
	public void PlayGame()
    {
		PlayerPrefs.Save();
		
        // Loads the scene
        SceneManager.LoadScene(gameSceneName);
		Time.timeScale = 1f; 
    }
	
	private void StartFadeOutAndDisable(GameObject panelUI)
    {
        Animator anim = panelUI.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("FadeOut");
        }
    }
	
	public void handleSettingsButton(){
		settingsButtonGroup.SetActive(true);
				
		pauseButtonGroup.SetActive(false);
	}
	
	public void handleBackButton(){
		// Fade Out
		pauseButtonGroup.SetActive(true);
				
		settingsButtonGroup.SetActive(false);
	}
	
	public void SetSensitivity(float sensitivity){
		currentSensitivity = sensitivity;

		if (senseTextValue != null)
			senseTextValue.text = sensitivity.ToString("0.0");

		// Keep slider in sync
		if (senseSlider != null && !Mathf.Approximately(senseSlider.value, sensitivity)) {
			senseSlider.value = sensitivity;
		}
		
		PlayerPrefs.SetFloat(sensitivityPrefKey, currentSensitivity);
	}
	
	public void SetMasterVolume(float volume)
	{
		currentVolume = volume;
		
		UpdateMasterVolumeText(volume);

		// Apply volume globally to the scene
		AudioListener.volume = currentVolume;
		
		PlayerPrefs.SetFloat(masterVolumePrefKey, currentVolume);
	}

	private void UpdateMasterVolumeText(float volume)
	{
		if (masterVolumeTextValue != null)
		{
			masterVolumeTextValue.text = (volume * 100).ToString("0") + "%";
		}
	}
	
	public void SetSFXVolume(float volume)
    {
        currentSFXVolume = volume;
        UpdateSFXText(volume);
        SetMixerVolume(sfxParameter, volume);
		
		PlayerPrefs.SetFloat(sfxPrefKey, currentSFXVolume);
    }

    private void UpdateSFXText(float volume)
    {
        if (sfxTextValue != null)
            sfxTextValue.text = (volume * 100).ToString("0") + "%";
    }

    // Helper function to handle the Math and DB assignment
    private void SetMixerVolume(string parameterName, float sliderValue)
    {
        // 0 on slider = -80dB, 1 on slider = 0dB
        float mixerValue = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20;
        
        if (mainAudioMixer != null)
        {
            mainAudioMixer.SetFloat(parameterName, mixerValue);
        }
    }
	
	public void SetMusicVolume(float volume)
    {
        currentMusicVolume = volume;
        UpdateMusicText(volume);
        SetMixerVolume(musicParameter, volume);
		
		PlayerPrefs.SetFloat(musicPrefKey, currentMusicVolume);
    }

    private void UpdateMusicText(float volume)
    {
        if (musicTextValue != null)
            musicTextValue.text = (volume * 100).ToString("0") + "%";
    }
}
