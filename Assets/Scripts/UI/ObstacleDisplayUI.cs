using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Pool;

public class ObstacleDisplayUI : MonoBehaviour
{
    public static ObstacleDisplayUI Instance;
    public RectTransform container;
    public GameObject obstacleItemPrefab;

    private ObjectPool<GameObject> obstacleItemPool;
    private List<GameObject> activeItems = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            obstacleItemPool = new ObjectPool<GameObject>(
                createFunc: () => Instantiate(obstacleItemPrefab),
                actionOnGet: item => item.SetActive(true),
                actionOnRelease: item => item.SetActive(false),
                actionOnDestroy: Destroy,
                collectionCheck: false,
                defaultCapacity: 5,
                maxSize: 10
            );
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateDisplay()
    {
        Debug.Log("🔄 UpdateDisplay1 called...");
        if (container == null)
        {
            container = GameObject.Find("Container")?.GetComponent<RectTransform>();
            if (container == null)
            {
                Debug.LogError("❌ Obstacle container not found in scene! Aborting UpdateDisplay.");
                return;
            }
        }

        // Release all previous items
        foreach (var item in activeItems)
        {
            obstacleItemPool.Release(item);
        }
        activeItems.Clear();

        Debug.Log("🔍 UpdateDisplay2 called...");
        List<int> obstacleCounts = ObstacleManager.Instance.GetObstacleCountsAsList();
        List<Sprite> obstacleSprites = new List<Sprite>();
        List<int> countsToShow = new List<int>();

        Sprite boxSprite = Resources.Load<Sprite>("Art/Blocks/Crate4");
        Sprite vaseSprite = Resources.Load<Sprite>("Art/Blocks/Vase_2");
        Sprite stoneSprite = Resources.Load<Sprite>("Art/Blocks/Stone");

        if (obstacleCounts[0] > 0)
        {
            obstacleSprites.Add(boxSprite);
            countsToShow.Add(obstacleCounts[0]);
        }
        if (obstacleCounts[1] > 0)
        {
            obstacleSprites.Add(vaseSprite);
            countsToShow.Add(obstacleCounts[1]);
        }
        if (obstacleCounts[2] > 0)
        {
            obstacleSprites.Add(stoneSprite);
            countsToShow.Add(obstacleCounts[2]);
        }

        int count = obstacleSprites.Count;
        Vector2 baseVector = new Vector2(160, 160);
        Vector2 size = new Vector2(80, 80);

        if (count == 1)
        {
            GameObject item = obstacleItemPool.Get();
            item.transform.SetParent(container, false);
            activeItems.Add(item);

            RectTransform rt = item.GetComponent<RectTransform>();
            rt.sizeDelta = baseVector;
            rt.anchoredPosition = Vector2.zero;
            item.GetComponent<Image>().sprite = obstacleSprites[0];

            TMP_Text countText = item.GetComponentInChildren<TMP_Text>();
            if (countText != null)
                countText.text = countsToShow[0].ToString();
        }
        else if (count == 2)
        {
            float spacing = 20f;

            for (int i = 0; i < 2; i++)
            {
                GameObject item = obstacleItemPool.Get();
                item.transform.SetParent(container, false);
                activeItems.Add(item);

                item.transform.localScale = Vector3.one * 0.5f;
                RectTransform rt = item.GetComponent<RectTransform>();
                rt.sizeDelta = baseVector;
                rt.anchoredPosition = new Vector2((i == 0 ? -1 : 1) * (size.x / 2 + spacing / 2f), 0);
                item.GetComponent<Image>().sprite = obstacleSprites[i];

                TMP_Text countText = item.GetComponentInChildren<TMP_Text>();
                if (countText != null)
                    countText.text = countsToShow[i].ToString();
            }
        }
        else if (count >= 3)
        {
            float horizontalSpacing = 20f;
            float verticalSpacing = 20f;

            // Top Left
            GameObject topLeft = obstacleItemPool.Get();
            topLeft.transform.SetParent(container, false);
            activeItems.Add(topLeft);

            topLeft.transform.localScale = Vector3.one * 0.42f;
            RectTransform rtTL = topLeft.GetComponent<RectTransform>();
            rtTL.sizeDelta = baseVector;
            rtTL.anchoredPosition = new Vector2(-size.x / 2 - horizontalSpacing / 2f, verticalSpacing / 2f + size.y / 2f);
            topLeft.GetComponent<Image>().sprite = obstacleSprites[0];
            topLeft.GetComponentInChildren<TMP_Text>().text = countsToShow[0].ToString();

            // Top Right
            GameObject topRight = obstacleItemPool.Get();
            topRight.transform.SetParent(container, false);
            activeItems.Add(topRight);

            topRight.transform.localScale = Vector3.one * 0.42f;
            RectTransform rtTR = topRight.GetComponent<RectTransform>();
            rtTR.sizeDelta = baseVector;
            rtTR.anchoredPosition = new Vector2(size.x / 2 + horizontalSpacing / 2f, verticalSpacing / 2f + size.y / 2f);
            topRight.GetComponent<Image>().sprite = obstacleSprites[1];
            topRight.GetComponentInChildren<TMP_Text>().text = countsToShow[1].ToString();

            // Bottom Center
            GameObject bottom = obstacleItemPool.Get();
            bottom.transform.SetParent(container, false);
            activeItems.Add(bottom);

            bottom.transform.localScale = Vector3.one * 0.42f;
            RectTransform rtB = bottom.GetComponent<RectTransform>();
            rtB.sizeDelta = baseVector;
            rtB.anchoredPosition = new Vector2(0, -verticalSpacing / 2f - size.y / 2f);
            bottom.GetComponent<Image>().sprite = obstacleSprites[2];
            bottom.GetComponentInChildren<TMP_Text>().text = countsToShow[2].ToString();
        }
    }
}
