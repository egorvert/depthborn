using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target; // The player transform
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, 0f); // Offset from player center
    
    [Header("Distance Settings")]
    [SerializeField] private float distance = 5f;
    [SerializeField] private float minDistance = 2f;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private float scrollSpeed = 2f;
    
    [Header("Orbit Settings")]
    [SerializeField] private float mouseSensitivity = 3f;
    [SerializeField] private float joystickSensitivity = 100f;
    [SerializeField] private float minVerticalAngle = -80f;
    [SerializeField] private float maxVerticalAngle = 80f;
    
    [Header("Smoothing")]
    [SerializeField] private float smoothSpeed = 10f;
    
    private float currentX = 0f;
    private float currentY = 20f;
    
    void Start()
    {
        // Lock and hide cursor (optional - comment out if you don't want this)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Initialize angles based on current rotation
        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;
        currentY = angles.x;
    }
    
    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("Camera target is not assigned!");
            return;
        }
        
        HandleInput();
        UpdateCameraPosition();
    }
    
    void HandleInput()
    {
        // Mouse input
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        
        // Joystick input (right stick typically)
        float joystickX = Input.GetAxis("Horizontal"); // Or use "Right Stick X" if configured
        float joystickY = Input.GetAxis("Vertical");   // Or use "Right Stick Y" if configured
        
        // Combine inputs
        float inputX = mouseX * mouseSensitivity + joystickX * joystickSensitivity * Time.deltaTime;
        float inputY = mouseY * mouseSensitivity + joystickY * joystickSensitivity * Time.deltaTime;
        
        // Update rotation angles
        currentX += inputX;
        currentY -= inputY; // Inverted for natural camera movement
        
        // Clamp vertical angle to prevent camera flipping
        currentY = Mathf.Clamp(currentY, minVerticalAngle, maxVerticalAngle);
        
        // Handle zoom with mouse scroll wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance -= scroll * scrollSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
    }
    
    void UpdateCameraPosition()
    {
        // Calculate target position (player position + offset)
        Vector3 targetPosition = target.position + offset;
        
        // Calculate rotation (locked rotationally - no roll)
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0f);
        
        // Calculate desired camera position
        Vector3 desiredPosition = targetPosition - (rotation * Vector3.forward * distance);
        
        // Smooth camera movement
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        
        // Always look at the target
        transform.LookAt(targetPosition);
    }
    
    // Optional: Call this to unlock cursor
    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    // Optional: Call this to lock cursor
    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}