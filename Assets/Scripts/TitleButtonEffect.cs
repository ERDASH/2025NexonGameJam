using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class TitleButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("변경할 스프라이트")]
    public Sprite defaultSprite;
    public Sprite hoverSprite;

    private Vector3 originalScale;
    private Image buttonImage;

    void Start()
    {
        originalScale = transform.localScale;
        buttonImage = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(originalScale * 1.1f, 0.2f).SetEase(Ease.OutBack);

        if (hoverSprite != null)
            buttonImage.sprite = hoverSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetButtonVisual();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ResetButtonVisual();
    }

    private void ResetButtonVisual()
    {
        transform.DOScale(originalScale, 0.2f).SetEase(Ease.OutBack);

        if (defaultSprite != null)
            buttonImage.sprite = defaultSprite;
    }
}
