using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Cinemachine;
using TMPro;

public class PauseMenu : MonoBehaviour
{
	[Header("Exit to scene")]
    public string mainMenuSceneName = "Main Menu";
	
	[Header("Third Person Camera")]
	[SerializeField] private CinemachineFreeLook freeLookCam;
	[SerializeField] private float baseXSpeed = 0.025f;
	[SerializeField] private float baseYSpeed = 0.1f;
	
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
	public GameObject OverlayUI;
	public GameObject pauseMenuUI;
	public GameObject pauseButtonGroup;
	public GameObject settingsButtonGroup;
	private bool isSettingsActive;

	[Header("Pause key")]
	public KeyCode pauseKey = KeyCode.P;

	public bool isPaused { get; private set; }
	
	public Animator pauseMenuAnimator;
	public float fadeOutDuration = 0.5f;
		
    // Start is called before the first frame update
    void Start()
    {		
		Time.timeScale = 1f;
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		isPaused = false;

		// Start panels hidden
		if (pauseMenuUI != null)
			pauseMenuUI.SetActive(false);
		
		isSettingsActive = false;
		
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
		
		ApplySensitivityToCamera();
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(pauseKey) && !isSettingsActive){
			if (isPaused){
				Resume();
			} else {
				Pause();
			}
		}
    }
	
	public void Pause()
    {
		OverlayUI.SetActive(false);
        pauseMenuUI.SetActive(true);
		settingsButtonGroup.SetActive(false);

		if (freeLookCam) freeLookCam.enabled = false;
		
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
		
		// Fade In
		pauseMenuAnimator.Play("PauseMenu", 0, 0f);
		
        isPaused = true;
    }

    public void Resume()
    {
		PlayerPrefs.Save();
		
		// Fade Out
		pauseMenuAnimator.Play("PauseMenu_fadeOut", 0, 0f);
		
        Time.timeScale = 1f;
        Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
        isPaused = false;
		
		if (freeLookCam) freeLookCam.enabled = true;

		StartFadeOutAndDisable(pauseMenuUI);
    }
	
	public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        
        // Load the menu scene
        SceneManager.LoadScene(mainMenuSceneName);
    }
	
	private void StartFadeOutAndDisable(GameObject panelUI)
    {
        Animator anim = panelUI.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("FadeOut");
        }
    }
	
	// Disables pause panel after fade out
	public void AlertObservers(string message)
    {
        if (message == "PauseFadeOutComplete")
        {
            pauseMenuUI.SetActive(false);
			OverlayUI.SetActive(true);
        }
	}
	
	public void handleSettingsButton(){
		settingsButtonGroup.SetActive(true);
		
		isSettingsActive = true;
		
		pauseButtonGroup.SetActive(false);
	}
	
	public void handleBackButton(){
		// Fade Out
		pauseButtonGroup.SetActive(true);
		
		isSettingsActive = false;
		
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
		
		ApplySensitivityToCamera();
		PlayerPrefs.SetFloat(sensitivityPrefKey, currentSensitivity);
	}
	
	private void ApplySensitivityToCamera(){
		if (freeLookCam != null){
			freeLookCam.m_XAxis.m_MaxSpeed = baseXSpeed * currentSensitivity;
			freeLookCam.m_YAxis.m_MaxSpeed = baseYSpeed * currentSensitivity;
		} else Debug.Log("no freelook");
	}
	
	public void SetMasterVolume(float volume)
	{
		currentVolume = volume;
		UpdateMasterVolumeText(volume);
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
