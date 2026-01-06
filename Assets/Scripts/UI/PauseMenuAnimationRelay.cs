using UnityEngine;

public class PauseMenuAnimationRelay : MonoBehaviour
{
    public PauseMenu pauseMenu;   

    public void AlertObservers(string message)
    {
        if (pauseMenu != null)
        {
            pauseMenu.AlertObservers(message);
        }
    }
}