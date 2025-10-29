using UnityEngine;
using System.Collections;

public class scr_breakerController : MonoBehaviour
{
    [Header("브레이커 이미지 (SpriteRenderer)")]
    public SpriteRenderer breakerSprite;

    [Header("펑 시점에 생성할 CleanCar 오브젝트")]
    public GameObject cleanCarPrefab; // 드래그 앤 드롭!

    private bool IsFallingOn = false;
    [HideInInspector] public int targetLine = 1;

    private void Start()
    {
        if (breakerSprite == null)
            breakerSprite = GetComponent<SpriteRenderer>();

        StartCoroutine(BreakerRoutine());
    }

    IEnumerator BreakerRoutine()
    {
        Color baseColor = breakerSprite.color;

        // ⚪ 1~3번째 깜빡깜빡
        for (int i = 0; i < 3; i++)
        {
            // 0 → 0.3 (빠르게)
            yield return StartCoroutine(FadeAlpha(0f, 0.3f, 0.08f));
            yield return new WaitForSeconds(0.05f);

            // 0.3 → 0 (빠르게)
            yield return StartCoroutine(FadeAlpha(0.3f, 0f, 0.08f));
            yield return new WaitForSeconds(0.05f);
        }

        // 🔴 펑! — 빨강으로 변경 + 충돌 비활성화 + 즉시 처리
        breakerSprite.color = new Color(1f, 0f, 0f, 1f);

        

        IsFallingOn = true;

        yield return new WaitForFixedUpdate();
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        // cleanCarPrefab 생성 (자신의 X, Y+0.5f)
        if (cleanCarPrefab != null)
        {
            Vector3 spawnPos = transform.position + new Vector3(0f, 0.5f, 0f);
            Instantiate(cleanCarPrefab, spawnPos, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("⚠️ cleanCarPrefab이 지정되지 않았습니다!");
        }

        // 💥 살짝 "펑" 효과 (X축만 확대)
        Vector3 originalScale = transform.localScale;
        transform.localScale = new Vector3(originalScale.x * 1.2f, originalScale.y, originalScale.z);

        // 🔸 즉시 원복 후 바로 삭제 (스르륵 제거)
        transform.localScale = originalScale;
        Destroy(gameObject);
    }

    IEnumerator FadeAlpha(float from, float to, float duration)
    {
        float t = 0f;
        Color c = breakerSprite.color;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            c.a = Mathf.Lerp(from, to, t);
            breakerSprite.color = c;
            yield return null;
        }
    }


    void OnCollisionStay2D(Collision2D collision)
    {
        if (!IsFallingOn) return;

        if (collision.gameObject.CompareTag("Block"))
        {
            FallingBlock block = collision.gameObject.GetComponent<FallingBlock>();

            if (block != null && block.isPreviewBlock == false)
            {
                bool hasVisibleChild = false;
                SpriteRenderer[] childRenderers = block.GetComponentsInChildren<SpriteRenderer>(includeInactive: false);

                foreach (SpriteRenderer sr in childRenderers)
                {
                    // 부모 자신은 건너뛰기
                    if (sr.gameObject == block.gameObject) continue;

                    if (sr.gameObject.activeSelf)
                    {
                        Debug.Log($"검사중: {sr.name}, 알파={sr.color.a}");

                        if (sr.color.a > 0.9f)
                        {
                            hasVisibleChild = true;
                            break;
                        }
                    }
                }

                if (hasVisibleChild)
                {
                    block.FallGone();
                    Debug.Log($"{block.name} : FallingOn 실행됨 (투명도 > 0.9 자식 감지)");
                }
            }
        }
    }

    /*
    void OnCollisionStay2D(Collision2D collision)
    {
        if (!IsFallingOn) return;

        if (collision.gameObject.CompareTag("Block"))
        {
            FallingBlock block = collision.gameObject.GetComponent<FallingBlock>();

            if (block != null && block.isPreviewBlock == false)
            {
                bool hasVisibleChild = false;

                // 🔎 자식들 검사
                SpriteRenderer[] childRenderers = block.GetComponentsInChildren<SpriteRenderer>(includeInactive: false);
                foreach (SpriteRenderer sr in childRenderers)
                {
                    if (sr.gameObject.activeSelf && sr.color.a > 0.9f)
                    {
                        hasVisibleChild = true;
                        break;
                    }
                }

                // 💥 조건 모두 만족 시에만 실행
                if (hasVisibleChild)
                {
                    block.FallGone();
                    Debug.Log($"{block.name} : FallingOn 실행됨 (투명도 > 0.9 자식 감지)");
                }
            }
        }
    }*/


    /*
    void OnCollisionStay2D(Collision2D collision)
    {
        if (IsFallingOn)
        {
            if (collision.gameObject.CompareTag("Block"))
            {
                FallingBlock block = collision.gameObject.GetComponent<FallingBlock>();

                if (block != null && block.isPreviewBlock == false)
                {
                    block.FallGone();
                    Debug.Log($"{block.name} : FallingOn 실행됨");
                }
            }
        }
    }*/
}
