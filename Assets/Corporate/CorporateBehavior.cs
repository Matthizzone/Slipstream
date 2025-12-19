using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorporateBehavior : MonoBehaviour
{
    public static CorporateBehavior instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
