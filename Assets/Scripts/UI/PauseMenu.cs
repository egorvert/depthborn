using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using TMPro;

public class PauseMenu : MonoBehaviour
{
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
	
	[Header("Pause panel")]
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
		// Fade Out
		pauseMenuAnimator.Play("PauseMenu_fadeOut", 0, 0f);
		
        Time.timeScale = 1f;
        Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
        isPaused = false;
		
		if (freeLookCam) freeLookCam.enabled = true;

		StartFadeOutAndDisable(pauseMenuUI);
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
	}
	
	private void ApplySensitivityToCamera(){
		if (freeLookCam != null){
			freeLookCam.m_XAxis.m_MaxSpeed = baseXSpeed * currentSensitivity;
			freeLookCam.m_YAxis.m_MaxSpeed = baseYSpeed * currentSensitivity;
		} else Debug.Log("no freelook");
	}
	
	public void handleSaveChangesButton(){
		PlayerPrefs.SetFloat(sensitivityPrefKey, currentSensitivity);
		PlayerPrefs.Save();
		SetSensitivity(currentSensitivity);
	}
}
