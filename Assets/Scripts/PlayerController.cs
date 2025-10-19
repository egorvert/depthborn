using UnityEngine;

public class PlayerController:MonoBehaviour {

    public Vector3 jump;
    public Rigidbody rb;

    public float speed = 3.0f;
    public float jumpForce = 4.0f;
    public bool isGrounded;

    void Start() {
        rb = GetComponent<Rigidbody>();
        jump = new Vector3(0.0f, 2.0f, 0.0f);

        Renderer platformRenderer = GetComponent<Renderer>();
        platformRenderer.material.SetColor("_Color", Color.magenta);

    }

    void Update()
    {
        Transform camTransform = Camera.main.transform;

        Vector3 camPosition = new(camTransform.position.x, transform.position.y, camTransform.position.z);
        Vector3 direction = (transform.position - camPosition).normalized;

        Vector3 forwardMovement = direction * Input.GetAxis("Vertical");
        Vector3 horizontalMovement = camTransform.right * Input.GetAxis("Horizontal");

        Vector3 movement = Vector3.ClampMagnitude(forwardMovement + horizontalMovement, 1);

        transform.Translate(speed * Time.deltaTime * movement, Space.World);

    	if(Input.GetKeyDown(KeyCode.Space) && isGrounded) {
    
            rb.AddForce(jump * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }
    
    void OnCollisionStay() {
    	isGrounded = true;
    }
}
