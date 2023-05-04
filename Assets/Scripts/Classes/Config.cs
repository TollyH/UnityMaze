using UnityEngine;

public class Config
{
    public bool MonsterEnabled { get; set; }
    public bool MonsterSoundOnKill { get; set; }
    public bool MonsterSoundOnSpot { get; set; }
    public bool MonsterFlickerLights { get; set; }
    public bool MonsterSoundOnRoam { get; set; }
    public bool DrawFog { get; set; }
    public float FieldOfView { get; set; }

    public Config()
    {
        MonsterEnabled = PlayerPrefs.GetInt("MonsterEnabled", 1) != 0;
        MonsterSoundOnKill = PlayerPrefs.GetInt("MonsterSoundOnKill", 1) != 0;
        MonsterSoundOnSpot = PlayerPrefs.GetInt("MonsterSoundOnSpot", 1) != 0;
        MonsterFlickerLights = PlayerPrefs.GetInt("MonsterFlickerLights", 1) != 0;
        MonsterSoundOnRoam = PlayerPrefs.GetInt("MonsterSoundOnRoam", 1) != 0;
        DrawFog = PlayerPrefs.GetInt("DrawFog", 1) != 0;
        FieldOfView = PlayerPrefs.GetFloat("FieldOfView", 53.0f);
    }

    public void Save()
    {
        PlayerPrefs.SetInt("MonsterEnabled", MonsterEnabled ? 1 : 0);
        PlayerPrefs.SetInt("MonsterSoundOnKill", MonsterSoundOnKill ? 1 : 0);
        PlayerPrefs.SetInt("MonsterSoundOnSpot", MonsterSoundOnSpot ? 1 : 0);
        PlayerPrefs.SetInt("MonsterFlickerLights", MonsterFlickerLights ? 1 : 0);
        PlayerPrefs.SetInt("MonsterSoundOnRoam", MonsterSoundOnRoam ? 1 : 0);
        PlayerPrefs.SetInt("DrawFog", DrawFog ? 1 : 0);
        PlayerPrefs.SetFloat("FieldOfView", FieldOfView);
        PlayerPrefs.Save();
    }
}
