using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfigBox : MonoBehaviour
{
    private Config config;

    [SerializeField]
    private Toggle monsterToggle;
    [SerializeField]
    private Toggle jumpscareDeathToggle;
    [SerializeField]
    private Toggle jumpscareSpotToggle;
    [SerializeField]
    private Toggle lightFlickerToggle;
    [SerializeField]
    private Toggle roamSoundToggle;
    [SerializeField]
    private Toggle fogToggle;
    [SerializeField]
    private TextMeshProUGUI fovLabel;
    [SerializeField]
    private Slider fovSlider;
    [SerializeField]
    private Toggle aimGuideToggle;

    private void Update()
    {
        fovLabel.text = $"Field of View ({fovSlider.value:0.0}):";
    }

    private void OnEnable()
    {
        config = new Config();

        monsterToggle.isOn = config.MonsterEnabled;
        jumpscareDeathToggle.isOn = config.MonsterSoundOnKill;
        jumpscareSpotToggle.isOn = config.MonsterSoundOnSpot;
        lightFlickerToggle.isOn = config.MonsterFlickerLights;
        roamSoundToggle.isOn = config.MonsterSoundOnRoam;
        fogToggle.isOn = config.DrawFog;
        fovSlider.value = config.FieldOfView;
        aimGuideToggle.isOn = config.GunAimGuide;
    }

    public void OnConfigClose()
    {
        config.MonsterEnabled = monsterToggle.isOn;
        config.MonsterSoundOnKill = jumpscareDeathToggle.isOn;
        config.MonsterSoundOnSpot = jumpscareSpotToggle.isOn;
        config.MonsterFlickerLights = lightFlickerToggle.isOn;
        config.MonsterSoundOnRoam = roamSoundToggle.isOn;
        config.DrawFog = fogToggle.isOn;
        config.FieldOfView = fovSlider.value;
        config.GunAimGuide = aimGuideToggle.isOn;
        config.Save();

        gameObject.SetActive(false);
    }
}
