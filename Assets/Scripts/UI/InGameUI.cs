using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class InGameUI : MonoBehaviour
{
    private Canvas thisCanvas;
    private CanvasScaler thisScaler;

    [SerializeField]
    private Image statsPanel;
    [SerializeField]
    private Image controlsPanel;
    [SerializeField]
    private TextMeshProUGUI keysLabel;
    [SerializeField]
    private TextMeshProUGUI movesLabel;
    [SerializeField]
    private TextMeshProUGUI timeLabel;

    private void Awake()
    {
        thisCanvas = GetComponent<Canvas>();
        thisScaler = GetComponent<CanvasScaler>();
    }

    private void Update()
    {
        thisCanvas.renderMode = XRSettings.enabled ? RenderMode.ScreenSpaceCamera : RenderMode.ScreenSpaceOverlay;
        thisScaler.uiScaleMode = XRSettings.enabled ? CanvasScaler.ScaleMode.ScaleWithScreenSize : CanvasScaler.ScaleMode.ConstantPixelSize;

        KeysManager keys = LevelManager.Instance.KeysManager;
        PlayerManager player = LevelManager.Instance.PlayerManager;
        MonsterManager monster = LevelManager.Instance.MonsterManager;

        keysLabel.text = $"Keys: {keys.TotalLevelKeys - keys.KeysRemaining}/{keys.TotalLevelKeys}";
        movesLabel.text = $"Moves: {player.LevelMoves:0.0}";
        timeLabel.text = $"Time: {player.LevelTime:0.0}";

        Color bgColor = monster.IsMonsterSpawned ? Color.red : Color.black;
        bgColor.a = 0.5f;
        statsPanel.color = bgColor;
        controlsPanel.color = bgColor;
    }
}
