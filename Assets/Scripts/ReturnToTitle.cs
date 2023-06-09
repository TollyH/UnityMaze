using UnityEngine;
using UnityEngine.SceneManagement;

public static class ReturnToTitle
{
    [RuntimeInitializeOnLoadMethod]
    private static void RunOnStart()
    {
        Application.wantsToQuit += Application_wantsToQuit;
    }

    private static bool Application_wantsToQuit()
    {
        // Leave any multiplayer games
        foreach (LevelManager levelManager in Object.FindObjectsOfType<LevelManager>())
        {
            if (levelManager.IsMulti)
            {
                try
                {
                    levelManager.MultiplayerManager.LeaveServer();
                }
                catch (System.InvalidOperationException) { }
            }
        }

        // Return to title screen when quitting if title screen isn't already active
        if (SceneManager.GetActiveScene().path != "Assets/Scenes/TitleScreen.unity")
        {
            SceneManager.LoadScene("Scenes/TitleScreen");
            return false;
        }
        return true;
    }
}
