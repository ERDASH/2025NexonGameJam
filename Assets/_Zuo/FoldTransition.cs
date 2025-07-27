using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FoldTransition : MonoBehaviour
{
    public RectTransform topRect;
    public RectTransform bottomRect;

    public float waitBeforeOpen = 0.2f;

    private Vector2 topOrigin;
    private Vector2 bottomOrigin;
    private float screenHalfHeight;

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

        // �ָ� ������ �ְ� ���� (���� ����)
        topRect.anchoredPosition = topOrigin + new Vector2(0, screenHalfHeight);
        bottomRect.anchoredPosition = bottomOrigin - new Vector2(0, screenHalfHeight);
    }

    void Update()
    {
        if (state == State.Entering)
        {
            // ������ �ִϸ��̼�
            topRect.anchoredPosition += (topOrigin - topRect.anchoredPosition) / 10f;
            bottomRect.anchoredPosition += (bottomOrigin - bottomRect.anchoredPosition) / 10f;

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
            // �������� �ִϸ��̼� (�ٽ� ���� ����)
            Vector2 topTarget = topOrigin + new Vector2(0, screenHalfHeight);
            Vector2 bottomTarget = bottomOrigin - new Vector2(0, screenHalfHeight);

            topRect.anchoredPosition += (topTarget - topRect.anchoredPosition) / 10f;
            bottomRect.anchoredPosition += (bottomTarget - bottomRect.anchoredPosition) / 10f;

            if (Vector2.Distance(topRect.anchoredPosition, topTarget) < 0.5f)
            {
                Destroy(gameObject);
            }
        }
    }

    IEnumerator DoSceneChange()
    {
        // �� �ε�
        if (!string.IsNullOrEmpty(global.mapChange))
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(global.mapChange);
            yield return op; // �ε� �Ϸ���� ���
        }

        yield return new WaitForSeconds(waitBeforeOpen);
        state = State.Exiting;
    }
}