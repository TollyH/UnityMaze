using System.Collections.Generic;
using UnityEngine;

public class OtherPlayers : MonoBehaviour
{
    [SerializeField]
    private LevelManager levelManager;

    [SerializeField]
    private GameObject spritePrefab;

    private readonly List<SpriteRenderer> playerRenderers = new();
    private Sprite[] playerSprites;

    private void Awake()
    {
        playerSprites = Resources.LoadAll<Sprite>("Textures/sprite/player");
    }

    private void Update()
    {
        if (!levelManager.IsMulti)
        {
            return;
        }

        NetData.Player[] otherPlayers = levelManager.MultiplayerManager.OtherPlayers;

        while (playerRenderers.Count < otherPlayers.Length)
        {
            GameObject newPlayer = Instantiate(spritePrefab, transform);
            playerRenderers.Add(newPlayer.GetComponentInChildren<SpriteRenderer>());
        }

        while (playerRenderers.Count > otherPlayers.Length)
        {
            Destroy(transform.GetChild(0).gameObject);
            playerRenderers.RemoveAt(0);
        }

        for (int i = 0; i < otherPlayers.Length; i++)
        {
            playerRenderers[i].sprite = playerSprites[otherPlayers[i].Skin];
            Vector2 maze2dPos = otherPlayers[i].Pos.ToVector2().MazeToWorldPosition(levelManager.UnitSize);
            Vector3 mazePos = new(maze2dPos.x, 0, maze2dPos.y);
            playerRenderers[i].transform.position = mazePos;
            playerRenderers[i].transform.localScale = new Vector3(levelManager.UnitSize, levelManager.UnitSize, levelManager.UnitSize);
        }
    }
}
