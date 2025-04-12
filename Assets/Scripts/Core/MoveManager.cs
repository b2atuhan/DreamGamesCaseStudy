using UnityEngine;
using System.Collections;
public class MoveManager : MonoBehaviour
{
    public static MoveManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public int remainingMoves;

    public void UseMove()
    {
        remainingMoves--;
        UIManager.Instance.UpdateMoveText(remainingMoves);
        StartCoroutine(DelayedCheckGameState());
    }
    private IEnumerator DelayedCheckGameState()
    {
        yield return new WaitForSeconds(0.4f);
        GameManager.Instance.CheckGameState();
    }

    public void SetInitialMoves(int count)
    {
        remainingMoves = count;
        UIManager.Instance.UpdateMoveText(remainingMoves);
    }
}
