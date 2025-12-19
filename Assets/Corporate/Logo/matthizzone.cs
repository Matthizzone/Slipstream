using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable] public class LogoDoneEvent : UnityEvent { }

public class matthizzone : MonoBehaviour
{
    public LogoDoneEvent LogoDone = new LogoDoneEvent();

    public float DelayTime = 0.5f;
    public float InTime = 0.3f;
    public float HoldTime = 1.2f;
    public float OutTime = 0.3f;

    public bool skip;

    TMPro.TMP_Text matthizzone_text;

    public static bool was_already_shown;

    private void Awake()
    {
        if (was_already_shown) Destroy(gameObject);
        transform.GetChild(0).gameObject.SetActive(true);
    }

    private void Start()
    {
        if (skip)
        {
            LogoDone.Invoke();
            Destroy(gameObject);
        }
        else
        {
            matthizzone_text = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<TMPro.TMP_Text>();
            StartCoroutine(matthizzoneRoutine());
        }
    }

    IEnumerator matthizzoneRoutine()
    {
        yield return new WaitForSeconds(DelayTime);

        float start_time = Time.time;
        float end_time = Time.time + InTime;
        while (Time.time < end_time)
        {
            float a = (Time.time - start_time) / (end_time - start_time);

            matthizzone_text.color = new Color(1, 1, 1, a);
            yield return null;
        }

        matthizzone_text.color = new Color(1, 1, 1, 1);
        yield return new WaitForSeconds(HoldTime);

        start_time = Time.time;
        end_time = Time.time + InTime;
        while (Time.time < end_time)
        {
            float a = (Time.time - start_time) / (end_time - start_time);

            matthizzone_text.color = new Color(1, 1, 1, 1 - a);
            yield return null;
        }
        matthizzone_text.color = new Color(1, 1, 1, 0);

        yield return null;

        Curtain.instance.ResetValues();
        Curtain.instance.SetType(Curtain.CurtainType.Cutout);
        Curtain.instance.SetDuration(0);
        Curtain.instance.Open();

        LogoDone.Invoke();

        was_already_shown = true;
        Destroy(gameObject);
    }

    public void OpenYoutube()
    {
        Application.OpenURL("https://www.youtube.com/@matthizzone");
    }

    public void OpenMusic()
    {
        Application.OpenURL("https://matthizzone.bandcamp.com/");
    }

    public void OpenGames()
    {
        Application.OpenURL("https://store.steampowered.com/search/?developer=matthizzone");
    }

    public void OpenDonate()
    {
        Application.OpenURL("www.patreon.com/MattArdizzone");
    }
}