using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class Multiplayer
{
    public bool IsCoop { get; private set; } = false;

    public bool Initialised { get; private set; } = false;
    public string LastErrorMessage { get; private set; } = "";

    public string MultiplayerServer { get; private set; }
    public string MultiplayerName { get; private set; }

    public NetData.Player[] OtherPlayers { get; private set; } = Array.Empty<NetData.Player>();
    public byte HitsRemaining { get; private set; } = 1;
    public byte LastKillerSkin { get; private set; } = 0;
    public ushort Kills { get; private set; } = 0;
    public ushort Deaths { get; private set; } = 0;

    private readonly LevelManager levelManager;

    private UdpClient sock;
    private IPEndPoint addr;

    private byte[] playerKey;

    public Multiplayer(LevelManager levelManager, string multiplayerServer, string multiplayerName)
    {
        this.levelManager = levelManager;
        MultiplayerServer = multiplayerServer;
        MultiplayerName = multiplayerName;
    }

    public void Initialise()
    {
        bool failed = false;

        (byte[], int, bool)? joinResponse = null;
        try
        {
            sock = NetCode.CreateClientSocket();
            addr = NetCode.GetHostPort(MultiplayerServer!);
            MultiplayerName ??= "Unnamed";
            int retries = 0;
            while (joinResponse is null && retries < 10)
            {
                joinResponse = NetCode.JoinServer(sock, addr, MultiplayerName);
                retries++;
                Thread.Sleep(500);
            }
            if (joinResponse is null)
            {
                LastErrorMessage = "Could not connect to server";
                failed = true;
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex);
            LastErrorMessage = "Invalid server information provided";
            failed = true;
        }

        if (failed)
        {
            
            return;
        }

        playerKey = joinResponse.Value.Item1;
        IsCoop = joinResponse.Value.Item3;
        levelManager.LoadLevel(joinResponse.Value.Item2);
        Initialised = true;
    }

    public void Ping(Vector3 position)
    {
        if (!Initialised)
        {
            throw new InvalidOperationException(
                "The Initialise method must be called on this object before utilising it.");
        }

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
                    Vector2 pos = child.position.ToMazePosition(levelManager.UnitSize);
                    Vector2 roundedPos = new(Mathf.Round(pos.x - (2 / levelManager.UnitSize)),
                        Mathf.Round(pos.y - (2 / levelManager.UnitSize)));
                    if (!pickedUpItems.Contains(roundedPos))
                    {
                        UnityEngine.Object.Destroy(child.gameObject);
                    }
                }
                foreach (Transform child in levelManager.KeysManager.transform)
                {
                    Vector2 pos = child.position.ToMazePosition(levelManager.UnitSize);
                    Vector2 roundedPos = new(Mathf.Round(pos.x - (2 / levelManager.UnitSize)),
                        Mathf.Round(pos.y - (2 / levelManager.UnitSize)));
                    if (!pickedUpItems.Contains(roundedPos))
                    {
                        UnityEngine.Object.Destroy(child.gameObject);
                    }
                }
            }
        }
    }

    public ShotResponse FireGun(Vector3 position, Vector3 direction)
    {
        if (!Initialised)
        {
            throw new InvalidOperationException(
                "The Initialise method must be called on this object before utilising it.");
        }

        Vector2 direction2d = new Vector2(-direction.x, direction.z).normalized;
        ShotResponse? response = NetCode.FireGun(sock, addr, playerKey,
            position.ToMazePosition(levelManager.UnitSize), direction2d);
        return response ?? ShotResponse.Denied;
    }

    public void Respawn()
    {
        if (!Initialised)
        {
            throw new InvalidOperationException(
                "The Initialise method must be called on this object before utilising it.");
        }

        NetCode.Respawn(sock, addr, playerKey);
    }

    public void LeaveServer()
    {
        if (!Initialised)
        {
            throw new InvalidOperationException(
                "The Initialise method must be called on this object before utilising it.");
        }

        NetCode.LeaveServer(sock, addr, playerKey);
    }
}
