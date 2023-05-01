using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

public class VictoryScreen : MonoBehaviour
{
    [SerializeField]
    private LevelManager levelManager;

    [SerializeField]
    private TextMeshProUGUI timeScore;
    [SerializeField]
    private TextMeshProUGUI moveScore;
    [SerializeField]
    private TextMeshProUGUI bestTimeScore;
    [SerializeField]
    private TextMeshProUGUI bestMoveScore;
    [SerializeField]
    private TextMeshProUGUI bestGameTimeScore;
    [SerializeField]
    private TextMeshProUGUI bestGameMoveScore;
    [SerializeField]
    private TextMeshProUGUI nextLevelHint;

    private float timeOnScreen = 0;

    private void OnEnable()
    {
        timeOnScreen = 0;

        timeScore.gameObject.SetActive(true);
        moveScore.gameObject.SetActive(false);
        bestTimeScore.gameObject.SetActive(false);
        bestMoveScore.gameObject.SetActive(false);
        bestGameTimeScore.gameObject.SetActive(false);
        bestGameMoveScore.gameObject.SetActive(false);
        nextLevelHint.gameObject.SetActive(false);
    }

    private void Update()
    {
        PlayerManager player = levelManager.PlayerManager;
        timeOnScreen += Time.deltaTime;

        timeScore.text = $"Time Score: {player.LevelTime * Math.Min(1.0, timeOnScreen / 2):F1}";
        if (timeOnScreen >= 2.5)
        {
            moveScore.gameObject.SetActive(true);
            moveScore.text = $"Move Score: {player.LevelMoves * Math.Min(1.0, (timeOnScreen - 2.5) / 2):F1}";
        }
        if (timeOnScreen >= 5.5)
        {
            bestTimeScore.gameObject.SetActive(true);
            bestMoveScore.gameObject.SetActive(true);
            bestTimeScore.text = $"Best Time Score: {levelManager.Highscores[levelManager.CurrentLevelIndex].Item1:F1}";
            bestMoveScore.text = $"Best Move Score: {levelManager.Highscores[levelManager.CurrentLevelIndex].Item2:F1}";
        }
        if (timeOnScreen >= 6.5)
        {
            bestGameTimeScore.gameObject.SetActive(true);
            bestGameMoveScore.gameObject.SetActive(true);
            bestGameTimeScore.text = $"Best Game Time Score: {levelManager.Highscores.Sum(x => x.Item1):F1}";
            bestGameMoveScore.text = $"Best Game Move Score: {levelManager.Highscores.Sum(x => x.Item2):F1}";
        }
        if (timeOnScreen >= 7.5 && (levelManager.CurrentLevelIndex < levelManager.LoadedLevels.Length - 1 || levelManager.IsMulti)
            && !XRSettings.enabled)
        {
            nextLevelHint.text = levelManager.IsMulti ? "Restart the server to play another level" : "Press `]` to go to next level";
            nextLevelHint.gameObject.SetActive(true);
        }
    }
}
