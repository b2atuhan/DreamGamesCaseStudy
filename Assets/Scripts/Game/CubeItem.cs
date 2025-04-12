using UnityEngine;

public class CubeItem : MonoBehaviour
{
    public Vector2Int GridPosition { get; set; }
    public GridManager GridManager { get; set; }
    public virtual bool CanFall => true; // Default: items like cubes and rockets can fall

    public virtual void OnTapped()
    {
        // To be overridden
    }

    public void Initialize(Vector2Int position, GridManager manager)
    {
        GridPosition = position;
        GridManager = manager;
    }
}
