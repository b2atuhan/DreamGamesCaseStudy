using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    static public int height;
    static public int width;

    private CubeItem[,] grid;

    private void Awake()
    {
        Instance = this;
    }
    public void InitializeGrid(int w, int h)
    {
        width = w;
        height = h;
        grid = new CubeItem[width, height];
    }

    public void SetItemAt(Vector2Int position, CubeItem item)
    {
        grid[position.x, position.y] = item;
        item.GridPosition = position;
        item.GridManager = this;
    }

    public void TryBlastCubesFrom(Cube startCube)
    {
        List<Cube> matchingGroup = GetConnectedCubes(startCube);

        if (matchingGroup.Count < 2)
        {
            Debug.Log("Not enough cubes to blast!");
            return;
        }

        if (matchingGroup.Count >= 4)
        {
            Debug.Log("Create a rocket!");
            // You’ll add rocket logic here later
        }

        foreach (Cube cube in matchingGroup)
        {
            Vector2Int pos = cube.GridPosition;

            GridUtility.RemoveCube(pos); // Clears cube from grid array
            Destroy(cube.gameObject);    // Removes from scene
        }

        // After destruction, drop cubes and refill
        DropAndRefill();
    }


    private List<Cube> GetConnectedCubes(Cube start)
    {
        List<Cube> result = new List<Cube>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Queue<Cube> queue = new Queue<Cube>();

        queue.Enqueue(start);
        visited.Add(start.GridPosition);

        while (queue.Count > 0)
        {
            Cube current = queue.Dequeue();
            result.Add(current);

            foreach (Vector2Int dir in Directions)
            {
                Vector2Int neighborPos = current.GridPosition + dir;
                CubeItem item = GridUtility.GetCubeAt(neighborPos);

                if (item is Cube neighbor &&
                    neighbor.colorType == start.colorType &&
                    !visited.Contains(neighbor.GridPosition))
                {
                    visited.Add(neighbor.GridPosition);
                    queue.Enqueue(neighbor);
                }
            }
        }

        return result;
    }


    private List<Vector2Int> GetDirections()
    {
        return new List<Vector2Int> {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };
    }

    private bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }
    public void DropAndRefill()
    {
        int width = GridUtility.GridWidth;
        int height = GridUtility.GridHeight;

        for (int x = 0; x < width; x++)
        {
            int targetY = 0;

            for (int y = 0; y < height; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                CubeItem cube = GridUtility.GetCubeAt(pos);

                if (cube != null)
                {
                    if (y != targetY)
                    {
                        GridUtility.RemoveCube(pos);
                        Vector2Int newPos = new Vector2Int(x, targetY);
                        GridUtility.RegisterCube(cube, newPos);

                        Vector2 newWorldPos = new Vector2(x, targetY);
                        cube.transform.position = newWorldPos; // Optional: Animate
                    }
                    targetY++;
                }
            }

            // Refill from top
            for (int y = targetY; y < height; y++)
            {
                CubeItem newCube = SpawnRandomCube(x, y);
                GridUtility.RegisterCube(newCube, new Vector2Int(x, y));
            }
        }
    }
    CubeItem SpawnRandomCube(int x, int y)
    {
        string[] prefabNames = { "CubeRed", "CubeGreen", "CubeBlue", "CubeYellow" };
        string randomPrefab = prefabNames[Random.Range(0, prefabNames.Length)];

        GameObject prefab = Resources.Load<GameObject>($"Prefabs/{randomPrefab}");
        GameObject obj = Instantiate(prefab, new Vector2(x, y), Quaternion.identity);
        obj.transform.SetParent(GameObject.Find("GridParent").transform); // Optional

        return obj.GetComponent<CubeItem>();
    }
    private List<Vector2Int> Directions => new List<Vector2Int>
{
    Vector2Int.up,
    Vector2Int.down,
    Vector2Int.left,
    Vector2Int.right
};


}
