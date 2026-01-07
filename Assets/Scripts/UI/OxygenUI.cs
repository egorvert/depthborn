using UnityEngine;
using UnityEngine.UI;

public class OxygenUI : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement player;
    public Image fillImage; // drag your bar fill image here
    
    [Header("Colors")]
    public Color fullColor = Color.cyan; // underwater vibe
    public Color lowColor = Color.red;
    public float lowThreshold = 0.3f; // when to start turning red

    void Start()
    {
        if (player == null)
            player = FindObjectOfType<PlayerMovement>();
    }

    void Update()
    {
        if (!player || !fillImage) return;

        float oxygenPercent = player.GetOxygenPercent();
        fillImage.fillAmount = oxygenPercent;

        // Color lerp based on oxygen level
        if (oxygenPercent <= lowThreshold)
        {
            fillImage.color = Color.Lerp(lowColor, fullColor, oxygenPercent / lowThreshold);
        }
        else
        {
            fillImage.color = fullColor;
        }
    }
}