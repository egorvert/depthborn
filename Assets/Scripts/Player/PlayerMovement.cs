using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour {
    [Header("References")]
    public Vector3 movement;
    public Rigidbody rb;

    // enviromental constants
    [Header("Physics Settings")]
    public float waterDrag; // inverse to the resistance mechanic
    public float waterAngularDrag; // turning left/right
    public float gravity = -9.81f; // gravity
    public float acceleration = 120f;

    // player constants
    [Header("Player Settings")]
    public float oxygen; // how much "air" the player has left
    public float waterResistance;
    private bool isDead = false;

    // movement
    [Header("Movement Settings")]
    public float movementSpeed = 10f; // movement speed
    public float jumpSpeed = 6f; // jump speed
    public LayerMask groundMask;
    public float groundCheckDistance = 1.1f;
    private bool jump = false;
    private bool isGrounded; // whether the player is on the ground/a platform

    // input vars
    private float moveX;
    private float moveZ;

    void Start() {
        Renderer platformRenderer = GetComponent<Renderer>();
        platformRenderer.material.SetColor("_Color", Color.magenta);
        
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update() {
        moveX = Input.GetAxis("Vertical");
        moveZ = Input.GetAxis("Horizontal");

        movement = new(moveX, 0f, moveZ);
        movement *= movementSpeed;

        if (Input.GetButtonDown("Jump") && isGrounded) jump = true;

        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundMask);
    }

    private void FixedUpdate() {
        // float buoyancy = gravity * oxygen;
        HandleMovement();
        HandleJump();
    }

    private void HandleMovement() {
        Vector3 targetV = transform.TransformDirection(movement);
        Vector3 currentV = rb.velocity;

        Vector3 deltaV = new(targetV.x - currentV.x, 0, targetV.z - currentV.z);
        deltaV = Vector3.ClampMagnitude(deltaV, acceleration * Time.fixedDeltaTime);
        rb.AddForce(deltaV, ForceMode.VelocityChange);
    }

    private void HandleJump() {
        if (jump && isGrounded) {
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            jump = false;
        }
    }

    void OnCollisionStay() {
        isGrounded = true;
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.CompareTag("Fish"))
        {
            UnityEngine.Debug.Log("ONO U DED! :(((");
            Die();
        }
    }

    // --- Added: death/respawn ---

    public void Die() {
        if (isDead) return;
        isDead = true;

        UnityEngine.Debug.Log("Respawning...");
        // Put death VFX/SFX/animation hooks here in the future

        Respawn();

        isDead = false;
    }

    private void Respawn()
    {
        // Find the last checkpoint activated (falls back to starting checkpoint or Vector3.zero)
        Vector3 pos = CheckpointManager.Instance.GetRespawnPosition();
        Quaternion rot = CheckpointManager.Instance.GetRespawnRotation();
        UnityEngine.Debug.Log($"Respawning to {pos} with rotation {rot}");

        // Reset physics before teleport to avoid carry-over momentum
        // rb.velocity = Vector3.zero;
        // rb.angularVelocity = Vector3.zero;

        transform.position = pos;
        transform.rotation = rot;
    }
}
