using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Leaderboard : MonoBehaviour
{
    public float SpacingPerLine = 33;

    [SerializeField]
    private LevelManager levelManager;

    [SerializeField]
    private GameObject templatePlayerLine;
    [SerializeField]
    private GameObject playerLineContainer;

    private readonly List<TextMeshProUGUI[]> currentPlayerLines = new();

    private void Update()
    {
        List<NetData.Player> otherPlayers = levelManager.MultiplayerManager.OtherPlayers.ToList();
        // Add a dummy player object representing ourselves to show on leaderboard
        otherPlayers.Add(new NetData.Player(levelManager.MultiplayerManager.MultiplayerName, new NetData.Coords(-1, -1),
            0, levelManager.MultiplayerManager.Kills, levelManager.MultiplayerManager.Deaths));
        List<NetData.Player> otherPlayersSorted = otherPlayers.OrderBy(x => -(x.Kills - x.Deaths)).ToList();

        while (currentPlayerLines.Count < otherPlayersSorted.Count)
        {
            GameObject newLine = Instantiate(templatePlayerLine, playerLineContainer.transform);
            newLine.SetActive(true);
            currentPlayerLines.Add(newLine.GetComponentsInChildren<TextMeshProUGUI>());
        }

        while (currentPlayerLines.Count > otherPlayersSorted.Count)
        {
            Destroy(playerLineContainer.transform.GetChild(0).gameObject);
            currentPlayerLines.RemoveAt(0);
        }

        for (int i = 0; i < otherPlayersSorted.Count; i++)
        {
            NetData.Player plr = otherPlayersSorted[i];
            TextMeshProUGUI[] playerLine = currentPlayerLines[i];
            float yPos = templatePlayerLine.transform.localPosition.y - (i * SpacingPerLine);

            playerLine[0].transform.parent.localPosition = new Vector3(
                templatePlayerLine.transform.localPosition.x, yPos, templatePlayerLine.transform.localPosition.z);

            playerLine[0].text = plr.Name;
            playerLine[1].text = plr.Kills.ToString();
            playerLine[2].text = plr.Deaths.ToString();
            playerLine[3].text = (plr.Kills - plr.Deaths).ToString();
        }
    }
}
