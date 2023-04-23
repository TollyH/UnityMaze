using System;
using System.Linq;
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
    private (float, float)[] highscores;

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

        PlayerManager player = LevelManager.Instance.PlayerManager;
        highscores = new (float, float)[LevelManager.Instance.LoadedLevels.Length];
        for (int i = 0; i < LevelManager.Instance.LoadedLevels.Length; i++)
        {
            highscores[i] = (PlayerPrefs.GetFloat($"{i}-time", 0), PlayerPrefs.GetFloat($"{i}-moves", 0));

            if (i == LevelManager.Instance.CurrentLevelIndex)
            {
                if (player.LevelTime < highscores[i].Item1 || highscores[i].Item1 == 0)
                {
                    highscores[i] = (player.LevelTime, highscores[i].Item2);
                    PlayerPrefs.SetFloat($"{i}-time", player.LevelTime);
                }
                if (player.LevelMoves < highscores[i].Item2 || highscores[i].Item2 == 0)
                {
                    highscores[i] = (highscores[i].Item1, player.LevelMoves);
                    PlayerPrefs.SetFloat($"{i}-moves", player.LevelMoves);
                }
            }
        }
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
            bestTimeScore.text = $"Best Time Score: {highscores[LevelManager.Instance.CurrentLevelIndex].Item1:F1}";
            bestMoveScore.text = $"Best Move Score: {highscores[LevelManager.Instance.CurrentLevelIndex].Item2:F1}";
        }
        if (timeOnScreen >= 6.5)
        {
            bestGameTimeScore.gameObject.SetActive(true);
            bestGameMoveScore.gameObject.SetActive(true);
            bestGameTimeScore.text = $"Best Game Time Score: {highscores.Sum(x => x.Item1):F1}";
            bestGameMoveScore.text = $"Best Game Move Score: {highscores.Sum(x => x.Item2):F1}";
        }
        if (timeOnScreen >= 7.5 && LevelManager.Instance.CurrentLevelIndex < LevelManager.Instance.LoadedLevels.Length - 1)
        {
            nextLevelHint.gameObject.SetActive(true);
        }
    }
}
