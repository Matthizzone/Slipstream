using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineMove : MonoBehaviour
{
    public Vector3 pos1;
    public Vector3 pos2;
    public float period = 2;

    void Update()
    {
        float t = Mathf.Cos(Time.time * Mathf.PI * 2 / period);
        t = t * 0.5f + 0.5f;

        transform.localPosition = Vector3.Lerp(pos1, pos2, t);
    }
}
