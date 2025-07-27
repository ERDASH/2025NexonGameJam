using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PanelBlurController : MonoBehaviour
{
    public GameObject panelContainer;         // 패널 전체 (내용 + 블러 배경)
    public Image blurImage;                   // blur Image (Image 컴포넌트)
    public float blurInTime = 0.3f;
    public float blurOutTime = 0.3f;
    public float targetBlurStrength = 2f;

    private Material blurMat;

    void Awake()
    {
        blurMat = blurImage.material;
        blurMat.SetFloat("_Size", 0f);        // 초기값
        panelContainer.SetActive(false);      // 시작 시 꺼두기
    }

    public void ShowPanel()
    {
        panelContainer.SetActive(true);
        blurMat.SetFloat("_Size", 0f);
        DOTween.To(() => blurMat.GetFloat("_Size"), x => blurMat.SetFloat("_Size", x), targetBlurStrength, blurInTime);
    }

    public void HidePanel()
    {
        DOTween.To(() => blurMat.GetFloat("_Size"), x => blurMat.SetFloat("_Size", x), 0f, blurOutTime)
               .OnComplete(() => panelContainer.SetActive(false));
    }
}
