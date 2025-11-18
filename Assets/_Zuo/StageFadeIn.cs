/*
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageFadeIn_UI : MonoBehaviour
{
    public GameObject transitionPrefab;
    public string sceneName;              // 버튼 클릭 시 이동할 씬 이름
    public int stageSelect = 0;           // 현재 선택된 스테이지


    public void ButtonStageClick()
    {

        global.mapChange = sceneName;
        global.stageNow = stageSelect;

        if (!string.IsNullOrEmpty(global.mapChange))
        {
            Instantiate(transitionPrefab);
        }


    }
}
*/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.EventSystems;

public class StageFadeIn_UI : MonoBehaviour , IPointerEnterHandler
{
    public GameObject transitionPrefab;
    public float startDelay = 0f;         // 시작 지연 시간
    public float duration = 0.3f;         // 애니메이션 지속 시간
    public float dropDistance = 80f;      // 위에서 얼마나 떨어질지
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
//            Debug.LogError("RectTransform 또는 CanvasGroup이 필요합니다!");
            return;
        }

        // 초기 세팅: 위로 이동 + 투명하게
        Vector2 endPos = rt.anchoredPosition;
        Vector2 startPos = endPos + new Vector2(0, dropDistance);
        rt.anchoredPosition = startPos;
        cg.alpha = 0f;

        StartCoroutine(FadeInAndDrop(startPos, endPos));
    }

    IEnumerator FadeInAndDrop(Vector2 start, Vector2 end)
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

        // 보정
        rt.anchoredPosition = end;
        cg.alpha = 1f;
    }

    // 버튼 클릭 시 호출
    public void ButtonStageClick()
    {

        global.mapChange = sceneName;
        global.stageNow = stageSelect;

        if (!string.IsNullOrEmpty(global.mapChange))
        {
            Instantiate(transitionPrefab);
        }



    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        global.stageNowTemp = stageSelect;
        // 디버그용 로그
//        Debug.Log($"[Hover] global.stageNowTemp = {global.stageNowTemp}");
    }
}
