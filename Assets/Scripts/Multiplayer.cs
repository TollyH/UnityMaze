using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class Multiplayer
{
    public bool IsCoop { get; private set; } = false;

    private LevelManager levelManager;

    private string multiplayerServer;

    private UdpClient sock;
    private IPEndPoint addr;
    private string multiplayerName;

    private byte[] playerKey;

    public Multiplayer(LevelManager levelManager, string multiplayerServer, string multiplayerName)
    {
        this.levelManager = levelManager;
        this.multiplayerServer = multiplayerServer;
        this.multiplayerName = multiplayerName;
    }

    public void Initialise()
    {
        bool quit = false;

        (byte[], int, bool)? joinResponse = null;
        try
        {
            sock = NetCode.CreateClientSocket();
            addr = NetCode.GetHostPort(multiplayerServer!);
            multiplayerName ??= "Unnamed";
            int retries = 0;
            while (joinResponse is null && retries < 10)
            {
                joinResponse = NetCode.JoinServer(sock, addr, multiplayerName);
                retries++;
                Thread.Sleep(500);
            }
            if (joinResponse is null)
            {
                // TODO: "Could not connect to server", "Connection error"
                quit = true;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            // TODO: "Invalid server information provided", "Connection error"
            quit = true;
        }

        if (!quit)
        {
            playerKey = joinResponse.Value.Item1;
            levelManager.LoadLevel(joinResponse.Value.Item2);
            IsCoop = joinResponse.Value.Item3;

            if (!IsCoop)
            {
                levelManager.PlayerManager.RandomisePlayerCoords();
                // Remove pickups and monsters from deathmatches.
                levelManager.KeysManager.gameObject.DestroyAllChildren();
                levelManager.PickupsManager.gameObject.DestroyAllChildren();
                levelManager.CurrentLevel.MonsterStart = null;
                levelManager.CurrentLevel.MonsterWait = null;
                levelManager.CurrentLevel.EndPoint = new Vector2(-1, -1);  // Make end inaccessible in deathmatches
                levelManager.CurrentLevel.StartPoint = new Vector2(-1, -1);  // Hide start point in deathmatches
                levelManager.PointMarkerManager.ReloadPointMarkers(levelManager.CurrentLevel);
            }
        }
        else
        {
            Application.Quit();
        }
    }
}
