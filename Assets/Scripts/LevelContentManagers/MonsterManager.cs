using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : LevelContentManager
{
    public float StruggleTime = 5;
    public float ClicksToEscape = 10;

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

    private Renderer thisRenderer;
    private Vector2? lastPosition;

    private void Awake()
    {
        thisRenderer = GetComponentInChildren<Renderer>();
    }

    private void LateUpdate()
    {
        if (TimeToSpawn == null || GridPosition == null || LevelManager.Instance.IsGameOver)
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

    public override void OnLevelLoad(Level level)
    {
        GridPosition = level.MonsterStart;
        TimeToSpawn = level.MonsterWait;
        TimeToMove = TimeBetweenMoves;

        remainingLevelStruggle = StruggleTime;
        remainingEscapeClicks = -1;

        thisRenderer.enabled = false;
    }
}
