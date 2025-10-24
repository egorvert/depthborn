using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Renderer))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Vector3 movement;
    public Rigidbody rb;

    [Header("Physics Settings")]
<<<<<<< HEAD
    public float acceleration = 120f;
    public Vector3 boxSize = new Vector3(0.2f, 0.2f, 0.2f);
    private Vector3 halfBoxSize;

=======
    public float gravity = -9.81f; // gravity
    public float acceleration = 60f;

    // player constants
    [Header("Player Settings")]
    [Range(0f, 100f)] public float oxygen; // how much "air" the player has left
    public float waterResistance = 0.4f; // how easily the player can swim
                                        // 0: full drag - 1: no drag
    // movement
>>>>>>> c9f1819 (broken physics)
    [Header("Movement Settings")]
    public float movementSpeed = 10f;
    public float jumpSpeed = 6f;
    public LayerMask groundMask;
    public float groundCheckDistance = 0.3f;
    private bool isGrounded;

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

    // Jump Buffer
    private float jumpBufferDelay = 0.2f;
    private float jumpBufferCounter;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

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
        moveX = Input.GetAxis("Vertical");
        moveZ = Input.GetAxis("Horizontal");
        movement = new Vector3(moveX, 0f, moveZ) * movementSpeed;

        // Jump Buffering
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferDelay;
        }

        Debug.Log(isGrounded);
    }

<<<<<<< HEAD
    public void setOnPlatform(MovingPlatform platform)
    {
        currentPlatform = platform;
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

        // in the future there'll be buoyancy and drag implemented here
        // float buoyancy = gravity * oxygen;
=======
    private void FixedUpdate() {
    if (rb.velocity.y < 0f) {
        float effectiveGravity = oxygen / 100 * gravity;
        Vector3 buoyancy = Vector3.up * -effectiveGravity;
        rb.AddForce(buoyancy, ForceMode.Acceleration);
    } else {
        rb.AddForce(Vector3.up * gravity, ForceMode.Acceleration);
    }
        
>>>>>>> c9f1819 (broken physics)
        HandleMovement();
        HandleJump();
    }

    private void groundCheck()
    {
        // Ground detection ray (slightly extended for reliability)
        Vector3 boxOrigin = transform.position + Vector3.up * 0.1f;
        isGrounded = Physics.BoxCast(boxOrigin, halfBoxSize, Vector3.down, out RaycastHit hitInfo, Quaternion.identity, groundCheckDistance, groundMask);
    }

    private void HandleMovement()
    {
        if (cameraTransform == null) return;

        // Calculate camera-relative movement directions
        Vector3 camLateral = transform.position - cameraTransform.position;
        camLateral.y = 0f;
        camLateral.Normalize();
        Vector3 camOrbital = Vector3.Cross(Vector3.up, camLateral);

        // Compute horizontal movement velocity change
        Vector3 targetV = camLateral * movement.x + camOrbital * movement.z;
        Vector3 currentV = rb.velocity;
        Vector3 deltaV = new Vector3(targetV.x - currentV.x, 0f, targetV.z - currentV.z);

<<<<<<< HEAD
        // Reduces control on sticky surfaces
        if (onStickySurface)
            deltaV *= 0.1f;

        // Limits acceleration to prevent fast dashes
        deltaV = Vector3.ClampMagnitude(deltaV, acceleration * Time.fixedDeltaTime);
=======
        float effectiveAcceleration = acceleration * waterResistance;

        // clamp acceleration so it doesn't go haywire
        deltaV = Vector3.ClampMagnitude(deltaV, effectiveAcceleration * Time.fixedDeltaTime);
>>>>>>> c9f1819 (broken physics)
        rb.AddForce(deltaV, ForceMode.VelocityChange);
    }

    private void HandleJump()
    {
        if (jumpBufferCounter > 0)
            jumpBufferCounter -= Time.fixedDeltaTime;

        if (isGrounded && (Input.GetButtonDown("Jump") || jumpBufferCounter > 0))
        {
            // Cancel existing vertical movement before jumping to reset velocity
            Vector3 v = rb.velocity;
            v.y = 0;
            rb.velocity = v;

            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            isGrounded = false;
            jumpBufferCounter = 0;
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