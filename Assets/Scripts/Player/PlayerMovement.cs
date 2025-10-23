using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Renderer))]
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
        // in the future there'll be buoyancy and drag implemented here
        // float buoyancy = gravity * oxygen;
        HandleMovement();
        HandleJump();
    }

    private void HandleMovement() {
        // for the physics implementation we're going with we're doing
        // a-level kinematics. Vfinal - Vinitial = delta V.
        // this is so we can use unity's physics and colliders.
        Vector3 targetV = transform.TransformDirection(movement);
        Vector3 currentV = rb.velocity;
        Vector3 deltaV = new(targetV.x - currentV.x, 0, targetV.z - currentV.z);

        // clamp acceleration so it doesn't go haywire
        deltaV = Vector3.ClampMagnitude(deltaV, acceleration * Time.fixedDeltaTime);
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
