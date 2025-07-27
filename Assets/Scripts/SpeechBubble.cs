using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeechBubble : MonoBehaviour
{
    public TextMeshProUGUI textUI;
    public Text textLegacy;

    [TextArea]
    public string fullText = "안녕하세요! 이것은 타이핑 효과입니다.";
    public float duration = 2f;

    public static void TMPDOText(TextMeshProUGUI text, float duration)
    {
        Debug.Log("잘 나와용", text);
        text.maxVisibleCharacters = 0;
        DOTween.To(x => text.maxVisibleCharacters = (int)x, 0f, text.text.Length, duration);
    }

    public void TypingEffect()
    {
        if (textUI != null)
        {
            StageCheck();
            textUI.text = fullText;
            TMPDOText(textUI, duration);
        }
    }
    void StageCheck()
        {
            if (global.stage == 1)
            {
                fullText = "으으,, 첫근무,,,\n 위가 쓰려온다..";
            }
            else if (global.stage == 2)
            {
                fullText = "오늘은 단속 근무날이네,,,\n 정신 똑바로 차리자";
            }
            else if (global.stage == 3)
            {
                fullText = "보트 도난 신고가 들어왔다는데?\n 그걸 어떻게 훔친 거야…";
            }
            else if (global.stage == 4)
            {
                fullText = "집중 단속기간이라,\n 별 일 없겠지…";
            }
            else if (global.stage == 5)
            {
                fullText = "드디어\n금요일이다.";
            }
        }
    }

