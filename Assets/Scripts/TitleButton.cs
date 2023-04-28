using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleButton : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("Scenes/MazeLevelScene");
    }
}
