using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorFlasher : MonoBehaviour
{
    public Color Color1;
    public Color Color2;
    public float freq = 1;

    void Update()
    {
        float t = Mathf.Sin(freq * Time.unscaledTime) * 0.5f + 0.5f;
        Color color = Color.Lerp(Color1, Color2, t);

        if (GetComponent<Image>() != null)
        {
            GetComponent<Image>().color = color;
        }
        if (GetComponent<RawImage>() != null)
        {
            GetComponent<RawImage>().color = color;
        }
        if (GetComponent<TMPro.TMP_Text>() != null)
        {
            GetComponent<TMPro.TMP_Text>().color = color;
        }
    }
}
