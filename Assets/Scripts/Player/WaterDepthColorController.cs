using UnityEngine;
using UnderwaterEffect;

/// <summary>
/// Controls the camera's underwater depth effect color based on the type of water the player enters.
/// Attach this script to the Player GameObject.
/// </summary>
public class WaterDepthColorController : MonoBehaviour
{
    [Header("Camera Reference")]
    [SerializeField] private CameraUnderwaterEffect cameraUnderwaterEffect;

    [Header("Water Colors")]
    [SerializeField] private Color defaultWaterColor = new Color(0f, 0.42f, 0.87f);      // #006BDE
    [SerializeField] private Color brackishWaterColor = new Color(0.594f, 0.414f, 0.306f); // Current scene brackish

    private void Start()
    {
        // Auto-assign camera underwater effect if not manually set
        if (cameraUnderwaterEffect == null && Camera.main != null)
        {
            cameraUnderwaterEffect = Camera.main.GetComponent<CameraUnderwaterEffect>();
        }

        if (cameraUnderwaterEffect == null)
        {
            Debug.LogWarning("WaterDepthColorController: CameraUnderwaterEffect not found on main camera.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Water")) return;
        if (cameraUnderwaterEffect == null) return;

        string waterName = other.gameObject.name;

        // Check water type by name (handles "(Clone)" suffix from instantiated prefabs)
        if (waterName.Contains("Brackish"))
        {
            cameraUnderwaterEffect.depthColor = brackishWaterColor;
        }
        else if (waterName.Contains("Default"))
        {
            cameraUnderwaterEffect.depthColor = defaultWaterColor;
        }
    }
}
