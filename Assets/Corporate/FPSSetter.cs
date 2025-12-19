using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSSetter : MonoBehaviour
{
    public int FPS = 60;

    static bool applied = false;

    void Start()
    {
        if (applied) return;

        Application.targetFrameRate = FPS;
        applied = true;
    }

    /*
    private void Update()
    {
        float FR = 1 / Time.deltaTime;

        string FPS = "";
        FPS += GetNthDigit(FR, 1);
        FPS += GetNthDigit(FR, 0);
        FPS += ".";
        FPS += GetNthDigit(FR, -1);

        transform.Find("Canvas").Find("Text").GetComponent<TMPro.TMP_Text>().text = FPS;
    }

    int GetNthDigit(float C, int n)
    {
        // 9876543210.[-1][-2] etc.

        return ((int)(C * Mathf.Pow(10, -n))) % 10;
    }
    */
}
