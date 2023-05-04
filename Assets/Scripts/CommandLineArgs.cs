#if !UNITY_EDITOR
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class CommandLineArgs
{
    [RuntimeInitializeOnLoadMethod]
    private static void CheckCommandLineArgs()
    {
        string[] args = Environment.GetCommandLineArgs();
        if (args.Length > 1)
        {
            // First argument is the path to the executable
            args = args[1..];
            bool error = false;
            foreach (string arg in args)
            {
                string[] argPair = arg.Split("=");
                if (argPair.Length == 2)
                {
                    string lowerKey = argPair[0].ToLowerInvariant();
                    if (lowerKey is "--level-json-path" or "-p")
                    {
                        LevelManager.NewMazeLevelsJsonPath = argPair[1];
                        continue;
                    }
                    if (lowerKey is "--multiplayer-server" or "-s")
                    {
                        LevelManager.NewMultiplayerServer = argPair[1];
                        continue;
                    }
                    if (lowerKey is "--multiplayer-name" or "-n")
                    {
                        LevelManager.NewMultiplayerName = argPair[1];
                        continue;
                    }
                }
                error = true;
                TitleUI.NewPopupTitle = "Argument Error";
                TitleUI.NewPopupContent = $"Unknown argument: '{arg}'";
                // If any command line arguments are incorrect, don't consider any
                LevelManager.NewMazeLevelsJsonPath = null;
                LevelManager.NewMultiplayerServer = null;
                LevelManager.NewMultiplayerName = null;
            }
            if (!error)
            {
                SceneManager.LoadScene("Scenes/MazeLevelScene");
            }
        }
    }
}
#endif
