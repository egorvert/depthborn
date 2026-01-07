using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Renderer))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
	[SerializeField] private InputActionReference jumpAction;
	[SerializeField] private InputActionReference moveAction;
    public Vector3 movement;
    public Rigidbody rb;
	private Vector2 moveInput;

    [Header("Physics Settings")]
    public float acceleration = 120f;
    public Vector3 boxSize = new Vector3(0.2f, 0.2f, 0.2f);
    private Vector3 halfBoxSize;

    [Header("Movement Settings")]
    public float movementSpeed = 10f;
    public float jumpSpeed = 6f;
    public float riseMultiplier = 0.8f; // 40% of gravity when rising (slow rise)
    public float fallMultiplier = 2f; // makes falling faster
    public LayerMask groundMask;
    public float groundCheckDistance = 0.3f;
    private bool isGrounded;
    
    [Header("Progressive Speed")]
    public float speedIncreasePerCheckpoint = 0.1f; // 10% faster per checkpoint
    public float maxSpeedMultiplier = 2f;
    private float currentSpeedMultiplier = 1f;

    [Header("Oxygen & Buoyancy")]
    public float maxOxygen = 100f;
    public float currentOxygen = 100f;
    public float oxygenCostPerJump = 10f; // how much oxygen each jump costs
    public float minJumpMultiplier = 0.5f; // jump at 0% oxygen
    public float maxJumpMultiplier = 0.7f; // jump at 100% oxygen

    [Header("Camera Reference")]
    public Transform cameraTransform; // Automatically assigned if null

    // Sticky surface detection
    private bool onStickySurface = false;

    // Input values
    private float moveX;
    private float moveZ;

    // Moving Platform
    private MovingPlatform currentPlatform;
    private Vector3 platformMovement;

    // Jump and movement
    private float jumpBufferDelay = 0.2f;
    private float jumpBufferCounter;
	public UnityEvent<PlayerMovement> OnJump;
	
	void OnEnable()
	{
		moveAction.action.Enable();
		jumpAction.action.Enable();
	}

	void OnDisable()
	{
		moveAction.action.Disable();
		jumpAction.action.Disable();
	}

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Auto-assign main camera if not manually set
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        // Make the player magenta for visual clarity
        Renderer platformRenderer = GetComponent<Renderer>();
        platformRenderer.material.SetColor("_Color", Color.magenta);

        // Lock the cursor to the screen center
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Get half box size for isGrounded check
        halfBoxSize = 0.5f * boxSize;
    }

    void Update()
    {
        // Gather player input
        moveInput = moveAction.action.ReadValue<Vector2>();
        moveX = moveInput.y;
        moveZ = moveInput.x;
        movement = new Vector3(moveX, 0f, moveZ) * (movementSpeed * currentSpeedMultiplier);

        // Jump Buffering
        if (jumpAction.action.WasPressedThisFrame())
        {
            jumpBufferCounter = jumpBufferDelay;
        }
    }
    
    public void IncreaseSpeed()
    {
        currentSpeedMultiplier = Mathf.Min(currentSpeedMultiplier + speedIncreasePerCheckpoint, maxSpeedMultiplier);
        Debug.Log($"Speed multiplier now: {currentSpeedMultiplier}");
    }

    public void ResetSpeedMultiplier()
    {
        currentSpeedMultiplier = 1f;
    }

    public void setOnPlatform(MovingPlatform platform)
    {
        currentPlatform = platform;
    }

    // Public methods for external scripts to modify oxygen
    public void AddOxygen(float amount)
    {
        currentOxygen = Mathf.Clamp(currentOxygen + amount, 0f, maxOxygen);
    }

    public void RemoveOxygen(float amount)
    {
        currentOxygen = Mathf.Max(currentOxygen - amount, 0f);
    }

    public void SetOxygen(float amount)
    {
        currentOxygen = Mathf.Clamp(amount, 0f, maxOxygen);
    }

    public float GetOxygenPercent()
    {
        return currentOxygen / maxOxygen;
    }

    public void ForceJump(float jumpVelocity)
    {
        Vector3 v = rb.velocity;
        v.y = 0f;
        rb.velocity = v;
        rb.AddForce(Vector3.up * jumpVelocity, ForceMode.VelocityChange);
    }

    private void FixedUpdate()
    {
        // Check if the player is grounded
        groundCheck();

        if (currentPlatform != null)
        {
            platformMovement = currentPlatform.platformVelocity * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + platformMovement);
        }

        HandleMovement();
        HandleJump();
        HandleFallSpeed();
    }

    private void groundCheck()
    {
        // Ground detection ray (slightly extended for reliability)
        Vector3 boxOrigin = transform.position + Vector3.up * 0.1f;
        isGrounded = Physics.BoxCast(boxOrigin, halfBoxSize, Vector3.down, out RaycastHit hitInfo, Quaternion.identity, groundCheckDistance, groundMask);
    }

    private void HandleMovement()
    {
        if (cameraTransform.IsUnityNull()) return;

        // Calculate camera-relative movement directions
        Vector3 camLateral = transform.position - cameraTransform.position;
        camLateral.y = 0f;
        camLateral.Normalize();
        Vector3 camOrbital = Vector3.Cross(Vector3.up, camLateral);

        // Compute horizontal movement velocity change with speed multiplier
        Vector3 targetV = (camLateral * movement.x + camOrbital * movement.z);
        Vector3 currentV = rb.velocity;
        Vector3 deltaV = new Vector3(targetV.x - currentV.x, 0f, targetV.z - currentV.z);

        // Reduces control on sticky surfaces
        if (onStickySurface)
            deltaV *= 0.1f;

        // Limits acceleration to prevent fast dashes
        deltaV = Vector3.ClampMagnitude(deltaV, acceleration * Time.fixedDeltaTime);
        rb.AddForce(deltaV, ForceMode.VelocityChange);
    }
    
    private void HandleJump() {
        if (jumpBufferCounter > 0)
            jumpBufferCounter -= Time.fixedDeltaTime;

        if (isGrounded && jumpBufferCounter > 0) {
            // Calculate jump HEIGHT based on oxygen (not speed)
            float oxygenPercent = currentOxygen / maxOxygen;
            float jumpMultiplier = Mathf.Lerp(minJumpMultiplier, maxJumpMultiplier, oxygenPercent);

            // Cancel existing vertical movement
            Vector3 v = rb.velocity;
            v.y = 0;
            rb.velocity = v;

            // Apply constant speed but variable force for height
            rb.AddForce(Vector3.up * (jumpSpeed * jumpMultiplier), ForceMode.Impulse);

            RemoveOxygen(oxygenCostPerJump);
			OnJump.Invoke(this);


            isGrounded = false;
            jumpBufferCounter = 0;
        }
    }

    private void HandleFallSpeed()
    {
        if (rb.velocity.y > 0)
        {
            // Rising - slow it down (underwater drag)
            rb.velocity += Vector3.up * (Physics.gravity.y * (riseMultiplier - 1) * Time.fixedDeltaTime);
        }
        else if (rb.velocity.y < 0)
        {
            // Falling - speed it up
            rb.velocity += Vector3.up * (Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        // Detects sticky surfaces without changing grounded state
        onStickySurface = collision.collider.GetComponent<StickySurface>() != null;
    }

    void OnCollisionExit(Collision collision)
    {
        // Reset sticky state when leaving the surface
        if (collision.collider.GetComponent<StickySurface>() != null)
            onStickySurface = false;
    }
}