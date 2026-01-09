using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DifficultyToggler : MonoBehaviour
{
	[Header("UI References")]
	[SerializeField] private TMP_Text difficultyText;
	
	private string[] difficulties = { "Easy", "Medium", "Hard" };
	private int currentIndex = 1;
	
	private const string DifficultyKey = "SelectedDifficulty";
	
    // Start is called before the first frame update
    void Start()
    {
		// Load index for medium as default
		currentIndex = PlayerPrefs.GetInt(DifficultyKey, 1);
		
        UpdateDifficultyDisplay();
    }
	
	public void ToggleDifficulty()
    {
        currentIndex++;

        // Wrap around array if needed
        if (currentIndex >= difficulties.Length)
        {
            currentIndex = 0;
        }
		
		// Save to prefs
		PlayerPrefs.SetInt(DifficultyKey, currentIndex);
        PlayerPrefs.Save();

        UpdateDifficultyDisplay();
    }

    private void UpdateDifficultyDisplay()
    {
        // Update the text
        difficultyText.text = "Difficulty: " + difficulties[currentIndex];
    }
}
