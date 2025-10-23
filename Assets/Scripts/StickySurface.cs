using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickySurface : MonoBehaviour
{
    [Range(0f, 1f)]
    public float slideMultiplier = 0.25f; // lower value = stickier surface , slows player down

    private void OnCollisionStay(Collision collision)
    {
        Rigidbody rb = collision.rigidbody;
        if (rb != null)
        {    // horizontal speed stored
            Vector3 horizontalVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            horizontalVel *= slideMultiplier; // reduces horizontal speed
            rb.velocity = new Vector3(horizontalVel.x, rb.velocity.y, horizontalVel.z);
        }
    }
}