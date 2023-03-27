using UnityEngine;

public class WallsManager : LevelContentManager
{
    public void ReloadWalls(Level level)
    {
        float unitSize = LevelManager.Instance.UnitSize;
        gameObject.DestroyAllChildren();

        // Create walls and collision
        for (int x = 0; x < level.Dimensions.x; x++)
        {
            for (int y = 0; y < level.Dimensions.y; y++)
            {
                Level.GridSquareContents contents = level[x, y];
                if (contents.Wall != null)
                {
                    GameObject newWall = new($"MazeWall{x}-{y}");
                    newWall.transform.parent = transform;
                    newWall.transform.position = new Vector3(unitSize * -x, unitSize / 2, unitSize * y);

                    GameObject newPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    newPlane.name = "NorthPlane";
                    // Walls may or may not have colliders, they'll be created later
                    Destroy(newPlane.GetComponent<Collider>());
                    newPlane.transform.parent = newWall.transform;
                    newPlane.transform.localPosition = new Vector3(0, 0, unitSize / 2);
                    newPlane.transform.localScale = new Vector3(unitSize / 10, 1, unitSize / 10);
                    newPlane.transform.localRotation = Quaternion.Euler(90, 0, 0);
                    newPlane.GetComponent<MeshRenderer>().material = Resources.Load<Material>($"Materials/Wall/{contents.Wall.Value.Item3}");

                    newPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    newPlane.name = "EastPlane";
                    // Walls may or may not have colliders, they'll be created later
                    Destroy(newPlane.GetComponent<Collider>());
                    newPlane.transform.parent = newWall.transform;
                    newPlane.transform.localPosition = new Vector3(unitSize / 2, 0, 0);
                    newPlane.transform.localScale = new Vector3(unitSize / 10, 1, unitSize / 10);
                    newPlane.transform.localRotation = Quaternion.Euler(90, 90, 0);
                    newPlane.GetComponent<MeshRenderer>().material = Resources.Load<Material>($"Materials/Wall/{contents.Wall.Value.Item4}");

                    newPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    newPlane.name = "SouthPlane";
                    // Walls may or may not have colliders, they'll be created later
                    Destroy(newPlane.GetComponent<Collider>());
                    newPlane.transform.parent = newWall.transform;
                    newPlane.transform.localPosition = new Vector3(0, 0, -unitSize / 2);
                    newPlane.transform.localScale = new Vector3(unitSize / 10, 1, unitSize / 10);
                    newPlane.transform.localRotation = Quaternion.Euler(90, 180, 0);
                    newPlane.GetComponent<MeshRenderer>().material = Resources.Load<Material>($"Materials/Wall/{contents.Wall.Value.Item1}");

                    newPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                    newPlane.name = "WestPlane";
                    // Walls may or may not have colliders, they'll be created later
                    Destroy(newPlane.GetComponent<Collider>());
                    newPlane.transform.parent = newWall.transform;
                    newPlane.transform.localPosition = new Vector3(-unitSize / 2, 0, 0);
                    newPlane.transform.localScale = new Vector3(unitSize / 10, 1, unitSize / 10);
                    newPlane.transform.localRotation = Quaternion.Euler(90, -90, 0);
                    newPlane.GetComponent<MeshRenderer>().material = Resources.Load<Material>($"Materials/Wall/{contents.Wall.Value.Item2}");
                }

                if (contents.PlayerCollide)
                {
                    GameObject newWallCollision = new($"MazeWallCollide{x}-{y}");
                    _ = newWallCollision.AddComponent<BoxCollider>();
                    newWallCollision.transform.parent = transform;
                    newWallCollision.transform.position = new Vector3(unitSize * -x, unitSize / 2, unitSize * y);
                    // "Ignore Raycast" - prevents monster from using player collides during line-of-sight checks
                    newWallCollision.layer = 2;
                    newWallCollision.transform.localScale = new Vector3(unitSize, unitSize, unitSize);
                }

                if (contents.MonsterCollide)
                {
                    GameObject newWallCollision = new($"MonsterCollide{x}-{y}");
                    BoxCollider collider = newWallCollision.AddComponent<BoxCollider>();
                    // Setting as a trigger will prevent player colliding, but will still block monster line-of-sight raycasts
                    collider.isTrigger = true;
                    newWallCollision.transform.parent = transform;
                    newWallCollision.transform.position = new Vector3(unitSize * -x, unitSize / 2, unitSize * y);
                    newWallCollision.transform.localScale = new Vector3(unitSize, unitSize, unitSize);
                }
            }
        }

        // Create maze edge
        for (int x = 0; x < level.Dimensions.x; x++)
        {
            GameObject newWall = new($"MazeWallNorthEdge{x}");
            newWall.transform.parent = transform;
            newWall.transform.position = new Vector3(unitSize * -x, unitSize / 2, unitSize * level.Dimensions.y);

            GameObject newPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            newPlane.name = "SouthPlane";
            newPlane.transform.parent = newWall.transform;
            newPlane.transform.localPosition = new Vector3(0, 0, -unitSize / 2);
            newPlane.transform.localScale = new Vector3(unitSize / 10, 1, unitSize / 10);
            newPlane.transform.localRotation = Quaternion.Euler(90, 180, 0);
            newPlane.GetComponent<MeshRenderer>().material = Resources.Load<Material>($"Materials/Wall/{level.EdgeWallTextureName}");

            newWall = new($"MazeWallSouthEdge{x}");
            newWall.transform.parent = transform;
            newWall.transform.position = new Vector3(unitSize * -x, unitSize / 2, -1 * unitSize);

            newPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            newPlane.name = "NorthPlane";
            newPlane.transform.parent = newWall.transform;
            newPlane.transform.localPosition = new Vector3(0, 0, unitSize / 2);
            newPlane.transform.localScale = new Vector3(unitSize / 10, 1, unitSize / 10);
            newPlane.transform.localRotation = Quaternion.Euler(90, 0, 0);
            newPlane.GetComponent<MeshRenderer>().material = Resources.Load<Material>($"Materials/Wall/{level.EdgeWallTextureName}");
        }
        for (int y = 0; y < level.Dimensions.y; y++)
        {
            GameObject newWall = new($"MazeWallEastEdge{y}");
            newWall.transform.parent = transform;
            newWall.transform.position = new Vector3(unitSize, unitSize / 2, unitSize * y);

            GameObject newPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            newPlane.name = "WestPlane";
            newPlane.transform.parent = newWall.transform;
            newPlane.transform.localPosition = new Vector3(-unitSize / 2, 0, 0);
            newPlane.transform.localScale = new Vector3(unitSize / 10, 1, unitSize / 10);
            newPlane.transform.localRotation = Quaternion.Euler(90, -90, 0);
            newPlane.GetComponent<MeshRenderer>().material = Resources.Load<Material>($"Materials/Wall/{level.EdgeWallTextureName}");

            newWall = new($"MazeWallWestEdge{y}");
            newWall.transform.parent = transform;
            newWall.transform.position = new Vector3(-level.Dimensions.x * unitSize, unitSize / 2, unitSize * y);

            newPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            newPlane.name = "EastPlane";
            newPlane.transform.parent = newWall.transform;
            newPlane.transform.localPosition = new Vector3(unitSize / 2, 0, 0);
            newPlane.transform.localScale = new Vector3(unitSize / 10, 1, unitSize / 10);
            newPlane.transform.localRotation = Quaternion.Euler(90, 90, 0);
            newPlane.GetComponent<MeshRenderer>().material = Resources.Load<Material>($"Materials/Wall/{level.EdgeWallTextureName}");
        }
    }

    public override void OnLevelLoad(Level level)
    {
        ReloadWalls(level);
    }
}
