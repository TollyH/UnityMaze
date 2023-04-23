using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

public class PlayerManager : LevelContentManager
{
    public Vector2 MazePosition => new((-transform.position.x + (LevelManager.Instance.UnitSize / 2)) / LevelManager.Instance.UnitSize,
            (transform.position.z + (LevelManager.Instance.UnitSize / 2)) / LevelManager.Instance.UnitSize);
    public Vector2 GridPosition => new((int)MazePosition.x, (int)MazePosition.y);

    public bool HasMovedThisLevel { get; private set; }

    public float LevelTime { get; private set; }
    public float LevelMoves { get; private set; }

    public float KeySensorTime = 10;
    [NonSerialized]
    public float RemainingKeySensorTime = 0;

    [NonSerialized]
    public bool HasGun = false;

    private CharacterController characterController;
    private CapsuleCollider capsuleCollider;

    [SerializeField]
    private AudioSource ambience;

    [SerializeField]
    private AudioSource footstep;
    private AudioClip[] footstepClips;

    [SerializeField]
    private AudioSource breathing;
    private Dictionary<int, AudioClip> breathingClips;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        footstepClips = Resources.LoadAll<AudioClip>("Sounds/footsteps");

        breathingClips = new Dictionary<int, AudioClip>()
        {
            { 0, Resources.Load<AudioClip>("Sounds/player_breathe/heavy") },
            { 5, Resources.Load<AudioClip>("Sounds/player_breathe/medium") },
            { 10, Resources.Load<AudioClip>("Sounds/player_breathe/light") }
        };
    }

    private void Update()
    {
        if (HasMovedThisLevel && !LevelManager.Instance.IsGameOver
            && !LevelManager.Instance.IsPaused)
        {
            if (!breathing.isPlaying)
            {
                breathing.Play();
            }

            // If there is no monster, play the calmest breathing sound
            AudioClip selectedSound = breathingClips[breathingClips.Keys.Max()];
            if (LevelManager.Instance.MonsterManager.IsMonsterSpawned)
            {
                float monsterDistance = Vector2.Distance(LevelManager.Instance.MonsterManager.GridPosition!.Value,
                    LevelManager.Instance.PlayerManager.GridPosition);
                foreach (int minDistance in breathingClips.Keys)
                {
                    if (monsterDistance >= minDistance)
                    {
                        selectedSound = breathingClips[minDistance];
                    }
                    else
                    {
                        break;
                    }
                }
            }
            breathing.clip = selectedSound;

            LevelTime += Time.deltaTime;
            RemainingKeySensorTime -= Time.deltaTime;
            if (RemainingKeySensorTime < 0)
            {
                RemainingKeySensorTime = 0;
            }
        }
        else if (breathing.isPlaying)
        {
            breathing.Pause();
        }

        if (LevelManager.Instance.IsGameOver || LevelManager.Instance.IsPaused)
        {
            if (ambience.isPlaying)
            {
                ambience.Pause();
            }
        }
        else if (!ambience.isPlaying)
        {
            ambience.Play();
        }
    }

    public override void OnLevelLoad(Level level)
    {
        if (!ambience.isPlaying)
        {
            ambience.Play();
        }
        if (breathing.isPlaying)
        {
            breathing.Pause();
        }

        float unitSize = LevelManager.Instance.UnitSize;
        LevelTime = 0;
        LevelMoves = 0;
        HasMovedThisLevel = false;
        RemainingKeySensorTime = 0;
        HasGun = false;

        // Initialise player position, place them in the middle of the square
        Vector2 startPos = level.StartPoint * unitSize;
        characterController.MoveAbsolute(new Vector3(-startPos.x, transform.position.y, startPos.y));
        if (!XRSettings.enabled)
        {
            characterController.height = unitSize / 2;
            capsuleCollider.height = unitSize / 2;
            transform.rotation = Quaternion.Euler(0, 0, 0);
            Camera.main.transform.SetPositionAndRotation(new Vector3(Camera.main.transform.position.x, unitSize / 2, Camera.main.transform.position.z),
                Quaternion.Euler(0, 0, 0));
        }
        characterController.center = new Vector3(0, characterController.height / 2, 0);
        capsuleCollider.center = new Vector3(0, capsuleCollider.height / 2, 0);
    }

    private void OnMove(float distance)
    {
        float unitSize = LevelManager.Instance.UnitSize;
        HasMovedThisLevel = true;
        float oldLevelMoves = LevelMoves;
        LevelMoves += distance / unitSize;

        // Play footstep sound every time move score crosses every other integer boundary.
        if ((int)(LevelMoves / 2) > (int)(oldLevelMoves / 2))
        {
            footstep.PlayOneShot(footstepClips[UnityEngine.Random.Range(0, footstepClips.Length)]);
        }
    }
}
