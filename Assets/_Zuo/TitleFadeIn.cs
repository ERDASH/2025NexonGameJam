using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleFadeIn : MonoBehaviour
{
    public float startDelay = 0f;         // 시작 지연 시간
    public float duration = 0.3f;         // 애니메이션 지속 시간
    public float moveDistance = 80f;      // 이동 거리 (방향에 따라 해석)
    public enum Direction { TopDown, RightToLeft, LeftToRight, BottomUp }
    public Direction moveDirection = Direction.RightToLeft;

    public int stageSelect = 0;
    public string sceneName;              // 버튼 클릭 시 이동할 씬 이름

    private RectTransform rt;
    private CanvasGroup cg;

    void Start()
    {
        rt = GetComponent<RectTransform>();
        cg = GetComponent<CanvasGroup>();

        if (rt == null || cg == null)
        {
            //Debug.LogError("RectTransform 또는 CanvasGroup이 필요합니다!");
            return;
        }

        Vector2 endPos = rt.anchoredPosition;
        Vector2 offset = Vector2.zero;

        switch (moveDirection)
        {
            case Direction.TopDown:
                offset = new Vector2(0, moveDistance);
                break;
            case Direction.RightToLeft:
                offset = new Vector2(moveDistance, 0);
                break;
            case Direction.LeftToRight:
                offset = new Vector2(-moveDistance, 0);
                break;
            case Direction.BottomUp:
                offset = new Vector2(0, -moveDistance);
                break;
        }

        Vector2 startPos = endPos + offset;
        rt.anchoredPosition = startPos;
        cg.alpha = 0f;

        StartCoroutine(FadeInAndMove(startPos, endPos));
    }

    IEnumerator FadeInAndMove(Vector2 start, Vector2 end)
    {
        yield return new WaitForSeconds(startDelay);

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / duration);

            rt.anchoredPosition = Vector2.Lerp(start, end, normalized);
            cg.alpha = normalized;

            yield return null;
        }

        rt.anchoredPosition = end;
        cg.alpha = 1f;
    }

    /*
    public void ButtonStageClick()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            global.stageNow = stageSelect;
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("이동할 씬 이름이 설정되지 않았습니다.");
        }
    }
    */
}