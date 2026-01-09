using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFinish : MonoBehaviour
{
    [Header("References")]
    public PlayerInventory playerInventory; 
    public Stopwatch stopwatch; 

    [Header("Scene Settings")]
    public string endingSceneName = "End Menu";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SaveStats();
            SceneManager.LoadScene(endingSceneName);
        }
    }

    private void SaveStats()
    {
        // Get Deaths and Collectables from Inventory
        if (playerInventory != null)
        {
            PlayerStats.FinalDeaths = playerInventory.totalDeaths;
            PlayerStats.FinalCollectables = playerInventory.totalCollectables;
        }
		
        if (stopwatch != null)
        {
            // Stop the timer first so it doesn't keep counting during the fade
            stopwatch.stopStopwatch(); 
            PlayerStats.FinalTime = stopwatch.GetTime();
        }
    }
}