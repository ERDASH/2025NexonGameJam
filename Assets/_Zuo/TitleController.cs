using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleController : MonoBehaviour
{

    public GameObject transitionPrefab; // FoldTransition이 붙어 있는 프리팹
    private GameObject canvasHow;
    private GameObject canvasHowBtn;
    private GameObject canvasTitleBtn;
    public UnityEngine.UI.Image sprTitleLogo;


    private SpriteRenderer sprHow;
    private int imageIndex = 0;

    private float fadeDuration = 0.3f;
    private float dropDistance = 100f;

    void Start()
    {
        SoundManager.Instance.PlayBGM("MainMenu");
        GameObject root = GameObject.Find("Obj_TitleController");
        if (root == null)
        {
//            Debug.LogError("Obj_TitleController를 찾을 수 없습니다.");
            return;
        }

        Transform rootTransform = root.transform;

        canvasHow = rootTransform.Find("CanvasHow")?.gameObject;
        canvasHowBtn = rootTransform.Find("CanvasHowBtn")?.gameObject;
        canvasTitleBtn = rootTransform.Find("CanvasTitleBtn")?.gameObject;

        Transform sprHowTransform = rootTransform.Find("CanvasHow/SprHow");
        if (sprHowTransform != null)
        {
            sprHow = sprHowTransform.GetComponent<SpriteRenderer>();
        }
        /*
        if (canvasHow == null) Debug.LogError("CanvasHow 오브젝트를 찾을 수 없습니다.");
        if (canvasHowBtn == null) Debug.LogError("CanvasHowBtn 오브젝트를 찾을 수 없습니다.");
        if (canvasTitleBtn == null) Debug.LogError("CanvasTitleBtn 오브젝트를 찾을 수 없습니다.");
        */
        if (canvasHow != null) canvasHow.SetActive(false);
        if (canvasHowBtn != null) canvasHowBtn.SetActive(false);

        if (sprTitleLogo != null)
        {
            sprTitleLogo.transform.localScale = Vector3.zero;
            StartCoroutine(PopInLogo());
        }

        UpdateImage();
    }

    public void ButtonHowClick()
    {
        if (canvasHow != null/* && canvasHowBtn != null && canvasTitleBtn != null*/)
        {
            canvasHow.SetActive(true);
            canvasHowBtn.SetActive(true);
            canvasTitleBtn.SetActive(false);

            StartCoroutine(FadeInAndDropUI(canvasHow));
            //            StartCoroutine(FadeInAndDropUI(canvasHowBtn));
        }
        else
        {
         //   Debug.LogError("How 버튼 클릭 시 참조가 null입니다.");
        }
    }

    public void ButtonStart_Click()
    {
        global.mapChange = "Scene_StageSelect";


        if (!string.IsNullOrEmpty(global.mapChange))
        {
            Instantiate(transitionPrefab);
        }
        else
        {
            //Debug.LogWarning("global.mapChange 가 비어있습니다.");
        }

        //        SceneManager.LoadScene("Scene_StageSelect");



        void Update()
        {
            /*
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (!string.IsNullOrEmpty(global.mapChange))
            {
                Instantiate(transitionPrefab);
            }
            else
            {
                Debug.LogWarning("global.mapChange 가 비어있습니다.");
            }
        }
        */
        }
    }

    public void OnClickNext()
    {
        if (imageIndex < 1)
        {
            imageIndex++;
            UpdateImage();
        }
    }

    public void OnClickPrev()
    {
        if (imageIndex > 0)
        {
            imageIndex--;
            UpdateImage();
        }
    }

    public void OnClickQuit()
    {
        imageIndex = 0;
        canvasHow.SetActive(false);
        canvasHowBtn.SetActive(false);
        canvasTitleBtn.SetActive(true);
    }



    private void UpdateImage()
    {
        if (sprHow == null) return;

        string path = $"_Res_Zuo/Res_Title/res_title_how_1{imageIndex + 1}";
        Sprite newSprite = Resources.Load<Sprite>(path);

        if (newSprite != null)
        {
            sprHow.sprite = newSprite;
        }
        else
        {
      //      Debug.LogWarning($"해당 경로에 스프라이트 없음: {path}");
        }
    }

    IEnumerator FadeInAndDropUI(GameObject target)
    {
        // CanvasGroup이 없으면 추가
        CanvasGroup group = target.GetComponent<CanvasGroup>();
        if (group == null) group = target.AddComponent<CanvasGroup>();

        // ✨ 애니메이션 시작 전 — 클릭 막기
        group.interactable = false;
        group.blocksRaycasts = false;

        RectTransform rt = target.GetComponent<RectTransform>();
        if (rt == null)
        {
       //     Debug.LogError("UI 오브젝트에 RectTransform 없음");
            yield break;
        }

        Vector2 endPos = rt.anchoredPosition;
        Vector2 startPos = endPos + new Vector2(0, dropDistance);

        float t = 0f;
        group.alpha = 0f;
        rt.anchoredPosition = startPos;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float norm = Mathf.Clamp01(t / fadeDuration);
            group.alpha = norm;
            rt.anchoredPosition = Vector2.Lerp(startPos, endPos, norm);
            yield return null;
        }

        group.alpha = 1f;
        rt.anchoredPosition = endPos;

        // ✨ 등장 애니메이션 끝난 후 — 클릭 가능하게
        group.interactable = true;
        group.blocksRaycasts = true;
    }



    IEnumerator PopInLogo()
    {
        yield return new WaitForSeconds(0.3f);
        Transform logo = sprTitleLogo.transform;

        Vector3 baseScale = Vector3.one; // 실제 크기
        logo.localScale = Vector3.zero;

        float duration = 0.6f;
        float time = 0f;

        // 뽈록 키프레임
        Vector3[] keyframes = new Vector3[]
        {
        Vector3.zero,             // 0%
        baseScale * 1.2f,         // 튕김
        baseScale                 // 원본 크기
        };

        float[] keyTimes = new float[] { 0f, 0.3f, 1f };
        int currentFrame = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            while (currentFrame < keyTimes.Length - 2 && t > keyTimes[currentFrame + 1])
                currentFrame++;

            float segmentT = Mathf.InverseLerp(keyTimes[currentFrame], keyTimes[currentFrame + 1], t);
            logo.localScale = Vector3.Lerp(keyframes[currentFrame], keyframes[currentFrame + 1], segmentT);

            yield return null;
        }

        logo.localScale = baseScale;
    }
}