using UnityEngine;
using System.Collections;

public class DisappearingSurface : MonoBehaviour
{
    public float disappearDelay = 3f; // seconds before disappearing
    public float reappearDelay = 2f;  // seconds after disappearing to reappear

    private bool isTriggered = false;
    private Renderer objRenderer;
    private Collider objCollider;

    void Start()
    {
        // stores for fast access
        objRenderer = GetComponent<Renderer>();
        objCollider = GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision collision)
    {       //if player has touched surface and if surface is not already disappearing
        if (!isTriggered && collision.gameObject.GetComponent<PlayerMovement>() != null)
        {
            isTriggered = true;
            StartCoroutine(DisappearAndReturn()); //timer for disappear cycle
        }
    }

    private IEnumerator DisappearAndReturn()
    {
        // pause before it disappears
        yield return new WaitForSeconds(disappearDelay);

        // Disables Renderer and Collider
        if (objRenderer != null) objRenderer.enabled = false;
        if (objCollider != null) objCollider.enabled = false;

        // pause before it reappears
        yield return new WaitForSeconds(reappearDelay);

        // Renderer and Collider enabled 
        if (objRenderer != null) objRenderer.enabled = true;
        if (objCollider != null) objCollider.enabled = true;

        isTriggered = false; // allows repeated use
    }
}