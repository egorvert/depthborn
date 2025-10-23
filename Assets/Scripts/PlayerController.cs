using System.Diagnostics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Vector3 jump;
    public Rigidbody rb;
    public bool isDead = false;

    public float speed = 3.0f;
    public float jumpForce = 4.0f;
    public bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        jump = new Vector3(0.0f, 2.0f, 0.0f);

        Renderer platformRenderer = GetComponent<Renderer>();
        platformRenderer.material.SetColor("_Color", Color.magenta);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (isDead) return; // prevent inputs during a respawn frame if you later add VFX/anim

        Transform camTransform = Camera.main.transform;

        Vector3 camPosition = new(camTransform.position.x, transform.position.y, camTransform.position.z);
        Vector3 direction = (transform.position - camPosition).normalized;

        Vector3 forwardMovement = direction * Input.GetAxis("Vertical");
        Vector3 horizontalMovement = camTransform.right * Input.GetAxis("Horizontal");

        Vector3 movement = Vector3.ClampMagnitude(forwardMovement + horizontalMovement, 1);

        transform.Translate(speed * Time.deltaTime * movement, Space.World);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(jump * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void OnCollisionStay()
    {
        isGrounded = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Fish"))
        {
            UnityEngine.Debug.Log("ONO U DED! :(((");
            Die();
        }
    }

    public void Die()
    {
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
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.position = pos;
        transform.rotation = rot;

        speed = 3.0f; // reset speed value
    }
}
