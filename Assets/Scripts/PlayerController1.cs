using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController1 : MonoBehaviour
{
    public Rigidbody rb;
    public float moveSpeed = 5f;
    public float jumpForce = 4f;
    public bool isGrounded;

    private Vector3 inputDir;
    private bool onStickySurface = false; // tracks sticky surface contacts

    void Start()
    {    //prevents tipping
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        // Camera-relative input
        Transform cam = Camera.main.transform;
        Vector3 forward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 right = cam.right;

        inputDir = (forward * Input.GetAxis("Vertical") + right * Input.GetAxis("Horizontal")).normalized;

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void FixedUpdate()
    {
        // movement different depending on surface type
        if (onStickySurface)
        {
            //Sliding physics
            Vector3 horizontalVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            Vector3 desiredVel = inputDir * moveSpeed;

            // Smoothly change velocity (more realistic slides)
            Vector3 velocityChange = desiredVel - horizontalVel;
            rb.AddForce(velocityChange * rb.mass * 0.1f, ForceMode.Impulse); // changes speed of stickiness
        }
        else
        {
            //Normal movements
            Vector3 desiredVel = inputDir * moveSpeed;
            Vector3 currentVel = rb.velocity;
            Vector3 velocityChange = desiredVel - new Vector3(currentVel.x, 0f, currentVel.z);
            rb.AddForce(velocityChange * rb.mass, ForceMode.VelocityChange);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        isGrounded = true;

        //Detects if surface is slippery
        onStickySurface = collision.collider.GetComponent<StickySurface>() != null;
    }

    void OnCollisionExit(Collision collision)
    {
        //Reset when leaving slippery surface
        if (collision.collider.GetComponent<StickySurface>() != null)
        {
            onStickySurface = false;
        }
    }
}