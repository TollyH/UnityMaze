using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField]
    private Image statsPanel;
    [SerializeField]
    private TextMeshProUGUI keysLabel;
    [SerializeField]
    private TextMeshProUGUI movesLabel;
    [SerializeField]
    private TextMeshProUGUI timeLabel;

    private void Update()
    {
        KeysManager keys = LevelManager.Instance.KeysManager;
        PlayerManager player = LevelManager.Instance.PlayerManager;
        MonsterManager monster = LevelManager.Instance.MonsterManager;

        keysLabel.text = $"Keys: {keys.TotalLevelKeys - keys.KeysRemaining}/{keys.TotalLevelKeys}";
        movesLabel.text = $"Moves: {player.LevelMoves:0.0}";
        timeLabel.text = $"Time: {player.LevelTime:0.0}";

        Color bgColor = monster.IsMonsterSpawned ? Color.red : Color.black;
        bgColor.a = 0.5f;
        statsPanel.color = bgColor;
    }
}
