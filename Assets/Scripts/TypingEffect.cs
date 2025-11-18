using TMPro;
using UnityEngine.UI;   
using DG.Tweening;
using UnityEngine;

public class TypingEffect : MonoBehaviour
{
    public TextMeshProUGUI textUI;
    public Text textLegacy;
    [TextArea]
    public string fullText = "안녕하세요! 이것은 타이핑 효과입니다.";
    public float duration = 2f;

    public static void TMPDOText(TextMeshProUGUI text, float duration)
    {
//        Debug.Log("잘 나와용", text);
        text.maxVisibleCharacters = 0;
        DOTween.To(x=> text.maxVisibleCharacters = (int)x, 0f, text.text.Length, duration);
    }

    void Start()
    {
        if (textUI != null)
        {
            // 타이핑 효과
            TMPDOText(textUI, duration);
        }

        if (textLegacy != null)
        {
            textLegacy.DOText(fullText, duration);
        }
    }
}
