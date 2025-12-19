using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeakerBehavior : MonoBehaviour
{
    public bool killOnFinish;

    public float base_vol;

    bool fading_out;

    void Update()
    {
        if (killOnFinish && !GetComponent<AudioSource>().isPlaying) Destroy(gameObject);
    }

    public void StartFade(float target_vol, float how_long)
    {
        if (fading_out) return;


        if (how_long == 0)
        {
            if (target_vol == 0)
            {
                Destroy(gameObject);
            }
            else
            {
                GetComponent<AudioSource>().volume = target_vol;
            }
        }
        else
        {
            StartCoroutine(FadeRoutine(target_vol, how_long));
        }
    }

    IEnumerator FadeRoutine(float target_vol, float how_long)
    {
        fading_out = true;

        AudioSource AS = GetComponent<AudioSource>();
        float start_vol = AS.volume;

        float end_time = Time.time + how_long;

        while (Time.time < end_time)
        {
            float factor = 1 + (Time.time - end_time) / how_long;

            AS.volume = Mathf.Lerp(start_vol, target_vol, factor);

            yield return null;
        }

        if (target_vol == 0)
        {
            Destroy(gameObject);
        }

        fading_out = false;
    }
}
