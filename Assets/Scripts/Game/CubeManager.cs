using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class CubeManager : MonoBehaviour
{
    public static CubeManager Instance;
    public Transform gridParent;

    private void Awake()
    {
        Instance = this;
    }

    // This will be the method we call when a cube is tapped
    public void HandleCubeTap(Cube tappedCube)
    {
        List<CubeItem> connected = GetConnectedCubes(tappedCube);

        if (connected.Count < 2)
            return;
        MoveManager.Instance.UseMove(); // This will trigger CheckGameState

        bool createRocket = connected.Count >= 4;
        Vector2Int rocketPos = tappedCube.GridPosition;

        // Track which cubes are blasted
        HashSet<Vector2Int> blastPositions = new HashSet<Vector2Int>();

        foreach (CubeItem cube in connected)
        {
            Vector2Int pos = cube.GridPosition;
            blastPositions.Add(pos);
            GridUtility.RemoveCube(pos);

            Vector3 worldPos = new Vector3(pos.x, pos.y, 0);

            // Check if the CubeItem is a Cube and get the color type
            if (cube is Cube typedCube)
            {
                string colorName = typedCube.colorType.ToString(); // "Red", "Green", etc.
                string path = $"Particles/{colorName}CubeDestroy";
                GameObject prefab = Resources.Load<GameObject>(path);

                if (prefab != null)
                {
                    GameObject effect = Instantiate(prefab, worldPos, Quaternion.identity);
                }
                else
                {
                    Debug.LogWarning($"❌ Could not load particle prefab at: {path}");
                }
            }
            else
            {
                Debug.LogWarning("❌ CubeItem was not of type Cube. Skipping color particle.");
            }

            Destroy(cube.gameObject);
        }


        // ✅ Deal damage to obstacles around blast positions
        foreach (Vector2Int blastPos in blastPositions)
        {
            foreach (Vector2Int dir in Directions)
            {
                Vector2Int neighborPos = blastPos + dir;
                CubeItem neighbor = GridUtility.GetCubeAt(neighborPos);

                if (neighbor is ObstacleBase obstacle)
                {
                    // Check obstacle type and whether it's blast-sensitive
                    if (obstacle is BoxObstacle || obstacle is VaseObstacle)
                    {
                        obstacle.TakeDamage();
                    }
                }
            }
        }


        if (createRocket)
        {
            Debug.Log($"🚀 Creating rocket at {rocketPos}");
            CubeItem rocket = SpawnRocket(rocketPos);
            GridUtility.RegisterCube(rocket, rocketPos);
        }

        // ✅ ALWAYS drop after match or rocket
        Invoke(nameof(DropAndRefill), 0.1f);
        Invoke(nameof(PrintGrid), 0.15f);

    }

    public bool PossibleMoveExists()
    {
        for (int x = 0; x < GridUtility.GridWidth; x++)
        {
            for (int y = 0; y < GridUtility.GridHeight; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                CubeItem item = GridUtility.GetCubeAt(pos);

                if (item is Cube cube)
                {
                    List<CubeItem> connected = GetConnectedCubes(cube);
                    if (connected.Count >= 2)
                    {
                        return true;
                    }
                }
                else if (item is Rocket rocket)
                {
                    return true;
                }
            }
        }
        return false; // ✅ Return false if no possible move found

    }
    public Dictionary<Vector2Int, CubeColorType> GetPotentialRocketCandidates()
    {
        Dictionary<Vector2Int, CubeColorType> rocketCandidates = new();

        for (int x = 0; x < GridUtility.GridWidth; x++)
        {
            for (int y = 0; y < GridUtility.GridHeight; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                CubeItem item = GridUtility.GetCubeAt(pos);

                if (item is Cube cube)
                {
                    List<CubeItem> connected = GetConnectedCubes(cube);

                    if (connected.Count >= 4)
                    {
                        // Make sure the position isn't already added
                        if (!rocketCandidates.ContainsKey(pos))
                        {
                            rocketCandidates.Add(pos, cube.colorType);
                        }
                    }
                }
            }
        }

        return rocketCandidates;
    }
    public void ClearAllRocketIcons()
    {
        for (int x = 0; x < GridUtility.GridWidth; x++)
        {
            for (int y = 0; y < GridUtility.GridHeight; y++)
            {
                CubeItem item = GridUtility.GetCubeAt(new Vector2Int(x, y));
                if (item is Cube cube)
                    cube.ShowRocketIcon(false);
            }
        }
    }

    public void MarkRocketCandidates(Dictionary<Vector2Int, CubeColorType> colorMap)
    {
        foreach (var entry in colorMap)
        {
            Vector2Int pos = entry.Key;
            CubeItem item = GridUtility.GetCubeAt(pos);

            if (item is Cube cube)
            {
                cube.ShowRocketIcon(true, 1f); // show with full opacity

                Debug.Log($"🚀 Marked {cube.colorType} cube at {pos} as rocket candidate");
            }
        }
    }
    public void rocketSymbol()
    {
        ClearAllRocketIcons();
        MarkRocketCandidates(GetPotentialRocketCandidates());
    }


    public void Shuffle()
    {
        // 1. Collect all Cube positions (excluding obstacles and rockets)
        List<Vector2Int> cubePositions = new List<Vector2Int>();

        for (int x = 0; x < GridUtility.GridWidth; x++)
        {
            for (int y = 0; y < GridUtility.GridHeight; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                CubeItem item = GridUtility.GetCubeAt(pos);

                if (item is Cube)
                {
                    cubePositions.Add(pos);
                }
            }
        }

        int total = cubePositions.Count;
        int swapsToDo = Mathf.FloorToInt(total * 0.2f);

        // 2. Perform random swaps
        for (int i = 0; i < swapsToDo; i++)
        {
            Vector2Int posA = cubePositions[Random.Range(0, cubePositions.Count)];
            Vector2Int posB = cubePositions[Random.Range(0, cubePositions.Count)];

            // Make sure they're not the same
            if (posA == posB)
            {
                i--;
                continue;
            }

            CubeItem cubeA = GridUtility.GetCubeAt(posA);
            CubeItem cubeB = GridUtility.GetCubeAt(posB);

            if (cubeA == null || cubeB == null) continue;

            // Swap their positions in grid utility
            GridUtility.RegisterCube(cubeA, posB);
            GridUtility.RegisterCube(cubeB, posA);

            // Animate position change
            cubeA.transform.DOMove(new Vector3(posB.x, posB.y, 0), 0.25f).SetEase(Ease.InOutQuad);
            cubeB.transform.DOMove(new Vector3(posA.x, posA.y, 0), 0.25f).SetEase(Ease.InOutQuad);
        }

        Debug.Log($"🔀 Shuffled {swapsToDo * 2} cubes (20% swap).");
    }


    List<CubeItem> GetConnectedCubes(CubeItem start)
    {
        List<CubeItem> result = new List<CubeItem>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Queue<CubeItem> toCheck = new Queue<CubeItem>();

        if (start is not Cube startCube)
            return result;

        toCheck.Enqueue(start);

        while (toCheck.Count > 0)
        {
            CubeItem current = toCheck.Dequeue();
            Vector2Int pos = current.GridPosition;

            if (visited.Contains(pos))
                continue;

            visited.Add(pos);
            result.Add(current);

            foreach (Vector2Int dir in Directions)
            {
                Vector2Int neighborPos = pos + dir;
                CubeItem neighbor = GridUtility.GetCubeAt(neighborPos);

                if (neighbor is Cube neighborCube &&
                    neighborCube.colorType == startCube.colorType &&
                    !visited.Contains(neighborPos))
                {
                    toCheck.Enqueue(neighborCube);
                }
            }
        }

        return result;
    }

    private List<Vector2Int> Directions => new List<Vector2Int>
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    public void DropAndRefill()
    {
        int width = GridUtility.GridWidth;
        int height = GridUtility.GridHeight;

        // ✅ 1. FALLING PHASE (TOP TO BOTTOM per column)
        for (int x = 0; x < width; x++)
        {
            int targetY = 0;

            for (int y = 0; y < height; y++)
            {
                Vector2Int currentPos = new Vector2Int(x, y);
                CubeItem current = GridUtility.GetCubeAt(currentPos);

                if (current == null)
                    continue;

                if (!current.CanFall)
                {
                    // Skip non-falling, lock its position
                    if (y != targetY)
                    {
                        GridUtility.RemoveCube(currentPos);
                        GridUtility.RegisterCube(current, new Vector2Int(x, y));
                    }
                    targetY = y + 1;
                    continue;
                }

                if (y != targetY)
                {
                    GridUtility.RemoveCube(currentPos);
                    GridUtility.RegisterCube(current, new Vector2Int(x, targetY));
                    current.transform.DOMove(new Vector2(x, targetY), 0.15f).SetEase(Ease.InQuad);
                }

                targetY++;
            }
        }

        // ✅ 2. SLIDING PHASE (TOP TO BOTTOM)
        for (int y = height - 2; y >= 0; y--) // skip topmost row
        {
            for (int x = 0; x < width; x++)
            {
                Vector2Int currentPos = new Vector2Int(x, y);
                CubeItem current = GridUtility.GetCubeAt(currentPos);

                // Skip filled cells
                if (current != null) continue;

                // Check if above is non-fallable (like stone or box)
                Vector2Int abovePos = new Vector2Int(x, y + 1);
                CubeItem above = GridUtility.GetCubeAt(abovePos);

                if (above == null || above.CanFall)
                    continue; // either nothing above or it's fallable = do not slide

                // Try to slide from left or right
                Vector2Int? slideFrom = TrySlideFromSides(x, y);
                if (slideFrom.HasValue)
                {
                    CubeItem sideCube = GridUtility.GetCubeAt(slideFrom.Value);
                    GridUtility.RemoveCube(slideFrom.Value);
                    GridUtility.RegisterCube(sideCube, currentPos);
                    sideCube.transform.DOMove(new Vector2(x, y), 0.15f).SetEase(Ease.InOutQuad);
                }
            }
        }

        // ✅ 3. REFILL PHASE (fill from top)
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (GridUtility.GetCubeAt(pos) == null)
                {
                    CubeItem newCube = SpawnRandomCube(x, y + height); // spawn above view
                    GridUtility.RegisterCube(newCube, pos);
                    newCube.transform.position = new Vector2(x, y + height);
                    newCube.transform.DOMove(new Vector2(x, y), 0.25f).SetEase(Ease.OutQuad);
                }
            }
        }

        // 🧾 Optional debug output
        PrintGrid();
        CubeManager.Instance.rocketSymbol();

    }






    CubeItem SpawnRandomCube(int x, int y)
    {
        string[] prefabNames = { "CubeRed", "CubeGreen", "CubeBlue", "CubeYellow" };
        string randomPrefab = prefabNames[Random.Range(0, prefabNames.Length)];

        GameObject prefab = Resources.Load<GameObject>($"Prefabs/{randomPrefab}");
        if (prefab == null)
        {
            Debug.LogError($"❌ Could not load prefab: {randomPrefab}. Check Resources/Prefabs folder!");
            return null;
        }

        Vector2 spawnPos = new Vector2(x, y); // or convert to world position if needed
        GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity);

        if (gridParent != null)
        {
            Debug.Log("✅ GridParent assigned, setting parent...");
            obj.transform.SetParent(gridParent, false);
        }
        else
        {
            Debug.LogWarning("⚠️ gridParent is null! Did you assign it in the Inspector?");
        }

        CubeItem cubeItem = obj.GetComponent<CubeItem>();
        if (cubeItem == null)
        {
            Debug.LogError($"❌ Missing CubeItem script on prefab: {randomPrefab}");
        }

        return cubeItem;
    }

    private Vector2Int? TrySlideFromSides(int x, int y)
    {
        int width = GridUtility.GridWidth;

        for (int dx = -1; dx <= 1; dx += 2)
        {
            int sx = x + dx;
            if (sx < 0 || sx >= width) continue;

            Vector2Int sidePos = new Vector2Int(sx, y);
            CubeItem side = GridUtility.GetCubeAt(sidePos);

            if (side != null && side.CanFall)
            {
                return sidePos;
            }
        }

        return null;
    }
    public void PrintGrid()
    {
        int width = GridUtility.GridWidth;
        int height = GridUtility.GridHeight;

        string output = "\n=== Current Grid State ===\n";

        for (int y = height - 1; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                CubeItem item = GridUtility.GetCubeAt(new Vector2Int(x, y));

                if (item is Cube cube)
                {
                    output += cube.colorType.ToString()[0] + " "; // R, G, B, Y
                }
                else if (item is ObstacleBase)
                {
                    output += "O ";
                }
                else if (item is Rocket rocket)
                {
                    output += "🚀 ";
                }
                else
                {
                    output += ". ";
                }
            }
            output += "\n";
        }

        Debug.Log(output);
    }


    private bool CanFallFromAbove(int x, int y)
    {
        int height = GridUtility.GridHeight;

        for (int checkY = y + 1; checkY < height; checkY++)
        {
            CubeItem item = GridUtility.GetCubeAt(new Vector2Int(x, checkY));

            if (item == null)
                continue;

            // 🚫 if it can't fall (e.g. Stone), we’re blocked — stop here
            if (!item.CanFall)
                return false;

            // ✅ there's something fallable above — don’t allow slide
            return true;
        }

        return false; // nothing above
    }






    public CubeItem SpawnRocket(Vector2Int pos)
    {
        string prefabName = Random.value < 0.5f ? "RocketHorizontal" : "RocketVertical";
        GameObject prefab = Resources.Load<GameObject>($"Prefabs/{prefabName}");

        if (prefab == null)
        {
            Debug.LogError($"❌ Rocket prefab '{prefabName}' not found in Resources/Prefabs!");
            return null;
        }

        GameObject obj = Instantiate(prefab, new Vector2(pos.x, pos.y), Quaternion.identity);

        GameObject gridParent = GameObject.Find("GridParent");
        if (gridParent != null)
            obj.transform.SetParent(gridParent.transform);
        else
            Debug.LogWarning("⚠️ 'GridParent' not found during rocket spawn!");

        Rocket rocket = obj.GetComponent<Rocket>();
        if (rocket == null)
            Debug.LogError("❌ Spawned rocket does not have Rocket script attached!");

        rocket.direction = prefabName == "RocketHorizontal" ? RocketDirection.Horizontal : RocketDirection.Vertical;

        return rocket;
    }
    



}
