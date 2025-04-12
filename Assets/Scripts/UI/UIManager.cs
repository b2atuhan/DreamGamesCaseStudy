using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public TMP_Text movesText;
    public TMP_Text levelText;

    private void Awake()
    {
        Instance = this;

    }

    public void UpdateMoveText(int moves)
    {
        movesText.text = moves.ToString();
    }
    public void UpdateLevelText(int level)
    {
        levelText.text = level.ToString();
    }
    public void UpdateWholeUI(int moves, int level)
    {
        UpdateMoveText(moves);
        UpdateLevelText(level);
    }
}
