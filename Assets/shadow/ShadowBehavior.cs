using UnityEngine;

public class ShadowBehavior : MonoBehaviour
{
    public Transform caster;

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(caster.position, Vector3.down, out hit, 100, 1<<6, QueryTriggerInteraction.Ignore))
        {
            transform.position = hit.point;
        }
        else
        {
            transform.position = new Vector3(0, 10000, 0);
        }
    }
}
