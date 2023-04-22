using System;
using TMPro;
using UnityEngine;

public class VictoryScreen : MonoBehaviour
{
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
        PlayerManager player = LevelManager.Instance.PlayerManager;
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
            // TODO: Highscores
        }
        if (timeOnScreen >= 6.5)
        {
            bestGameTimeScore.gameObject.SetActive(true);
            bestGameMoveScore.gameObject.SetActive(true);
            // TODO: Highscores
        }
        if (timeOnScreen >= 7.5 && LevelManager.Instance.CurrentLevelIndex < LevelManager.Instance.LoadedLevels.Length - 1)
        {
            nextLevelHint.gameObject.SetActive(true);
        }
    }
}
