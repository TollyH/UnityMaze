using System;
using System.Buffers.Binary;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NetData
{
    public class Coords
    {
        public static readonly int ByteSize = 8;

        public float XPos { get; set; }
        public float YPos { get; set; }

        public Coords(float xPos, float yPos)
        {
            XPos = xPos;
            YPos = yPos;
        }

        public Coords(byte[] coordBytes)
        {
            XPos = BinaryPrimitives.ReadInt32BigEndian(coordBytes.AsSpan()[..^4]) / 100f;
            YPos = BinaryPrimitives.ReadInt32BigEndian(coordBytes.AsSpan()[4..8]) / 100f;
        }

        public byte[] ToByteArray()
        {
            // Positions are sent as integers with 2 d.p of accuracy from the original float.
            byte[] bytes = new byte[ByteSize];
            BinaryPrimitives.WriteInt32BigEndian(bytes.AsSpan()[..4], (int)(XPos * 100));
            BinaryPrimitives.WriteInt32BigEndian(bytes.AsSpan()[4..8], (int)(YPos * 100));
            return bytes;
        }

        public Vector2 ToVector2()
        {
            return new Vector2(XPos, YPos);
        }
    }

    public class Player
    {
        public static readonly int ByteSize = Coords.ByteSize + 29;

        public string Name { get; set; }
        public Coords Pos { get; set; }
        public Vector2 GridPos => new((int)Pos.XPos, (int)Pos.YPos);
        public byte Skin { get; set; }
        public ushort Kills { get; set; }
        public ushort Deaths { get; set; }

        public Player(string name, Coords pos, byte skin, ushort kills, ushort deaths)
        {
            Name = name;
            Pos = pos;
            Skin = skin;
            Kills = kills;
            Deaths = deaths;
        }

        public Player(byte[] playerBytes)
        {
            Name = Encoding.ASCII.GetString(playerBytes[..24].TakeWhile(x => x != 0).ToArray());
            Pos = new Coords(playerBytes[24..32]);
            Skin = playerBytes[32];
            Kills = BinaryPrimitives.ReadUInt16BigEndian(playerBytes.AsSpan()[33..35]);
            Deaths = BinaryPrimitives.ReadUInt16BigEndian(playerBytes.AsSpan()[35..37]);
        }

        public byte[] ToByteArray()
        {
            // Positions are sent as integers with 2 d.p of accuracy from the original float.
            byte[] bytes = new byte[ByteSize];
            if (Name.Length > 0)
            {
                _ = Encoding.ASCII.GetBytes(Name, bytes.AsSpan()[..24]);
            }
            Array.Copy(Pos.ToByteArray(), 0, bytes, 24, Coords.ByteSize);
            bytes[32] = Skin;
            BinaryPrimitives.WriteUInt16BigEndian(bytes.AsSpan()[33..35], Kills);
            BinaryPrimitives.WriteUInt16BigEndian(bytes.AsSpan()[35..37], Deaths);
            return bytes;
        }

    }

    public class PrivatePlayer : Player
    {
        public static new readonly int ByteSize = Player.ByteSize + 2;

        public byte HitsRemaining { get; set; }
        public byte LastKillerSkin { get; set; }

        public PrivatePlayer(string name, Coords pos, byte skin, ushort kills, ushort deaths, byte hitsRemaining, byte lastKillerSkin)
            : base(name, pos, skin, kills, deaths)
        {
            HitsRemaining = hitsRemaining;
            LastKillerSkin = lastKillerSkin;
        }

        public PrivatePlayer(byte[] playerBytes) : base(playerBytes)
        {
            HitsRemaining = playerBytes[37];
            LastKillerSkin = playerBytes[38];
        }

        public new byte[] ToByteArray()
        {
            byte[] bytes = new byte[ByteSize];
            Array.Copy(base.ToByteArray(), bytes, Player.ByteSize);
            bytes[37] = HitsRemaining;
            bytes[38] = LastKillerSkin;
            return bytes;
        }
    }
}
