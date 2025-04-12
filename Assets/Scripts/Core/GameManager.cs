using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject winParticles;
    public GameObject failPopup;
    [SerializeField] private RectTransform uiTransform;
    [SerializeField] private RectTransform bgTransform;

    public float ConfettiOffsetY = 0.5f;
    public float ConfettiOffsetX = 0.5f;

    private bool checkedState = false;

    private void Awake()
    {
        Instance = this;

    }

    public void CheckGameState()
    {
        if (ObstacleManager.Instance.AreAllObstaclesCleared() && !checkedState )
        {
            WinLevel();
        }
        else if (MoveManager.Instance.remainingMoves <= 0)
        {
            LoseLevel();
        }
    }



    private void WinLevel()
    {

        if (checkedState) return;
        checkedState = true;
        
        InputBlocker.IsInputEnabled = false; // 🔒 block grid taps


        Vector3 centerScreen = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(centerScreen);
        worldPos.z = 0;
        Quaternion midRotation = Quaternion.Euler(0, 0, 60);
        Quaternion leftRotation = Quaternion.Euler(0, 0, 45);
        Quaternion rightRotation = Quaternion.Euler(0, 0, 75);

        Vector3 midPos = new Vector3(worldPos.x, worldPos.y + ConfettiOffsetY, worldPos.z);
        Vector3 leftPos = new Vector3(worldPos.x - ConfettiOffsetX, worldPos.y + ConfettiOffsetY, worldPos.z);
        Vector3 rightPos = new Vector3(worldPos.x + ConfettiOffsetX, worldPos.y + ConfettiOffsetY, worldPos.z);

        Instantiate(winParticles, midPos, midRotation);
        Instantiate(winParticles, leftPos, leftRotation);
        Instantiate(winParticles, rightPos, rightRotation);




        PlayerPrefs.SetInt("LastPlayedLevel", PlayerPrefs.GetInt("LastPlayedLevel", 1) + 1);

        Invoke("AnimateDown",2f); // Give time for particle to show
         // Set checkedState to true to prevent multiple calls
    }
    private void AnimateDown()
    {
        SceneChangeAnimation.Instance.AnimateAndChangeScene(
            SceneChangeAnimation.AnimationVariant.TopMiddle,
            () => SceneManager.LoadScene("MainScene")
        );
    }

    private void LoseLevel()
    {
        InputBlocker.IsInputEnabled = false;

        failPopup.SetActive(true); // Show the popup

        // Set both to small scale
        uiTransform.localScale = Vector3.one * 0.1f;
        bgTransform.localScale = Vector3.one * 0.1f;

        // Animate scaling up
        uiTransform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
        bgTransform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
    }
    public void TryAgain()
    {
        SceneChangeAnimation.Instance.AnimateAndChangeScene(
            SceneChangeAnimation.AnimationVariant.TopMiddle,
            () => SceneManager.LoadScene("LevelScene")
        );
    }

    public void ReturnToMainScene()
    {
        SceneChangeAnimation.Instance.AnimateAndChangeScene(
            SceneChangeAnimation.AnimationVariant.TopMiddle,
            () => SceneManager.LoadScene("MainScene")
        );
    }
}
