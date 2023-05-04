using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class DeathScreen : MonoBehaviour
{
    [SerializeField]
    private LevelManager levelManager;

    [SerializeField]
    private Image overlayImage;
    [SerializeField]
    private TextMeshProUGUI inputHint;
    [SerializeField]
    private AudioSource jumpscareSound;

    private Sprite[] playerSprites;
    private Sprite deathMonster;

    private void Awake()
    {
        playerSprites = Resources.LoadAll<Sprite>("Textures/sprite/player");
        deathMonster = Resources.Load<Sprite>("Textures/death_monster");
    }

    private void OnEnable()
    {
        if (levelManager.GameConfig.MonsterSoundOnKill)
        {
            jumpscareSound.Play();
        }
    }

    private void Update()
    {
        if (levelManager.IsMulti)
        {
            inputHint.gameObject.SetActive(!levelManager.MultiplayerManager.IsCoop && !XRSettings.enabled);
            inputHint.text = "Press R to respawn";
        }
        else
        {
            inputHint.gameObject.SetActive(!XRSettings.enabled);
            inputHint.text = "Press R to reset the level";
        }

        overlayImage.sprite = levelManager.IsMulti && !levelManager.MultiplayerManager.IsCoop
            ? playerSprites[levelManager.MultiplayerManager.LastKillerSkin]
            : deathMonster;
    }
}
