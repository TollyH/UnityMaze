using TMPro;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
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
        keysLabel.text = $"Keys: {keys.TotalLevelKeys - keys.KeysRemaining}/{keys.TotalLevelKeys}";
        movesLabel.text = $"Moves: {player.LevelMoves:0.0}";
        timeLabel.text = $"Time: {player.LevelTime:0.0}";
    }
}
