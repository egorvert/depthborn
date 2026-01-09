using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public float jumpVelocity = 5f;  //This controls height player is launched on the jump pad
    public float disableTime = 0.2f;  // disables PlayerMovement Script jump mechanics briefly

    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();
        Rigidbody rb = other.GetComponent<Rigidbody>();

        if (player != null && rb != null)
        {
            StartCoroutine(DoJump(player, rb));
        }
    }

    private IEnumerator DoJump(PlayerMovement player, Rigidbody rb) // disables PlayerMovement Script jump mechanics briefly
    { 
        player.enabled = false;
        Vector3 v = rb.velocity;
        v.y = jumpVelocity;
        rb.velocity = v;

        yield return new WaitForSeconds(disableTime);

        
        player.enabled = true; // enables original jump mechanics of player
    }
}