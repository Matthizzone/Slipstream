using UnityEngine;

public class WheelBehavior : MonoBehaviour
{
    int num_contacts = 0;

    private void OnCollisionEnter(Collision collision)
    {
        num_contacts++;
    }

    private void OnCollisionExit(Collision collision)
    {
        num_contacts--;
    }

    public bool IsGrounded()
    {
        return num_contacts > 0;
    }
}
