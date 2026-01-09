using UnityEngine;
using TMPro;

public class EndResultsDisplay : MonoBehaviour
{
    public TMP_Text timeText;
    public TMP_Text deathText;
    public TMP_Text collectablesText;
	
	private string chestText = " chests!";

    void Start()
    {
        // Format time (e.g. 02:15)
        float t = PlayerStats.FinalTime;
        int minutes = Mathf.FloorToInt(t / 60);
        int seconds = Mathf.FloorToInt(t % 60);

        if (timeText) 
            timeText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);

        if (deathText) 
            deathText.text = "You died " + PlayerStats.FinalDeaths + " times,";

		if (PlayerStats.FinalCollectables == 1){
			chestText = " chest!";
		}
        if (collectablesText) 
            collectablesText.text = "You found " + PlayerStats.FinalCollectables + chestText;
    }
}