using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMove : MonoBehaviour
{
    public Vector3 dir;

    void Update()
    {
        transform.position += dir * Time.deltaTime;
    }
}
