using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class SceneChangeAnimation : MonoBehaviour
{
    public static SceneChangeAnimation Instance;
    public GameObject cloud;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public enum AnimationVariant
    {
        TopDown,
        BottomUp,
        TopMiddle,
        BottomMiddle,
        MiddleBottom,
        MiddleTop
    }

    public void AnimateAndChangeScene(AnimationVariant aniType, Action onComplete = null)
    {
       

        RectTransform cloudRect = cloud.GetComponent<RectTransform>();
       

        float startY;
        Vector2 endPos;

        switch (aniType)
        {
            case AnimationVariant.TopDown:
                startY = 2750;
                cloudRect.anchoredPosition = new Vector2(0, startY);
                endPos = new Vector2(0, -2750);
                break;

            case AnimationVariant.BottomUp:
                startY = -2750;
                cloudRect.anchoredPosition = new Vector2(0, startY);
                endPos = new Vector2(0, 2750);
                break;

            case AnimationVariant.TopMiddle:
                startY = 2750;
                cloudRect.anchoredPosition = new Vector2(0, startY);
                endPos = Vector2.zero;
                break;

            case AnimationVariant.BottomMiddle:
                startY = -2750;
                cloudRect.anchoredPosition = new Vector2(0, startY);
                endPos = Vector2.zero;
                break;
            case AnimationVariant.MiddleBottom:
                startY = 0;
                cloudRect.anchoredPosition = new Vector2(0, startY);
                endPos = new Vector2(0, -2750);
                break;
            case AnimationVariant.MiddleTop:
                startY = 0;
                cloudRect.anchoredPosition = new Vector2(0, startY);
                endPos = new Vector2(0, 2750);
                break;

            default:
                return;
        }

        cloudRect.DOAnchorPos(endPos, 2f)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                onComplete?.Invoke();
            });
    }
}
