using UnityEngine;

public class CeilingCheck : MonoBehaviour
{
    public bool isTouchingCeiling;
    private void Update()
    {

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), 0.1f))
        {
            isTouchingCeiling = true;
        }
        else
        {
            isTouchingCeiling = false;
        }
    }
}