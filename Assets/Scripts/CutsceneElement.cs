using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

[RequireComponent(typeof(RectTransform))]
public class CutsceneElement : MonoBehaviour
{
    public enum EntryAnimationType
    {
        None,
        FadeIn,
        SlideFromRight,
        SlideFromTop,
        FadeInFromBottom
    }

    public enum ExitAnimationType
    {
        None,
        FadeOut,
        DisappearInstant,
        SlideToLeft,        // 필요하면 추가
        SlideToBottom       // 필요하면 추가
    }

    [Header("등장 애니메이션 설정")]
    public EntryAnimationType entryAnimation = EntryAnimationType.FadeIn;
    public float entryDuration = 3f;
    public float entryMoveDistance = 300f;
    public float entryDelay = 3f;

    [Header("퇴장 애니메이션 설정")]
    public ExitAnimationType exitAnimation = ExitAnimationType.FadeOut;
    public float exitDuration = 2f;
    public float exitMoveDistance = 300f;
    public float exitDelay = 2f;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;

    public Action onComplete;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        originalPosition = rectTransform.anchoredPosition;
    }

    public void Init()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        originalPosition = rectTransform.anchoredPosition;
    }

    public void PlayEntryAnimation(Action onCompleteCallback = null)
    {
        canvasGroup.alpha = 0f;
        rectTransform.anchoredPosition = originalPosition;

        Sequence seq = DOTween.Sequence();

        switch (entryAnimation)
        {
            case EntryAnimationType.FadeIn:
                seq.Append(canvasGroup.DOFade(1f, entryDuration).SetDelay(entryDelay));
                break;

            case EntryAnimationType.SlideFromRight:
                rectTransform.anchoredPosition = originalPosition + Vector2.right * entryMoveDistance;
                seq.Append(canvasGroup.DOFade(1f, entryDuration).SetDelay(entryDelay));
                seq.Join(rectTransform.DOAnchorPos(originalPosition, entryDuration).SetEase(Ease.OutCubic).SetDelay(entryDelay));
                break;

            case EntryAnimationType.SlideFromTop:
                rectTransform.anchoredPosition = originalPosition + Vector2.up * entryMoveDistance;
                seq.Append(canvasGroup.DOFade(1f, entryDuration).SetDelay(entryDelay));
                seq.Join(rectTransform.DOAnchorPos(originalPosition, entryDuration).SetEase(Ease.OutCubic).SetDelay(entryDelay));
                break;

            case EntryAnimationType.FadeInFromBottom:
                rectTransform.anchoredPosition = originalPosition - Vector2.up * entryMoveDistance;
                seq.Append(canvasGroup.DOFade(1f, entryDuration).SetDelay(entryDelay));
                seq.Join(rectTransform.DOAnchorPos(originalPosition, entryDuration).SetEase(Ease.OutCubic).SetDelay(entryDelay));
                break;

            case EntryAnimationType.None:
                canvasGroup.alpha = 1f;
                break;
        }

        seq.OnComplete(() => {
            onComplete?.Invoke();           // ✅ 항상 호출
            onCompleteCallback?.Invoke();
        });
    }

    public void PlayExitAnimation(Action onCompleteCallback = null)
    {
        Sequence seq = DOTween.Sequence();

        switch (exitAnimation)
        {
            case ExitAnimationType.FadeOut:
                seq.Append(canvasGroup.DOFade(0f, exitDuration).SetDelay(exitDelay));
                break;

            case ExitAnimationType.DisappearInstant:
                canvasGroup.alpha = 0f;
                onCompleteCallback?.Invoke();
                return;

            // (선택) 좌측/아래로 슬라이드 하면서 사라지는 효과도 추가 가능
            // case ExitAnimationType.SlideToLeft:
            //     seq.Append(canvasGroup.DOFade(0f, exitDuration).SetDelay(exitDelay));
            //     seq.Join(rectTransform.DOAnchorPos(originalPosition + Vector2.left * exitMoveDistance, exitDuration).SetEase(Ease.InCubic).SetDelay(exitDelay));
            //     break;
            // case ExitAnimationType.SlideToBottom:
            //     seq.Append(canvasGroup.DOFade(0f, exitDuration).SetDelay(exitDelay));
            //     seq.Join(rectTransform.DOAnchorPos(originalPosition - Vector2.up * exitMoveDistance, exitDuration).SetEase(Ease.InCubic).SetDelay(exitDelay));
            //     break;

            case ExitAnimationType.None:
                onCompleteCallback?.Invoke();
                return;
        }

        seq.OnComplete(() => {
            onComplete?.Invoke();           // ✅ 항상 호출
            onCompleteCallback?.Invoke();
        });
    }
}
