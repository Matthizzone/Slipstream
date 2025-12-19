using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowRotate : MonoBehaviour
{
    public Vector3 axis;

    void Update()
    {
        transform.Rotate(axis * Time.deltaTime);
    }
}
