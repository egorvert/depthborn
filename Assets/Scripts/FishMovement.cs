using UnityEngine;

public class FishMovement : MonoBehaviour
{
    public Rigidbody fishRigidbody;
    public Transform fishModel;
    public float speed = 2f;

    // Wiggles :)
    public float wiggleSpeed = 5f;
    public float wiggleAmount = 15f; // Angle amount in 

    private Vector3 moveDirection;
    private float wiggleTimer = 0f;


    void Start()
    {
        SetRandomDirection();
    }

    void FixedUpdate()
    {
        // Visual wiggle
        wiggleTimer += Time.fixedDeltaTime * wiggleSpeed;
        float wiggleAngle = Mathf.Sin(wiggleTimer) * wiggleAmount;
        if (fishModel != null)
        {
            fishModel.localRotation = Quaternion.Euler(0f, wiggleAngle, 0f);
        }

        // Move forward
        fishRigidbody.MovePosition(fishRigidbody.position + moveDirection * speed * Time.fixedDeltaTime);
    }

    void SetRandomDirection()
    {
        // Set random direction
        moveDirection = new Vector3(
            Random.Range(-1f, 1f),
            0,
            Random.Range(-1f, 1f)
        ).normalized;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("FishBoundary"))
        {
            Vector3 avgNormal = Vector3.zero;

            // Average all contact normals to handle corners
            foreach (var contact in collision.contacts)
                avgNormal += contact.normal;
            avgNormal.Normalize();

            // Reflect away from averaged normal
            moveDirection = Vector3.Reflect(moveDirection, avgNormal);

            // Add randomness
            moveDirection += new Vector3(
                Random.Range(-0.3f, 0.3f),
                0,
                Random.Range(-0.3f, 0.3f)
            );

            // Reduce back and forth motion
            moveDirection.x *= 0.5f;

            moveDirection.Normalize();
        }
    }
}
