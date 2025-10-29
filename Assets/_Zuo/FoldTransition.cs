using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FoldTransition : MonoBehaviour
{
    public RectTransform topRect;
    public RectTransform bottomRect;

    public float waitBeforeOpen = 0.2f;

    public bool isRealMove = true;

    private Vector2 topOrigin;
    private Vector2 bottomOrigin;
    private float screenHalfHeight;
    private float speed = 5f;

    private enum State { Entering, Waiting, SceneChange, Exiting }
    private State state = State.Entering;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        screenHalfHeight = Screen.height / 2f;

        topOrigin = topRect.anchoredPosition;
        bottomOrigin = bottomRect.anchoredPosition;

        // 멀리 떨어져 있게 시작 (닫힌 상태)
        topRect.anchoredPosition = topOrigin + new Vector2(0, screenHalfHeight);
        bottomRect.anchoredPosition = bottomOrigin - new Vector2(0, screenHalfHeight);
    }

    void Update()
    {
        if (state == State.Entering)
        {
            // 닫히는 애니메이션
            topRect.anchoredPosition += (topOrigin - topRect.anchoredPosition) / speed;
            bottomRect.anchoredPosition += (bottomOrigin - bottomRect.anchoredPosition) / speed;

            if (Vector2.Distance(topRect.anchoredPosition, topOrigin) < 0.5f)
            {
                topRect.anchoredPosition = topOrigin;
                bottomRect.anchoredPosition = bottomOrigin;

                state = State.Waiting;
                StartCoroutine(DoSceneChange());
            }
        }
        else if (state == State.Exiting)
        {
            // 펼쳐지는 애니메이션 (다시 위로 나감)
            Vector2 topTarget = topOrigin + new Vector2(0, screenHalfHeight);
            Vector2 bottomTarget = bottomOrigin - new Vector2(0, screenHalfHeight);

            topRect.anchoredPosition += (topTarget - topRect.anchoredPosition) / speed;
            bottomRect.anchoredPosition += (bottomTarget - bottomRect.anchoredPosition) / speed;

            if (Vector2.Distance(topRect.anchoredPosition, topTarget) < 0.5f)
            {
                Destroy(gameObject);
            }
        }
    }

    IEnumerator DoSceneChange()
    {
        // 씬 로딩
        if (!string.IsNullOrEmpty(global.mapChange) && isRealMove==true)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(global.mapChange);
            yield return op; // 로딩 완료까지 대기
        }

        yield return new WaitForSeconds(waitBeforeOpen);
        state = State.Exiting;
    }
}