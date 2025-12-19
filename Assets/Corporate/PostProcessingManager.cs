using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingManager : MonoBehaviour
{
    public static PostProcessingManager instance;

    DepthOfField DOF;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        Volume volume = GetComponent<Volume>();

        volume.profile.TryGet<DepthOfField>(out DOF);
    }

    public void BlurTransition(bool on)
    {
        StartCoroutine(BlurTransitionRoutine(on));
    }

    IEnumerator BlurTransitionRoutine(bool on)
    {
        float focal_length = DOF.focalLength.value;
        float focal_length_vel = 0;

        float smooth_time = 0.4f;

        float target_focal_length = on ? 35f : 1f;

        float end_time = Time.time + 1.2f;
        while (Time.time < end_time)
        {
            focal_length = Mathf.SmoothDamp(focal_length, target_focal_length, ref focal_length_vel, smooth_time);
            
            DOF.focalLength.value = focal_length;

            yield return null;
        }
    }

    private void OnApplicationQuit()
    {
        DOF.focalLength.value = 1f;
    }
}
