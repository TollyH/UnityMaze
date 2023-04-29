using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public static class NetCode
{
    /// <summary>
    /// Separates a string in the format 'host:port' to an IPEndPoint.
    /// </summary>
    public static IPEndPoint GetHostPort(string str)
    {
        string[] split = str.Split(":", 2);
        return new IPEndPoint(IPAddress.Parse(split[0]), int.Parse(split[1]));
    }

    /// <summary>
    /// Creates a socket (actually UdpClient - named socket/sock for consistency with Python version)
    /// for the client to use to connect to the server.
    /// </summary>
    public static UdpClient CreateClientSocket()
    {
        UdpClient sock = new();
        sock.Client.SendTimeout = 100;
        sock.Client.ReceiveTimeout = 100;
        return sock;
    }

    /// <summary>
    /// Tell the server where we currently are, and get a list of where all other players are. Also gets current hits
    /// remaining until death and the skin of our last known killer.
    /// </summary>
    /// <returns>Null if a response doesn't arrive in a timely manner, otherwise: (hitsRemaining, lastKillerSkin, kills, deaths, players)</returns>
    public static (byte, byte, ushort, ushort, NetData.Player[])? PingServer(UdpClient sock, IPEndPoint addr, byte[] playerKey, Vector2 coords)
    {
        byte[] coordBytes = new NetData.Coords(coords.x, coords.y).ToByteArray();
        byte[] toSend = new byte[33 + NetData.Coords.ByteSize];
        toSend[0] = (byte)RequestType.Ping;
        Array.Copy(playerKey, 0, toSend, 1, 32);
        Array.Copy(coordBytes, 0, toSend, 33, NetData.Coords.ByteSize);
        try
        {
            _ = sock.Send(toSend, 33 + NetData.Coords.ByteSize, addr);
            byte[] playerListBytes = sock.Receive(ref addr);
            if (playerListBytes.Length < 6)
            {
                throw new Exception("Invalid packet for ping. Ignoring.");
            }
            byte hitsRemaining = playerListBytes[0];
            byte lastKillerSkin = playerListBytes[1];
            ushort kills = BinaryPrimitives.ReadUInt16BigEndian(playerListBytes.AsSpan()[2..4]);
            ushort deaths = BinaryPrimitives.ReadUInt16BigEndian(playerListBytes.AsSpan()[4..6]);
            int playerByteSize = NetData.Player.ByteSize;
            NetData.Player[] players = new NetData.Player[(playerListBytes.Length - 6) / playerByteSize];
            for (int i = 0; i < players.Length; i++)
            {
                players[i] = new NetData.Player(playerListBytes[((i * playerByteSize) + 6)..(((i + 1) * playerByteSize) + 6)]);
            }
            return (hitsRemaining, lastKillerSkin, kills, deaths, players);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    /// <summary>
    /// Tell the server where we currently are, and get whether we're dead, where the monster is,
    /// and a list of where all other players are and what items they've picked up.
    /// </summary>
    /// <returns>Null if a response doesn't arrive in a timely manner, otherwise: (killed, monsterCoords, players, pickedUpItems)</returns>
    public static (bool, Vector2?, NetData.Player[], HashSet<Vector2>)? PingServerCoop(UdpClient sock, IPEndPoint addr, byte[] playerKey, Vector2 coords)
    {
        byte[] coordBytes = new NetData.Coords(coords.x, coords.y).ToByteArray();
        byte[] toSend = new byte[33 + NetData.Coords.ByteSize];
        toSend[0] = (byte)RequestType.Ping;
        Array.Copy(playerKey, 0, toSend, 1, 32);
        Array.Copy(coordBytes, 0, toSend, 33, NetData.Coords.ByteSize);
        try
        {
            int coordsSize = NetData.Coords.ByteSize;
            _ = sock.Send(toSend, 33 + NetData.Coords.ByteSize, addr);
            byte[] playerListBytes = sock.Receive(ref addr);
            if (playerListBytes.Length < coordsSize + 2)
            {
                throw new Exception("Invalid packet for ping. Ignoring.");
            }
            bool killed = playerListBytes[0] != 0;
            Vector2? monsterCoords = new NetData.Coords(playerListBytes[1..(coordsSize + 1)]).ToVector2();
            if (monsterCoords == new Vector2(-1, -1))
            {
                monsterCoords = null;
            }
            int playerSize = NetData.Player.ByteSize;
            byte playerCount = playerListBytes[coordsSize + 1];
            int offset1 = coordsSize + 2;
            int offset2 = (playerSize * playerCount) + offset1;
            NetData.Player[] players = new NetData.Player[playerCount];
            for (int i = 0; i < players.Length; i++)
            {
                players[i] = new NetData.Player(playerListBytes[((i * playerSize) + offset1)..(((i + 1) * playerSize) + offset1)]);
            }
            int coordsCount = (playerListBytes.Length - offset2) / coordsSize;
            HashSet<Vector2> pickedUpItems = new(coordsCount);
            for (int i = 0; i < coordsCount; i++)
            {
                _ = pickedUpItems.Add(new NetData.Coords(playerListBytes[((i * coordsSize) + offset2)..(((i + 1) * coordsSize) + offset2)]).ToVector2());
            }
            return (killed, monsterCoords, players, pickedUpItems);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    /// <summary>
    /// Join a server at the specified address. Returns the private player key assigned to us by the server,
    /// the level the server is using, and whether the match is co-op or not.
    /// </summary>
    /// <returns>Null if a response doesn't arrive in a timely manner, otherwise: (playerKey, level, coop)</returns>
    public static (byte[], int, bool)? JoinServer(UdpClient sock, IPEndPoint addr, string name)
    {
        // Player key is all 0 here as we don't have one yet, but all requests still need to have one.
        byte[] toSend = new byte[57];
        toSend[0] = (byte)RequestType.Join;
        if (name.Length > 0)
        {
            _ = Encoding.ASCII.GetBytes(name.ToCharArray(), 0, Math.Min(24, name.Length), toSend, 33);
        }
        try
        {
            _ = sock.Send(toSend, 57, addr);
            byte[] receivedBytes = sock.Receive(ref addr);
            return (receivedBytes[..32], receivedBytes[32], receivedBytes[33] != 0);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    /// <summary>
    /// Tell the server to fire a gunshot from the specified location in the specified facing direction.
    /// </summary>
    /// <returns>A ShotResponse, or null if a response doesn't arrive in a timely manner</returns>
    public static ShotResponse? FireGun(UdpClient sock, IPEndPoint addr, byte[] playerKey, Vector2 coords, Vector2 facing)
    {
        byte[] coordBytes = new NetData.Coords(coords.x, coords.y).ToByteArray();
        byte[] facingBytes = new NetData.Coords(facing.x, facing.y).ToByteArray();
        byte[] toSend = new byte[33 + (NetData.Coords.ByteSize * 2)];
        toSend[0] = (byte)RequestType.Fire;
        Array.Copy(playerKey, 0, toSend, 1, 32);
        Array.Copy(coordBytes, 0, toSend, 33, NetData.Coords.ByteSize);
        Array.Copy(facingBytes, 0, toSend, 33 + NetData.Coords.ByteSize, NetData.Coords.ByteSize);
        try
        {
            _ = sock.Send(toSend, 33 + (NetData.Coords.ByteSize * 2), addr);
            byte[] receivedBytes = sock.Receive(ref addr);
            return receivedBytes.Length != 1 ? throw new Exception("Invalid packet for gunfire. Ignoring.") : (ShotResponse?)(ShotResponse)receivedBytes[0];
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    /// <summary>
    /// Tell the server to reset our hits and position. This will only work if you are already dead.
    /// </summary>
    public static void Respawn(UdpClient sock, IPEndPoint addr, byte[] playerKey)
    {
        try
        {
            byte[] toSend = new byte[33];
            toSend[0] = (byte)RequestType.Respawn;
            Array.Copy(playerKey, 0, toSend, 1, 32);
            _ = sock.Send(toSend, 33, addr);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    /// <summary>
    /// Tell the server we are leaving the game. Our player key will become immediately unusable after this.
    /// </summary>
    public static void LeaveServer(UdpClient sock, IPEndPoint addr, byte[] playerKey)
    {
        try
        {
            byte[] toSend = new byte[33];
            toSend[0] = (byte)RequestType.Leave;
            Array.Copy(playerKey, 0, toSend, 1, 32);
            _ = sock.Send(toSend, 33, addr);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
