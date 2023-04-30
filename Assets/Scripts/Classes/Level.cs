using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
public class Level
{
    public readonly struct GridSquareContents
    {
        public (string, string, string, string)? Wall { get; }
        public bool PlayerCollide { get; }
        public bool MonsterCollide { get; }

        public GridSquareContents((string, string, string, string)? wall, bool playerCollide, bool monsterCollide)
        {
            Wall = wall;
            PlayerCollide = playerCollide;
            MonsterCollide = monsterCollide;
        }
    }

    public Vector2 Dimensions { get; set; }
    public string EdgeWallTextureName { get; set; }
    public (string, string, string, string)?[,] WallMap { get; set; }
    public (bool, bool)[,] CollisionMap { get; set; }
    public Vector2 StartPoint { get; set; }
    public Vector2 EndPoint { get; set; }
    public HashSet<Vector2> ExitKeys { get; private set; }
    public HashSet<Vector2> KeySensors { get; private set; }
    public HashSet<Vector2> Guns { get; private set; }
    public Dictionary<Vector2, string> Decorations { get; set; }
    public Vector2? MonsterStart { get; set; }
    public float? MonsterWait { get; set; }

    public Level(Vector2 dimensions, string edgeWallTextureName, (string, string, string, string)?[,] wallMap,
        (bool, bool)[,] collisionMap, Vector2 startPoint, Vector2 endPoint, HashSet<Vector2> exitKeys,
        HashSet<Vector2> keySensors, HashSet<Vector2> guns, Dictionary<Vector2, string> decorations, Vector2? monsterStart,
        float? monsterWait)
    {
        Dimensions = dimensions;
        EdgeWallTextureName = edgeWallTextureName;
        WallMap = wallMap;
        CollisionMap = collisionMap;
        StartPoint = startPoint;
        EndPoint = endPoint;
        ExitKeys = exitKeys;
        KeySensors = keySensors;
        Guns = guns;
        Decorations = decorations;
        MonsterStart = monsterStart;
        MonsterWait = monsterWait;
    }

    public override string ToString()
    {
        string str = "";
        for (int y = 0; y < WallMap.GetLength(1); y++)
        {
            for (int x = 0; x < WallMap.GetLength(0); x++)
            {
                Vector2 pnt = new(x, y);
                if (MonsterStart == pnt)
                {
                    str += "MM";
                }
                else if (ExitKeys.Contains(pnt))
                {
                    str += "KK";
                }
                else if (StartPoint == pnt)
                {
                    str += "SS";
                }
                else if (EndPoint == pnt)
                {
                    str += "EE";
                }
                else
                {
                    str += WallMap[x, y] is null ? "  " : "██";
                }
            }
            str += Environment.NewLine;
        }
        return str[..^1];
    }

    public JsonLevel GetJsonLevel()
    {
        List<string[]?[]> wallMap = new();
        for (int y = 0; y < Dimensions.y; y++)
        {
            wallMap.Add(new string[]?[(int)Dimensions.x]);
            string[]?[] last = wallMap[^1];
            for (int x = 0; x < Dimensions.x; x++)
            {
                (string, string, string, string)? value = WallMap[x, y];
                last[x] = value is null ? null : new string[4] { value.Value.Item1, value.Value.Item2, value.Value.Item3, value.Value.Item4 };
            }
        }

        List<bool[][]> collisionMap = new();
        for (int y = 0; y < Dimensions.y; y++)
        {
            collisionMap.Add(new bool[(int)Dimensions.x][]);
            bool[][] last = collisionMap[^1];
            for (int x = 0; x < Dimensions.x; x++)
            {
                (bool, bool) value = CollisionMap[x, y];
                last[x] = new bool[2] { value.Item1, value.Item2 };
            }
        }

        List<int[]> exitKeys = new();
        foreach (Vector2 key in ExitKeys)
        {
            exitKeys.Add(key.ToArray());
        }

        List<int[]> keySensors = new();
        foreach (Vector2 sensor in KeySensors)
        {
            keySensors.Add(sensor.ToArray());
        }

        List<int[]> guns = new();
        foreach (Vector2 gun in Guns)
        {
            guns.Add(gun.ToArray());
        }

        Dictionary<string, string> decorations = new();
        foreach (KeyValuePair<Vector2, string> decor in Decorations)
        {
            decorations[$"{decor.Key.x},{decor.Key.y}"] = decor.Value;
        }

        return new JsonLevel(Dimensions.ToArray(), wallMap.ToArray(), collisionMap.ToArray(), StartPoint.ToArray(), EndPoint.ToArray(),
            exitKeys.ToArray(), keySensors.ToArray(), guns.ToArray(), decorations, MonsterStart?.ToArray(), MonsterWait, EdgeWallTextureName);
    }

    public static explicit operator JsonLevel(Level lvl)
    {
        return lvl.GetJsonLevel();
    }

    public GridSquareContents this[int x, int y]
    {
        get
        {
            (bool, bool) collision = CollisionMap[x, y];
            return new GridSquareContents(WallMap[x, y], collision.Item1, collision.Item2);
        }
        set
        {
            WallMap[x, y] = value.Wall;
            CollisionMap[x, y] = (value.PlayerCollide, value.MonsterCollide);
        }
    }

    public GridSquareContents this[Vector2 coord]
    {
        get
        {
            (bool, bool) collision = CollisionMap[(int)coord.x, (int)coord.y];
            return new GridSquareContents(WallMap[(int)coord.x, (int)coord.y], collision.Item1, collision.Item2);
        }
        set
        {
            WallMap[(int)coord.x, (int)coord.y] = value.Wall;
            CollisionMap[(int)coord.x, (int)coord.y] = (value.PlayerCollide, value.MonsterCollide);
        }
    }

    public bool IsCoordInBounds(Vector2 coord)
    {
        return 0 <= coord.x && coord.x < Dimensions.x && 0 <= coord.y && coord.y < Dimensions.y;
    }

    public bool IsCoordInBounds(int x, int y)
    {
        return 0 <= x && x < Dimensions.x && 0 <= y && y < Dimensions.y;
    }
}

[Serializable]
public class JsonLevel
{
    public int[] dimensions;
    public string[]?[][] wall_map;
    public bool[][][] collision_map;
    public int[] start_point;
    public int[] end_point;
    public int[][] exit_keys;
    public int[][] key_sensors;
    public int[][] guns;
    public Dictionary<string, string> decorations;
    public int[]? monster_start;
    public float? monster_wait;
    public string edge_wall_texture_name;

    [JsonConstructor]
    public JsonLevel(int[] dimensions, string[]?[][] wall_map, bool[][][] collision_map, int[] start_point, int[] end_point, int[][] exit_keys,
        int[][] key_sensors, int[][] guns, Dictionary<string, string> decorations, int[]? monster_start, float? monster_wait, string edge_wall_texture_name)
    {
        this.dimensions = dimensions;
        this.wall_map = wall_map;
        this.collision_map = collision_map;
        this.start_point = start_point;
        this.end_point = end_point;
        this.exit_keys = exit_keys;
        this.key_sensors = key_sensors;
        this.guns = guns;
        this.decorations = decorations;
        this.monster_start = monster_start;
        this.monster_wait = monster_wait;
        this.edge_wall_texture_name = edge_wall_texture_name;
    }

    public Level GetLevel()
    {
        (string, string, string, string)?[,] wallMap = new (string, string, string, string)?[dimensions[0], dimensions[1]];
        for (int x = 0; x < dimensions[0]; x++)
        {
            for (int y = 0; y < dimensions[1]; y++)
            {
                string[]? value = wall_map[y][x];
                wallMap[x, y] = value is null ? null : (value[0], value[1], value[2], value[3]);
            }
        }

        (bool, bool)[,] collisionMap = new (bool, bool)[dimensions[0], dimensions[1]];
        for (int x = 0; x < dimensions[0]; x++)
        {
            for (int y = 0; y < dimensions[1]; y++)
            {
                bool[]? value = collision_map[y][x];
                collisionMap[x, y] = (value[0], value[1]);
            }
        }

        HashSet<Vector2> exitKeys = new();
        foreach (int[] key in exit_keys)
        {
            _ = exitKeys.Add(new Vector2(key[0], key[1]));
        }

        HashSet<Vector2> keySensors = new();
        foreach (int[] sensor in key_sensors)
        {
            _ = keySensors.Add(new Vector2(sensor[0], sensor[1]));
        }

        HashSet<Vector2> convertedGuns = new();
        foreach (int[] gun in guns)
        {
            _ = convertedGuns.Add(new Vector2(gun[0], gun[1]));
        }

        Dictionary<Vector2, string> convertedDecorations = new();
        foreach (KeyValuePair<string, string> decor in decorations)
        {
            string[] splitKey = decor.Key.Split(',');
            convertedDecorations[new Vector2(int.Parse(splitKey[0]), int.Parse(splitKey[1]))] = decor.Value;
        }

        return new Level(new Vector2(dimensions[0], dimensions[1]), edge_wall_texture_name, wallMap, collisionMap, new Vector2(start_point[0], start_point[1]),
            new Vector2(end_point[0], end_point[1]), exitKeys, keySensors, convertedGuns, convertedDecorations,
            monster_start is null ? null : new Vector2(monster_start[0], monster_start[1]), monster_wait);
    }

    public static explicit operator Level(JsonLevel lvl)
    {
        return lvl.GetLevel();
    }
}
