using UnityEngine;
using UnityEngine.UI;

public class UIFrameFitter : MonoBehaviour
{
    public RectTransform frameRect;         // Drag your UI Image here

    int width = GridUtility.GridWidth;
    int height = GridUtility.GridHeight;    // The level size
    public float padding = 20f;             // UI pixels of space around the grid

    void Start()
    {
        FitFrameToGrid();
    }

    void FitFrameToGrid()
    {
        float gridWidth = width;
        float gridHeight = height;

        float heightSize = 87f; // Assume 100 pixels per grid cell (you can adjust)
        float widthSize = 89f; // Assume 100 pixels per grid cell (you can adjust)
        float frameWidth = 6 * widthSize; ;
        float frameHeight = gridHeight * heightSize;

        frameRect.sizeDelta = new Vector2(frameWidth, frameHeight);
        float offsetY = -gridHeight * heightSize / 4f; // push down based on size

        frameRect.anchoredPosition = new Vector2(0, offsetY);
    }
}
