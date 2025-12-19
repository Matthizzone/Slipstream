using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoadEvent : UnityEvent { }

public class Curtain : MonoBehaviour
{
    public static Curtain instance;

    public LoadEvent completeEvent = new LoadEvent();



    public enum CurtainType { Fade, Slide, Cutout }
    CurtainType type;
    float duration;
    Color color;

    public enum SlideType { Top, Bottom, Left, Right, TopBottom, LeftRight };
    SlideType slide_type;




    private void Awake()
    {
        if (instance != null) Destroy(this);
        else instance = this;
    }

    private void Start()
    {
        ResetValues();
    }





    // ---------------------- CONTROLS -----------------------
    #region

    public void ResetValues()
    {
        type = CurtainType.Fade;
        duration = 1f;
        color = Color.black;
        slide_type = SlideType.TopBottom;
    }

    public void SetType(CurtainType new_type)
    {
        type = new_type;
    }

    public void SetDuration(float new_duration)
    {
        duration = new_duration;
    }

    public void SetColor(Color new_color)
    {
        color = new_color;
    }

    public void SetSlideType(SlideType new_slide_type)
    {
        slide_type = new_slide_type;
    }

    #endregion





    // ---------------------- TRANSITION STUFF -----------------------
    #region

    public void Close()
    {
        // cancel if tried already

        if (GameState.currently_transitioning) return;

        StartCoroutine(TransitionRoutine(false));
    }

    public void Open()
    {
        if (GameState.currently_transitioning) return;

        StartCoroutine(TransitionRoutine(true));
    }

    IEnumerator TransitionRoutine(bool opening)
    {
        GameState.currently_transitioning = true;



        // this allows connections between different curtain types (fix later)
        /*
        if (opening)
        {
            OpenAll();
            SetAlpha(0);
        }
        */


        // Transition

        float alpha;
        float t = 0;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            alpha = t / duration;

            SetAlpha(opening ? 1 - alpha : alpha);

            yield return null;
        }

        SetAlpha(opening ? 0f : 1f);


        // Finished. Now Load
        completeEvent.Invoke();
        completeEvent.RemoveAllListeners();


        GameState.currently_transitioning = false;
    }

    void SetAlpha(float alpha)
    {
        // 0 = closed, 0.5 = half, 1 = open
        // changes depending on type

        if (type == CurtainType.Fade)
        {
            transform.Find("Curtain").GetComponent<Image>().color =
                new Color(color.r, color.g, color.b, alpha);
        }
        else if (type == CurtainType.Slide)
        {
            if (slide_type == SlideType.Top)
            {
                transform.Find("Top").GetComponent<RectTransform>().sizeDelta = new Vector2(0, alpha * 1080);
            }
            else if (slide_type == SlideType.Bottom)
            {
                transform.Find("Bottom").GetComponent<RectTransform>().sizeDelta = new Vector2(0, alpha * 1080);
            }
            else if (slide_type == SlideType.Left)
            {
                transform.Find("Left").GetComponent<RectTransform>().sizeDelta = new Vector2(alpha * 1920, 0);
            }
            else if (slide_type == SlideType.Right)
            {
                transform.Find("Right").GetComponent<RectTransform>().sizeDelta = new Vector2(alpha * 1920, 0);
            }
            else if (slide_type == SlideType.TopBottom)
            {
                alpha /= 2;
                transform.Find("Top").GetComponent<RectTransform>().sizeDelta = new Vector2(0, alpha * 1080);
                transform.Find("Bottom").GetComponent<RectTransform>().sizeDelta = new Vector2(0, alpha * 1080);
            }
            else if (slide_type == SlideType.LeftRight)
            {
                alpha /= 2;
                transform.Find("Left").GetComponent<RectTransform>().sizeDelta = new Vector2(alpha * 1920, 0);
                transform.Find("Right").GetComponent<RectTransform>().sizeDelta = new Vector2(alpha * 1920, 0);
            }
        }
        else if (type == CurtainType.Cutout)
        {
            transform.Find("Cutout").GetComponent<RectTransform>().sizeDelta = Vector2.one * 2200 * (1 - alpha);
        }
    }

    void OpenAll()
    {
        CurtainType mem_type = type;

        type = CurtainType.Fade;
        SetAlpha(1);

        type = CurtainType.Slide;
        slide_type = SlideType.Top;
        SetAlpha(1);
        slide_type = SlideType.Bottom;
        SetAlpha(1);
        slide_type = SlideType.Left;
        SetAlpha(1);
        slide_type = SlideType.Right;
        SetAlpha(1);
        slide_type = SlideType.TopBottom;
        SetAlpha(1);
        slide_type = SlideType.LeftRight;
        SetAlpha(1);

        type = CurtainType.Cutout;
        SetAlpha(1);

        type = mem_type;
    }

    #endregion
}
