using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class scr_infiniteModeLevelUp : MonoBehaviour
{
    public float moveDuration = 1.0f;   // 이동 시간
    public float stayDuration = 2.0f;   // 중앙에서 대기 시간
    public float fadeOutDuration = 1.0f; // 사라지는 시간

    private RectTransform rect;
    private CanvasGroup canvasGroup;
    private Vector2 startPos;
    private Vector2 centerPos;
    private Vector2 endPos;
    private float screenWidth;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        screenWidth = Screen.width;

        // 시작 위치 (화면 우측 밖)
        startPos = new Vector2(screenWidth + rect.rect.width, Screen.height / 2);
        // 중앙 위치
        centerPos = new Vector2(screenWidth / 2, Screen.height / 2);
        // 끝 위치 (좌측 밖)
        endPos = new Vector2(-rect.rect.width, Screen.height / 2);

        // 초기 설정
        rect.position = startPos;
        canvasGroup.alpha = 0;

        // 연출 시작
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        // 1. 우측 → 중앙으로 이동 & 페이드 인
        yield return StartCoroutine(MoveAndFade(startPos, centerPos, 0, 1, moveDuration));

        // 2. 중앙에서 대기
        yield return new WaitForSeconds(stayDuration);

        // 3. 중앙 → 좌측 이동 & 페이드 아웃
        yield return StartCoroutine(MoveAndFade(centerPos, endPos, 1, 0, fadeOutDuration));

        // 4. 투명도 0 이하일 경우 제거
        Destroy(gameObject);
    }

    IEnumerator MoveAndFade(Vector2 from, Vector2 to, float fromAlpha, float toAlpha, float duration)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / duration);

            rect.position = Vector2.Lerp(from, to, lerp);
            canvasGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, lerp);

            yield return null;
        }
    }
}