using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsGroundedCheck : MonoBehaviour
{
    public static bool isGrounded;
    private void Update()
    {

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), 0.1f))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
}
