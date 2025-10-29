using UnityEngine;
using System.Collections;

public class scr_trafficLightLineController : MonoBehaviour
{
    public StageController stageController;
    public GameObject targetObject;   // SpriteRenderer들이 들어있는 오브젝트
    public float fadeSpeed = 2f;

    SpriteRenderer[] sprites;
    bool currentMode=false;

    void Start()
    {
        // 자식 포함 모든 SpriteRenderer 가져오기
        sprites = targetObject.GetComponentsInChildren<SpriteRenderer>(true);
        SetAlpha(0f); // 처음은 완전 투명
    }

    void Update()
    {
        if (!stageController) return;

        if (stageController.trafficLightMode != currentMode)
        {
            currentMode = stageController.trafficLightMode;
            StopAllCoroutines();
            StartCoroutine(FadeSprites(currentMode ? 1f : 0f));
        }
    }

    IEnumerator FadeSprites(float target)
    {
        float start = sprites.Length > 0 ? sprites[0].color.a : 0f;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;
            float a = Mathf.Lerp(start, target, t);
            SetAlpha(a);
            yield return null;
        }
    }

    void SetAlpha(float a)
    {
        foreach (var s in sprites)
        {
            if (!s) continue;
            Color c = s.color;
            c.a = a;
            s.color = c;
        }
    }
}
