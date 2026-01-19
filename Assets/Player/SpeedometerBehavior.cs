using UnityEngine;

public class SpeedometerBehavior : MonoBehaviour
{
    public Rigidbody rb;

    void Update()
    {
        GetComponent<TMPro.TMP_Text>().text = "" + (int)rb.velocity.magnitude + " m/s";
    }
}
