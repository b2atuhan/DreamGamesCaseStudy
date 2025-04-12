using UnityEngine;
using System.Collections;
public class CoroutineRunner : MonoBehaviour
{
    public static CoroutineRunner Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RunCoroutine(IEnumerator routine)
    {
        StartCoroutine(routine);
    }
}
