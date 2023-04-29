using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToTitle
{
    [RuntimeInitializeOnLoadMethod]
    private static void RunOnStart()
    {
        Application.wantsToQuit += Application_wantsToQuit;
    }

    private static bool Application_wantsToQuit()
    {
        // Return to title screen when quitting if title screen isn't already active
        if (SceneManager.GetActiveScene().path != "Assets/Scenes/TitleScreen.unity")
        {
            SceneManager.LoadScene("Scenes/TitleScreen");
            return false;
        }
        return true;
    }
}
