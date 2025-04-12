using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public int CurrentLevel { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Load saved level or default
            CurrentLevel = PlayerPrefs.GetInt("LastPlayedLevel", 1);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool AllLevelsFinished()
    {
        return CurrentLevel > 10; // or whatever your max level is
    }
}
