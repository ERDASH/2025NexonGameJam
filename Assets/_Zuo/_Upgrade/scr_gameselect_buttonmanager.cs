using UnityEngine;
using System.Collections;

public class scr_gameselect_buttonmanager : MonoBehaviour
{
    private RectTransform rect;
    private Coroutine scaleRoutine;

    private readonly Vector3 scaleDefault = Vector3.one;
    private readonly Vector3 scaleHover = Vector3.one * 1.05f;
    private readonly Vector3 scaleClick = Vector3.one * 1.10f;
    private readonly Vector3 scalePop = Vector3.one * 1.12f;

    private bool isHover = false;
    private bool isDown = false;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        bool inside = RectTransformUtility.RectangleContainsScreenPoint(rect, mousePos);

        // 클릭 중
        if (inside && Input.GetMouseButtonDown(0))
        {
            isDown = true;
            ForceStartCoroutine(ClickEffect());
        }

        // 마우스 위에 올려져있을 때
        else if (inside && !Input.GetMouseButton(0))
        {
            if (!isHover)
            {
                isHover = true;
                isDown = false;
                ForceStartCoroutine(ScaleTo(scaleHover, 15f));
            }
        }
        // 마우스 밖
        else if (!inside)
        {
            isHover = false;
            isDown = false;
            ForceStartCoroutine(ScaleTo(scaleDefault, 15f));
        }

        // 전역 마우스 뗌
        if (Input.GetMouseButtonUp(0))
        {
            isDown = false;
            ForceStartCoroutine(ScaleTo(inside ? scaleHover : scaleDefault, 20f));
        }
    }

    // 👇 코루틴을 강제로 가장 우선시해서 실행 (이전 코루틴 싹 무시)
    private void ForceStartCoroutine(IEnumerator routine)
    {
        if (scaleRoutine != null)
            StopCoroutine(scaleRoutine);

        // 💥 현재 상태를 즉시 보정 (중간값 남지 않게)
        rect.localScale = rect.localScale;

        scaleRoutine = StartCoroutine(routine);
    }

    private IEnumerator ScaleTo(Vector3 target, float speed)
    {
        Vector3 start = rect.localScale;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * speed;
            rect.localScale = Vector3.Lerp(start, target, t);
            yield return null;
        }

        rect.localScale = target; // 💥 무조건 마지막 값 보정
    }

    private IEnumerator ClickEffect()
    {
        yield return ScaleTo(scalePop, 30f);
        yield return ScaleTo(scaleClick, 25f);
    }
}
