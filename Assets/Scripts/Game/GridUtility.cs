using System.Collections.Generic;
using UnityEngine;

public static class GridUtility
{
    public static int GridWidth = 6; // Set dynamically from LevelData
    public static int GridHeight = 8;

    private static Dictionary<Vector2Int, CubeItem> cubeGrid = new();

    public static void RegisterCube(CubeItem cube, Vector2Int position)
    {
        cubeGrid[position] = cube;
        cube.GridPosition = position;

        // 🧩 Set sorting order by Y row (row 0 = order 0, row 1 = order 1, etc.)
        SpriteRenderer sr = cube.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder = position.y;
        }
    }

    public static void RemoveCube(Vector2Int position)
    {
        if (cubeGrid.ContainsKey(position))
        {
            cubeGrid.Remove(position);
        }
    }

    public static CubeItem GetCubeAt(Vector2Int position)
    {
        return cubeGrid.ContainsKey(position) ? cubeGrid[position] : null;
    }

    public static void ClearAll()
    {
        cubeGrid.Clear();
    }

    public static bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < GridWidth && pos.y < GridHeight;
    }
}
