using UnityEngine;
using System.Collections.Generic;

public class ObstacleManager : MonoBehaviour
{
    public static ObstacleManager Instance;

    private void Awake()
    {
        Instance = this;
    }


    public bool AreAllObstaclesCleared()
    {
        return GameObject.FindObjectsByType<ObstacleBase>(FindObjectsSortMode.None).Length == 0;
    }
    public List<int> GetObstacleCountsAsList()
    {
        int boxCount = GameObject.FindObjectsByType<BoxObstacle>(FindObjectsSortMode.None).Length;
        int vaseCount = GameObject.FindObjectsByType<VaseObstacle>(FindObjectsSortMode.None).Length;
        int stoneCount = GameObject.FindObjectsByType<StoneObstacle>(FindObjectsSortMode.None).Length;

        return new List<int> { boxCount, vaseCount, stoneCount };
    }



}
