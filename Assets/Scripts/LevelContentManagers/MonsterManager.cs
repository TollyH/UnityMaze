using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : LevelContentManager
{
    public float StruggleTime = 5;
    public float ClicksToEscape = 10;
    public float SpottedSoundTimeout = 10;
    public float TimeBetweenRoamSounds = 7.5f;
    public float FieldOfViewRaycasts = 50;

    public Vector2? GridPosition { get; private set; }
    public float? TimeToSpawn { get; private set; }
    public float TimeToMove { get; private set; }

    public bool IsMonsterSpawned => thisRenderer.enabled || remainingEscapeClicks > -1;
    public bool IsPlayerStruggling => remainingEscapeClicks > -1;

    public float TimeBetweenMoves;

    private float remainingLevelStruggle;
    private float remainingEscapeClicks = -1;

    [SerializeField]
    private GameObject monsterOverlay;
    [SerializeField]
    private ViewportFlash viewportFlash;
    [SerializeField]
    private AudioSource roamingSound;
    [SerializeField]
    private AudioSource spottedSound;
    [SerializeField]
    private AudioSource lightFlickerSound;

    private AudioClip[] roamingSoundClips;

    private Renderer thisRenderer;
    private Vector2? lastPosition;

    private float timeSinceSeen;
    private float timeToNextRoamSound = 0;

    private void Awake()
    {
        timeSinceSeen = SpottedSoundTimeout;
        thisRenderer = GetComponentInChildren<Renderer>();
        roamingSoundClips = Resources.LoadAll<AudioClip>("Sounds/monster_roam");
    }

    private void LateUpdate()
    {
        if (TimeToSpawn == null || GridPosition == null || LevelManager.Instance.IsGameOver
            || LevelManager.Instance.IsPaused)
        {
            monsterOverlay.SetActive(false);
            return;
        }

        float unitSize = LevelManager.Instance.UnitSize;
        PlayerManager player = LevelManager.Instance.PlayerManager;
        Vector3 gamePos = new(GridPosition.Value.x * -unitSize, 0, GridPosition.Value.y * unitSize);
        if (remainingEscapeClicks < 0)
        {
            monsterOverlay.SetActive(false);
            if (!thisRenderer.enabled)
            {
                if (player.HasMovedThisLevel)
                {
                    TimeToSpawn -= Time.deltaTime;
                    if (TimeToSpawn <= 0 && Vector3.Distance(player.transform.position, gamePos) > 2 * unitSize)
                    {
                        thisRenderer.enabled = true;
                        TimeToMove = TimeBetweenMoves;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }

            TimeToMove -= Time.deltaTime;
            if (TimeToMove <= 0)
            {
                TimeToMove = TimeBetweenMoves;
                ProcessMonsterMove();
            }

            timeToNextRoamSound -= Time.deltaTime;
            if (timeToNextRoamSound <= 0)
            {
                timeToNextRoamSound = TimeBetweenRoamSounds;
                roamingSound.clip = roamingSoundClips[Random.Range(0, roamingSoundClips.Length)];
                roamingSound.Play();
            }

            timeSinceSeen += Time.deltaTime;
            ProcessSpotSound();
        }
        else if (remainingEscapeClicks > 0)
        {
            thisRenderer.enabled = false;
            monsterOverlay.SetActive(true);
            remainingLevelStruggle -= Time.deltaTime;

            if (remainingLevelStruggle <= 0)
            {
                LevelManager.Instance.KillPlayer();
                monsterOverlay.SetActive(false);
                KillMonster();
            }
        }
        else
        {
            // Player has clicked enough times to escape
            remainingEscapeClicks = -1;
            monsterOverlay.SetActive(false);
            KillMonster();
        }

        transform.localScale = new Vector3(unitSize, unitSize, unitSize);
        transform.position = gamePos;
    }

    public void ProcessMonsterMove()
    {
        Level level = LevelManager.Instance.CurrentLevel;
        Transform player = LevelManager.Instance.PlayerManager.transform;
        float unitSize = LevelManager.Instance.UnitSize;
        Vector3 heightOffset = new(0, unitSize / 4, 0);

        if (TimeToSpawn == null || GridPosition == null || !thisRenderer.enabled)
        {
            return;
        }
        Vector2? prevLastPosition = lastPosition;
        lastPosition = GridPosition;

        Vector3 rayDirection = (player.position + heightOffset) - (transform.position + heightOffset);
        Vector2? movementVector = null;
        if (Physics.Raycast(transform.position + heightOffset, rayDirection, out RaycastHit hit))
        {
            if (hit.collider.transform == player)
            {
                // Get the angle between the player and the monster, then round it to the closest 90deg
                float roundedYaw = Mathf.Round(Mathf.Atan2(rayDirection.x, rayDirection.z) * Mathf.Rad2Deg / 90) * 90;
                movementVector = roundedYaw switch
                {
                    0 => new Vector2(0, 1),
                    360 => new Vector2(0, 1),
                    -360 => new Vector2(0, 1),
                    90 => new Vector2(-1, 0),
                    -270 => new Vector2(-1, 0),
                    180 => new Vector2(0, -1),
                    -180 => new Vector2(0, -1),
                    270 => new Vector2(1, 0),
                    -90 => new Vector2(1, 0),
                    _ => null
                };
            }
        }
        List<Vector2> movements = new() { new Vector2(0, 1), new Vector2(0, -1), new Vector2(-1, 0), new Vector2(1, 0) };
        while (movementVector == null || !level.IsCoordInBounds(GridPosition.Value + movementVector.Value)
            || level[GridPosition.Value + movementVector.Value].MonsterCollide
            || GridPosition.Value + movementVector.Value == LevelManager.Instance.PlayerWallManager.WallPosition)
        {
            if (movements.Count == 0)
            {
                return;
            }
            int randomIndex = Random.Range(0, movements.Count);
            Vector2 randomMovement = movements[randomIndex];
            if (GridPosition.Value + randomMovement != prevLastPosition)
            {
                movementVector = randomMovement;
            }
            movements.RemoveAt(randomIndex);
        }

        GridPosition += movementVector;
        ProcessLightFlicker();
    }

    public void KillMonster()
    {
        GridPosition = LevelManager.Instance.CurrentLevel.MonsterStart;
        thisRenderer.enabled = false;
    }

    private void OnSpriteTrigger(GameObject triggerObject)
    {
        if (!thisRenderer.enabled)
        {
            return;
        }
        remainingEscapeClicks = ClicksToEscape;
    }

    public void EscapeMonsterClick()
    {
        if (remainingEscapeClicks > 0)
        {
            remainingEscapeClicks--;
        }
    }

    private void ProcessSpotSound()
    {
        float degreeIncrements = Camera.main.fieldOfView / (FieldOfViewRaycasts - 1);
        float fovStartYaw = Camera.main.transform.eulerAngles.y - (Camera.main.fieldOfView / 2);
        for (int i = 0; i < FieldOfViewRaycasts; i++)
        {
            float yawDirection = fovStartYaw + (i * degreeIncrements);
            if (Physics.Raycast(Camera.main.transform.position, Quaternion.Euler(0, yawDirection, 0) * Vector3.forward, out RaycastHit hit))
            {
                if (hit.collider.transform == transform)
                {
                    if (!spottedSound.isPlaying && timeSinceSeen >= SpottedSoundTimeout)
                    {
                        spottedSound.Play();
                    }
                    timeSinceSeen = 0;
                }
            }
        }
    }

    private void ProcessLightFlicker()
    {
        float distance = Mathf.Pow(Vector2.Distance(GridPosition!.Value, LevelManager.Instance.PlayerManager.GridPosition), 2);
        // Flicker on every monster movement when close. Also don't divide by anything less than 1, it will have no more effect than just 1.
        distance = Mathf.Max(1, distance - 10);
        // < 1 exponent makes probability decay less with distance
        if (Random.value < 1 / Mathf.Pow(distance, 0.6f))
        {
            lightFlickerSound.Play();
            viewportFlash.PerformFlash(Colors.Black, Random.Range(0, 0.5f), 0.5f, 0.5f);
        }
    }

    public override void OnLevelLoad(Level level)
    {
        GridPosition = level.MonsterStart;
        TimeToSpawn = level.MonsterWait;
        TimeToMove = TimeBetweenMoves;

        remainingLevelStruggle = StruggleTime;
        remainingEscapeClicks = -1;

        timeToNextRoamSound = 0;
        timeSinceSeen = SpottedSoundTimeout;

        thisRenderer.enabled = false;
    }
}
