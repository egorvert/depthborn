using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Renderer))]
public class PlayerMovement : MonoBehaviour {
    [Header("References")]
    public Vector3 movement;
    public Rigidbody rb;

    // enviromental constants
    [Header("Physics Settings")]
    public float gravity = -9.81f; // gravity
    public float acceleration = 60f;

    // player constants
    [Header("Player Settings")]
    [Range(0f, 100f)] public float oxygen; // how much "air" the player has left
    public float waterResistance = 0.4f; // how easily the player can swim
                                        // 0: full drag - 1: no drag
    // movement
    [Header("Movement Settings")]
    public float movementSpeed = 10f; // movement speed
    public float jumpSpeed = 6f; // jump speed
    public LayerMask groundMask;
    public float groundCheckDistance = 1.1f;
    private bool jump = false;
    private bool isGrounded; // whether the player is on the ground/a platform

    [Header("Camera Reference")]
    public Transform cameraTransform; // 3rd person camera, not "Main"


    // input vars
    private float moveX;
    private float moveZ;

    void Start() {
        // make the colour magenta so it's easier to see
        Renderer platformRenderer = GetComponent<Renderer>();
        platformRenderer.material.SetColor("_Color", Color.magenta);
        
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // prevent it from spinning

        Cursor.lockState = CursorLockMode.Locked; // capture cursor
        Cursor.visible = false;
    }

    void Update() {
        // update movement with latest input
        moveX = Input.GetAxis("Vertical");
        moveZ = Input.GetAxis("Horizontal");

        movement = new(moveX, 0f, moveZ);
        movement *= movementSpeed;

        // also check if user wants to jump, and whether the player's on the ground
        if (Input.GetButtonDown("Jump") && isGrounded) jump = true;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundMask);
    }

    private void FixedUpdate() {
    if (rb.velocity.y < 0f) {
        float effectiveGravity = oxygen / 100 * gravity;
        Vector3 buoyancy = Vector3.up * -effectiveGravity;
        rb.AddForce(buoyancy, ForceMode.Acceleration);
    } else {
        rb.AddForce(Vector3.up * gravity, ForceMode.Acceleration);
    }
        
        HandleMovement();
        HandleJump();
    }

    private void HandleMovement() {
        /* imagine the player and the camera in 3d space. we don't know which "angle"
        either one is pointing towards, we have no compass. we just know their
        relative positions.

        we can make it so that the camera is always "behind" the player
        by doing some math.*/
        Vector3 camLateral = transform.position - cameraTransform.position;
        camLateral.y = 0f;
        camLateral.Normalize();

        Vector3 camOrbital = Vector3.Cross(Vector3.up, camLateral);

        /* for the physics implementation we're going with we're doing
        a-level kinematics. Vfinal - Vinitial = delta V.
        this is so we can use unity's physics and colliders. 
        also factoring in the camera offsets. */
        Vector3 targetV = camLateral * movement.x + camOrbital * movement.z;
        Vector3 currentV = rb.velocity;
        Vector3 deltaV = new(targetV.x - currentV.x, 0, targetV.z - currentV.z);

        float effectiveAcceleration = acceleration * waterResistance;

        // clamp acceleration so it doesn't go haywire
        deltaV = Vector3.ClampMagnitude(deltaV, effectiveAcceleration * Time.fixedDeltaTime);
        rb.AddForce(deltaV, ForceMode.VelocityChange);
    }

    private void HandleJump() {
        // juuuuump up in the aaaair
        if (jump && isGrounded) {
            rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            jump = false;
        }
    }
}
