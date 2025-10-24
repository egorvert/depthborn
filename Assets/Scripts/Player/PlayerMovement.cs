using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Renderer))]
public class PlayerMovementMerged : MonoBehaviour
{
    [Header("References")]
    public Vector3 movement;
    public Rigidbody rb;

    [Header("Physics Settings")]
    public float acceleration = 120f;

    [Header("Movement Settings")]
    public float movementSpeed = 10f;
    public float jumpSpeed = 6f;
    public LayerMask groundMask;
    public float groundCheckDistance = 1.1f;
    private bool jump = false;
    private bool isGrounded;

    [Header("Camera Reference")]
    public Transform cameraTransform; // Automatically assigned if null

    // Sticky surface detection
    private bool onStickySurface = false;

    // Input values
    private float moveX;
    private float moveZ;

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
    }

    void Update()
    {
        // Gather player input
        moveX = Input.GetAxis("Vertical");
        moveZ = Input.GetAxis("Horizontal");
        movement = new Vector3(moveX, 0f, moveZ) * movementSpeed;

        // Jump input (only when grounded)
        if (Input.GetButtonDown("Jump") && isGrounded)
            jump = true;
    }

    private void FixedUpdate()
    {
        // Ground detection ray (slightly extended for reliability)
        Vector3 rayOrigin = transform.position + Vector3.up * 0.2f;
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, groundCheckDistance + 0.3f, groundMask);

        // Fallback: treat very low vertical velocity as grounded
        if (!isGrounded && Mathf.Abs(rb.velocity.y) < 0.05f)
            isGrounded = true;

        // Visualize the ground check ray (green = grounded, red = air)
        Debug.DrawRay(rayOrigin, Vector3.down * (groundCheckDistance + 0.3f), isGrounded ? Color.green : Color.red);

        HandleMovement();
        HandleJump();
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

        // Reduces control on sticky surfaces
        if (onStickySurface)
            deltaV *= 0.1f;

        // Limits acceleration to prevent fast dashes
        deltaV = Vector3.ClampMagnitude(deltaV, acceleration * Time.fixedDeltaTime);
        rb.AddForce(deltaV, ForceMode.VelocityChange);
    }

    private void HandleJump()
    {
        if (jump && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            jump = false;
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