using UnityEngine;

public class Footstepper : MonoBehaviour
{
    public Animator Anim;
    public Rigidbody rb;

    float prev_anim_time;
    float last_step_time;

    public string override_clip_name;
    public int override_round_robin;

    public bool shake_cam;

    void Update()
    {
        FootStep();
    }

    void FootStep()
    {
        if (Time.time - last_step_time < 0.2f) return;
        if (!IsASteppingAnimation()) return;
        if (rb != null)
        {
            if (rb.velocity.magnitude < 0.2f) return;
        }

        float anim_time = Anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;

        bool l_stepped = anim_time > 0.25f && prev_anim_time < 0.25f;
        bool r_stepped = anim_time > 0.75f && prev_anim_time < 0.75f;
        if (l_stepped || r_stepped)
        {
            PlayFootstep();

            last_step_time = Time.time;
        }

        prev_anim_time = anim_time;
    }

    void PlayFootstep()
    {
        //float vol = 0.5f * rb.velocity.magnitude / 10;

        string clip = GetFootstepMaterial();
        int round_robin = 10;

        if (override_clip_name.Length > 0)
        {
            clip = override_clip_name;
            round_robin = override_round_robin;
        }

        AudioManager.instance.ResetValues();
        AudioManager.instance.SetRoundRobin(round_robin);
        AudioManager.instance.SetPitchRandomize(0.3f);
        AudioManager.instance.Set3D(1f);
        AudioManager.instance.SetParent(transform);
        AudioManager.instance.SetMaxDistance(10);
        AudioManager.instance.SetVol(0.35f);
        AudioManager.instance.PlaySound(clip, false);

        if (shake_cam) Camera.main.GetComponent<CamShake>().BeginDissipateShake(1, 10);
    }

    string GetFootstepMaterial()
    {
        Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 2, 1 << 6, QueryTriggerInteraction.Ignore))
        {
            FootstepMaterial mat = hit.transform.GetComponent<FootstepMaterial>();
            
            if (mat != null)
            {
                return mat.GetClip();
            }
        }

        // default
        return "Footstep/Metal";
    }

    bool IsASteppingAnimation()
    {
        AnimatorClipInfo[]  animatorinfo = Anim.GetCurrentAnimatorClipInfo(0);

        if (animatorinfo.Length == 0) return false;

        string current_animation = animatorinfo[0].clip.name;

        string[] animations_with_footsteps = new string[] { 
            "GroundMove", "Walk", "Jog", "Run", "Spring" };

        for (int i = 0; i < animations_with_footsteps.Length; i++)
        {
            if (current_animation == animations_with_footsteps[i]) return true;
        }

        return false;
    }
}
