using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioMixerGroup musicGroup;
    public AudioMixerGroup sfxGroup;
    public AudioMixerGroup voiceGroup;
    public AudioMixerGroup ambientGroup;

    string clip_last_played;
    float next_play_time;

    float Volume = 1;
    float Pitch = 1;
    SoundType Type = SoundType.SFX;
    bool Loop = false;
    float PitchRandomize = 0;
    bool StartRandomize = false;
    int RoundRobin = 1;
    bool CrowdingControl = false;
    Transform Parent = null;
    float ThreeD = 0;
    float MaxDistance = 50;

    const float twelft_root_of_two = 1.0594631f;

    public enum SoundType { Music, SFX, Voice, Ambient }

    private void Awake()
    {
        if (instance != null) Destroy(this);
        else instance = this;
    }

    public void SetVol(float vol)
    {
        Volume = vol;
    }

    public void SetPitch(float pitch)
    {
        Pitch = pitch;
    }

    public void SetPitchSemitones(float semis)
    {
        Pitch = Mathf.Pow(twelft_root_of_two, semis);
    }

    public void SetSoundType(SoundType type)
    {
        Type = type;
    }

    public void SetLoop(bool loop)
    {
        Loop = loop;
    }

    public void SetPitchRandomize(float pitchRandomize)
    {
        PitchRandomize = pitchRandomize;
    }

    public void SetStartRandomize(bool startRandomize)
    {
        StartRandomize = startRandomize;
    }

    public void SetRoundRobin(int round_robin_count)
    {
        RoundRobin = round_robin_count;
    }

    public void SetCrowdingControl(bool crowd_control)
    {
        CrowdingControl = crowd_control;
    }

    public void SetParent(Transform new_parent)
    {
        Parent = new_parent;
    }

    public void SetMaxDistance(float new_max_distance)
    {
        MaxDistance = new_max_distance;
    }

    public void Set3D(float new_3D)
    {
        ThreeD = new_3D;
    }

    public void SetAll(float volume, float pitch, SoundType type, bool loop, float pitch_randomize,
        bool start_randomize, int round_robin, bool crowd_control, float new_3D, float new_max_distance)
    {
        Volume = volume;
        Pitch = pitch;
        Type = type;
        Loop = loop;
        PitchRandomize = pitch_randomize;
        StartRandomize = start_randomize;
        RoundRobin = round_robin;
        CrowdingControl = crowd_control;
        ThreeD = new_3D;
        MaxDistance = new_max_distance;
    }

    public void ResetValues()
    {
        Volume = 1;
        Pitch = 1;
        Type = SoundType.SFX;
        Loop = false;
        PitchRandomize = 0;
        StartRandomize = false;
        RoundRobin = 1;
        CrowdingControl = false;
        Parent = null;
        ThreeD = 0;
        MaxDistance = 50;
    }

    public AudioSource PlaySound(string clip_name, bool reset)
    {
        if (CrowdingControl && clip_name == clip_last_played && Time.time < next_play_time) return null; // prevent too much of the same

        if (reset) ResetValues();

        clip_last_played = clip_name;
        next_play_time = Time.time + Random.Range(0.02f, 0.1f);

        if (Loop && transform.Find(clip_name) != null) return null;

        string suffix = "_" + Random.Range(0, RoundRobin);

        string file_name = clip_name + (RoundRobin > 1 ? suffix : "");
        AudioClip clip = Resources.Load<AudioClip>(file_name);

        if (clip == null)
        {
            print("Audio clip '" + file_name + "' not found");
            return null;
        }

        GameObject speaker = new();
        speaker.name = clip_name;
        speaker.transform.parent = transform;
        if (Parent != null)
        {
            speaker.transform.parent = Parent;
            speaker.transform.localPosition = Vector3.zero;
        }

        SpeakerBehavior SB = speaker.AddComponent<SpeakerBehavior>();
        SB.killOnFinish = !Loop;
        SB.base_vol = Volume;

        AudioSource AS = speaker.AddComponent<AudioSource>();
        AS.loop = Loop;
        AS.clip = clip;
        AS.volume = Volume;
        AS.pitch = Pitch + Random.Range(-PitchRandomize, PitchRandomize);
        AS.outputAudioMixerGroup = Type == SoundType.Music ? musicGroup :
                                    Type == SoundType.SFX ? sfxGroup :
                                    Type == SoundType.Voice ? voiceGroup :
                                    ambientGroup;
        AS.time = StartRandomize ? Random.Range(0f, AS.clip.length) : 0;
        AS.spatialBlend = ThreeD;
        if (ThreeD == 1) AS.rolloffMode = AudioRolloffMode.Linear;
        if (ThreeD == 1) AS.maxDistance = MaxDistance;
        if (ThreeD == 1) AS.dopplerLevel = 0f;
        AS.Play();

        return AS;
    }

    public void StopSound(string clip_name)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == clip_name)
            {
                transform.GetChild(i).GetComponent<SpeakerBehavior>().StartFade(0, 0);
            }
        }
    }

    public bool IsPlaying(string clip_name)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == clip_name)
            {
                return true;
            }
        }

        return false;
    }

    public void SetPitch(string clip_name, float newPitch)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == clip_name)
            {
                transform.GetChild(i).GetComponent<AudioSource>().pitch = newPitch;
            }
        }
    }

    public void FadeToVol(string clip_name, float target_vol, float how_long)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == clip_name)
            {
                transform.GetChild(i).GetComponent<SpeakerBehavior>().StartFade(target_vol, how_long);
            }
        }
    }
}
