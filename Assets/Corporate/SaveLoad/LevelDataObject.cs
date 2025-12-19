using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelDataObject
{
    public bool fullScreen;
    public bool levelOver;
    public string name;

    public int master_vol;
    public int music_vol;
    public int sfx_vol;
    public int ambience_vol;

    public LevelDataObject(bool newLevelOver, string new_name, int new_master_vol, int new_music_vol, int new_sfx_vol, int new_ambience_vol)
    {
        levelOver = newLevelOver;
        name = new_name;

        master_vol = new_master_vol;
        music_vol = new_music_vol;
        sfx_vol = new_sfx_vol;
        ambience_vol = new_ambience_vol;
    }
}
