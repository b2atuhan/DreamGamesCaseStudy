using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public int currentLevelNumber = 1;
    public LevelData currentLevelData;
    public Image bgImage; // Assign this via the inspector


    public Transform gridParent;

    void Start()
    { 
        if (SceneChangeAnimation.Instance != null)
    {

        Invoke(nameof(TriggerSceneChange), 1.5f); // 1.5 seconds delay
    }

        currentLevelNumber = PlayerPrefs.GetInt("LastPlayedLevel", 1);
        Sprite loadedSprite = Resources.Load<Sprite>($"Art/Backgrounds/BG-{currentLevelNumber}");
        if (loadedSprite != null)
        {
            bgImage.sprite = loadedSprite;
        }
        else
        {
            Debug.LogError($"❌ Background sprite for level {currentLevelNumber} not found!");
        }

        LoadLevel(currentLevelNumber);
        AdjustCameraToGrid();
        UIManager.Instance.UpdateWholeUI(MoveManager.Instance.remainingMoves, currentLevelData.level_number);
        InputBlocker.IsInputEnabled = true;
        GridManager.Instance.InitializeGrid(currentLevelData.grid_width, currentLevelData.grid_height);
        MoveManager.Instance.SetInitialMoves(currentLevelData.move_count);
        CoroutineRunner.Instance.RunCoroutine(DelayedObstacleDisplayUpdate());
    }
    private IEnumerator DelayedObstacleDisplayUpdate()
    {
        yield return new WaitForSeconds(0.2f); // let Unity destroy the object
        ObstacleDisplayUI.Instance.UpdateDisplay();
    }

    public void LoadLevel(int levelNumber)
    {
        TextAsset levelJson = Resources.Load<TextAsset>($"Levels/Level {levelNumber}");

        if (levelJson == null)
        {
            Debug.LogError($"Level {levelNumber} JSON not found!");
            return;
        }

        currentLevelData = JsonUtility.FromJson<LevelData>(levelJson.text);
        Debug.Log($"✅ Loaded level {levelNumber} with {currentLevelData.grid.Count} items.");

        GridUtility.ClearAll();

        GridUtility.GridWidth = currentLevelData.grid_width;
        GridUtility.GridHeight = currentLevelData.grid_height;


        CreateGrid(currentLevelData);
    }

    void CreateGrid(LevelData level)
    {
        for (int y = 0; y < level.grid_height; y++)
        {
            for (int x = 0; x < level.grid_width; x++)
            {
                int index = y * level.grid_width + x;

                if (index >= level.grid.Count)
                    continue;

                string itemCode = level.grid[index];
                CubeItem cube = SpawnCubeFromCode(itemCode, new Vector2Int(x, y));

                if (cube != null)
                {
                    cube.transform.SetParent(gridParent, false); // ✅ assign as child of GridParent
                }
                if (cube != null)
                {
                    GridUtility.RegisterCube(cube, new Vector2Int(x, y));
                }
            }
        }
        CubeManager.Instance.rocketSymbol();

    }

    CubeItem SpawnCubeFromCode(string code, Vector2Int gridPos)
    {
        string prefabName = code switch
        {
            "r" => "CubeRed",
            "g" => "CubeGreen",
            "b" => "CubeBlue",
            "y" => "CubeYellow",
            "rand" => GetRandomColorPrefab(),
            "vro" => "RocketVertical",
            "hro" => "RocketHorizontal",
            "s" => "Stone",
            "v" => "Vase",
            "bo" => "Box",
            _ => null
        };

        if (prefabName == null)
            return null;

        GameObject prefab = Resources.Load<GameObject>($"Prefabs/{prefabName}");
        if (prefab == null)
        {
            Debug.LogError($"❌ Prefab not found for code: {code} → {prefabName}");
            return null;
        }

        Vector2 worldPos = new Vector2(gridPos.x, gridPos.y);
        GameObject obj = Instantiate(prefab, worldPos, Quaternion.identity, gridParent);

        return obj.GetComponent<CubeItem>();
    }

    string GetRandomColorPrefab()
    {
        string[] options = { "CubeRed", "CubeGreen", "CubeBlue", "CubeYellow" };
        return options[Random.Range(0, options.Length)];
    }
    void AdjustCamera()
    {
        float camX = currentLevelData.grid_width / 2f;
        float camY = currentLevelData.grid_height / 2f;
        Camera.main.transform.position = new Vector3(camX, camY, -10);
        Camera.main.orthographicSize = currentLevelData.grid_height / 1f;
    }
    public void AdjustCameraToGrid()
    {
        float border = 0.5f; // Padding around the grid

        int gridWidth = currentLevelData.grid_width;
        int gridHeight = currentLevelData.grid_height;

        float targetAspect = 9f / 16f; // portrait mobile
        float gridAspect = (float)gridWidth / gridHeight;

        if (gridAspect > targetAspect)
        {
            // Grid is wider than screen, base camera size on width
            Camera.main.orthographicSize = (((float)gridWidth / targetAspect) * 1.2f) / 2f + border;
        }
        else
        {
            // Grid is taller than screen, base camera size on height
            Camera.main.orthographicSize = gridHeight / 2f + border;
        }

        // Optionally, center the camera too:
        Camera.main.transform.position = new Vector3(
            (gridWidth - 1) / 2f,
            (gridHeight + 2) / 2f,
            -10f
        );
    }
    private void TriggerSceneChange()
    {
        SceneChangeAnimation.Instance.AnimateAndChangeScene(
            SceneChangeAnimation.AnimationVariant.MiddleTop
        );
    }

}
