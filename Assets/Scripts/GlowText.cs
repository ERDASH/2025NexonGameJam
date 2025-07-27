using TMPro;
using DG.Tweening;
using UnityEngine;

public class GlowText : MonoBehaviour
{
    public TextMeshProUGUI tmpText;
    public float minAlpha = 0.3f;
    public float maxAlpha = 1f;
    public float duration = 0.8f;

    void Start()
    {
        Color c = tmpText.color;
        c.a = minAlpha;
        tmpText.color = c;

        tmpText.DOFade(maxAlpha, duration)
               .SetLoops(-1, LoopType.Yoyo)
               .SetEase(Ease.InOutSine);
    }
}
