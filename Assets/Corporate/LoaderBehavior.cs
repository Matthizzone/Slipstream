using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class LoaderBehavior : MonoBehaviour
{
    public static LoaderBehavior instance;

    private void Awake()
    {
        if (instance != null) Destroy(this);
        else instance = this;
    }

    int load_zone_number;

    private void Start()
    {
        //SceneManager.sceneLoaded += delegate { SceneLoadComplete(); };
    }

    /// This class does not handle anything transition / curtain related. It simply, in a single
    /// call handles things like loading rooms, reading save data, positioning the player, etc.
    /// Well that was the theory, but of course I fucked it up






    /*
    public void TransitionToScene(string scene_name, CurtainManager.CurtainType type)
    {
        // Don't forget to set stuff in TransitionManager first!

        if (GameState.currently_transitioning) return;

        bool fx_on = ThisSceneHasFX(scene_name);
        CurtainManager.instance.loadEvent.AddListener(delegate { SetBattleFX(fx_on); }); // handle fx
        CurtainManager.instance.loadEvent.AddListener(delegate { SceneManager.LoadScene(scene_name); });

        CurtainManager.instance.StartTransition(type);
    }*/

    /*
    public void LoadScene(string scene_path, int new_load_zone_number)
    {
        load_zone_number = new_load_zone_number;
        SceneManager.LoadScene(scene_path);
    }

    public void SceneLoadComplete()
    {
        // put player in the right spot

        if (GameObject.Find("LOADZONES") != null)
        {
            Transform load_zone = GameObject.Find("LOADZONES").transform.GetChild(load_zone_number);
            Vector3 where = load_zone.position;
            Vector3 exit_dir = load_zone.GetComponent<LoadZone>().exit_dir;
            exit_dir.y = 0;
            exit_dir.Normalize();

            Player.instance.LoadZoneEject(where, exit_dir * 16);
        }


        // transition back out

        StartCoroutine(WaitTwoFramesThenOpenCurtain());
    }

    IEnumerator WaitTwoFramesThenOpenCurtain()
    {
        // Two frames needed so PlayerDisplay can catch up

        yield return null;
        yield return null;

        Curtain.instance.SetDuration(0.5f);
        Curtain.instance.SetType(Curtain.CurtainType.Fade);
        Curtain.instance.Open();
    }



    public ScriptableRendererFeature outline_feature;
    public ScriptableRendererFeature warbly_feature;

    void SetBattleFX(bool to)
    {
        outline_feature.SetActive(to);
        warbly_feature.SetActive(to);
    }

    private void OnApplicationQuit()
    {
        bool fx_on = ThisSceneHasFX(SceneManager.GetActiveScene().name);
        SetBattleFX(fx_on);
    }

    bool ThisSceneHasFX(string scene_name)
    {
        return scene_name == "Battle" || scene_name == "GameOver";
    }
    */
}
