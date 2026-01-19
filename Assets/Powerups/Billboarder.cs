using UnityEngine;

public class Billboarder : MonoBehaviour
{
    void Update()
    {
        transform.LookAt(transform.position + Camera.main.transform.forward);
    }
}
