using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MainMenuController : MonoBehaviour
{
    public Button levelButton;
    public GameObject levelButtonTextGO; // Assign TMP object in inspector
    public Image backgroundImage; // Assign BG Image in inspector

    private TextMeshProUGUI levelButtonText;

    private void Start()
    {
        SceneChangeAnimation.Instance.AnimateAndChangeScene(
            SceneChangeAnimation.AnimationVariant.MiddleBottom
        );
        levelButtonText = levelButtonTextGO.GetComponent<TextMeshProUGUI>();
        if (levelButtonText == null)
        {
            Debug.LogError("❌ TMP component not found on levelButtonTextGO!");
        }

        int currentLevel = PlayerPrefs.GetInt("LastPlayedLevel", 1);
        Sprite loadedSprite = Resources.Load<Sprite>($"Sprites/BG-{currentLevel}");

        if (backgroundImage != null && loadedSprite != null)
        {
            backgroundImage.sprite = loadedSprite;
        }
        else
        {
            Debug.LogWarning($"⚠️ Background image or sprite for BG-{currentLevel} not found.");
        }

        levelButton.onClick.AddListener(OnLevelButtonClick);
    }

    private void Update()
    {
        if (levelButtonText == null) return;

        if (LevelManager.Instance.AllLevelsFinished())
        {
            levelButtonText.text = "Finished";
            levelButton.interactable = false;
        }
        else
        {
            int level = PlayerPrefs.GetInt("LastPlayedLevel", 1);
            levelButtonText.text = "Level " + level;
        }
    }

    private void OnLevelButtonClick()
    {
        SceneChangeAnimation.Instance.AnimateAndChangeScene(
            SceneChangeAnimation.AnimationVariant.BottomMiddle,
            () => SceneManager.LoadScene("LevelScene")
        );
    }
}
