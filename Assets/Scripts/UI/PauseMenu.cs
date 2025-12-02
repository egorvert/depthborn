using UnityEngine;

public class PauseMenu : MonoBehaviour
{
	[Header("Pause panel")]
	public GameObject pauseMenuUI;

	[Header("Pause key")]
	public KeyCode pauseKey = KeyCode.P;

	public bool isPaused { get; private set; }
		
    // Start is called before the first frame update
    void Start()
    {
		Resume();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(pauseKey)){
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
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        isPaused = true;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
        isPaused = false;
    }
}
