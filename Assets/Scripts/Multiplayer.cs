using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class Multiplayer
{
    public bool IsCoop { get; private set; } = false;

    public NetData.Player[] OtherPlayers { get; private set; } = Array.Empty<NetData.Player>();
    public byte HitsRemaining { get; private set; } = 1;
    public byte LastKillerSkin { get; private set; } = 0;
    public ushort Kills { get; private set; } = 0;
    public ushort Deaths { get; private set; } = 0;

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
                TitleUI.NewPopupTitle = "Connection error";
                TitleUI.NewPopupContent = "Could not connect to server";
                quit = true;
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex);
            TitleUI.NewPopupTitle = "Connection error";
            TitleUI.NewPopupContent = "Invalid server information provided";
            quit = true;
        }

        if (quit)
        {
            Application.Quit();
            return;
        }

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
            levelManager.PlayerManager.HasGun = true; // Player always has gun in deathmatch
        }
    }

    public void Ping(Vector3 position)
    {
        if (!IsCoop)
        {
            (byte, byte, ushort, ushort, NetData.Player[])? pingResponse = NetCode.PingServer(
                sock, addr, playerKey, position.ToMazePosition(levelManager.UnitSize));
            if (pingResponse != null)
            {
                HitsRemaining = pingResponse.Value.Item1;
                LastKillerSkin = pingResponse.Value.Item2;
                Kills = pingResponse.Value.Item3;
                Deaths = pingResponse.Value.Item4;
                OtherPlayers = pingResponse.Value.Item5;
            }
        }
        else
        {
            (bool, Vector2?, NetData.Player[], HashSet<Vector2>)? pingResponse = NetCode.PingServerCoop(
                sock, addr, playerKey, position.ToMazePosition(levelManager.UnitSize));
            if (pingResponse != null)
            {
                if (pingResponse.Value.Item1)
                {
                    levelManager.KillPlayer();
                }
                levelManager.MonsterManager.GridPosition = pingResponse.Value.Item2;
                OtherPlayers = pingResponse.Value.Item3;
                HashSet<Vector2> pickedUpItems = pingResponse.Value.Item4;
                // Remove items no longer present on the server
                foreach (Transform child in levelManager.PickupsManager.transform)
                {
                    if (!pickedUpItems.Contains(child.position.ToMazePosition(levelManager.UnitSize)))
                    {
                        UnityEngine.Object.Destroy(child.gameObject);
                    }
                }
            }
        }
    }

    public ShotResponse FireGun(Vector3 position, Vector3 direction)
    {
        Vector2 direction2d = new Vector2(direction.x, direction.z).normalized;
        ShotResponse? response = NetCode.FireGun(sock, addr, playerKey,
            position.ToMazePosition(levelManager.UnitSize), direction2d);
        return response ?? ShotResponse.Denied;
    }
}
