using UnityEditor;
using UnityEngine;

public class LevelSetterWindow : EditorWindow
{
    private int levelNumber = 1;

    [MenuItem("Tools/Set Level Progress")]
    public static void ShowWindow()
    {
        // Open the custom window
        GetWindow<LevelSetterWindow>("Set Level Progress");
    }

    private void OnGUI()
    {
        GUILayout.Label("Set Level Progress", EditorStyles.boldLabel);

        // Input field for specifying the level
        levelNumber = EditorGUILayout.IntField("Level Number", levelNumber);

        // When the button is pressed, update the PlayerPrefs
        if (GUILayout.Button("Set Level"))
        {
            PlayerPrefs.SetInt("LastPlayedLevel", levelNumber);
            Debug.Log("Progress set to Level " + levelNumber);
        }
    }
    [MenuItem("Tools/Reset to Level 1")]
    public static void SetToLevel1()
    {
        PlayerPrefs.SetInt("LastPlayedLevel", 1);
        Debug.Log("Progress reset to Level 1.");
    }
    [MenuItem("Tools/Reset to Level 5")]
    public static void SetToLevel5()
    {
        PlayerPrefs.SetInt("LastPlayedLevel", 5);
        Debug.Log("Progress reset to Level 5.");
    }
    [MenuItem("Tools/Reset to Level 9")]
    public static void SetToLevel10()
    {
        PlayerPrefs.SetInt("LastPlayedLevel", 9);
        Debug.Log("Progress reset to Level 9.");
    }
}