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
    
    private bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }
    
    
    private List<Vector2Int> Directions => new List<Vector2Int>
{
    Vector2Int.up,
    Vector2Int.down,
    Vector2Int.left,
    Vector2Int.right
};


}
