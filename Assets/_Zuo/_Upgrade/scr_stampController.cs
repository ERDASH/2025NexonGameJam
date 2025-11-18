using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class scr_stampController : MonoBehaviour
{
    private Image img;
    private Vector3 originalScale;
    private Coroutine effectCoroutine;
    public GameObject transitionPrefab;
    void Awake()
    {
        img = GetComponent<Image>();
        if (img == null)
        {
  //          Debug.LogError("⚠️ Image 컴포넌트가 필요합니다!");
            return;
        }

        originalScale = transform.localScale;
    }

    void OnEnable()
    {
        StartStampEffect();
    }

    void StartStampEffect()
    {
        if (effectCoroutine != null)
            StopCoroutine(effectCoroutine);

        // 초기 상태: 투명 + 크게
        Color c = img.color;
        c.a = 0f;
        img.color = c;

        transform.localScale = originalScale * 5f;

        effectCoroutine = StartCoroutine(StampEffect());

        Invoke(nameof(StampFinish), 1f);
    }

    IEnumerator StampEffect()
    {
        float t = 0f;
        float duration = 0.28f; // 🔹 더 빠르게
        Vector3 overshoot = originalScale * 1.25f; // 🔹 더 강한 튀김감

        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / duration);

            // 빠르게 축소 → 쿵!
            float scaleLerp = Mathf.SmoothStep(5f, 1.0f, normalized);
            transform.localScale = originalScale * scaleLerp;

            // 알파 빠르게 증가
            Color c = img.color;
            c.a = Mathf.Lerp(0f, 1f, normalized * 1.2f);
            img.color = c;

            yield return null;
        }

        // 순간적으로 더 커졌다가 “쿵” 하며 돌아오기
        transform.localScale = overshoot;
        yield return new WaitForSeconds(0.04f);
        transform.localScale = originalScale;

        // 짧고 강한 흔들림
        yield return StartCoroutine(Shake(0.1f, 10f));
    }

    IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }

    public void StampFinish()
    {
        global.mapChange = "AnimTest";
        global.stageNow = 100;

        if (!string.IsNullOrEmpty(global.mapChange))
        {
            Instantiate(transitionPrefab);
        }
//        Debug.Log("✅ Stamp finished!");
        // 예: 다음 연출로 넘어가기
    }
}