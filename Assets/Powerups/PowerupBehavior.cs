using UnityEngine;

public class PowerupBehavior : MonoBehaviour
{
    bool used = false;

    private void OnTriggerEnter(Collider other)
    {
        if (used) return;
        used = true;

        Destroy(transform.parent.gameObject);

        PlayerControls.instance.AddJump();
    }
}
