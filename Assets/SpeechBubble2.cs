using System.Collections;
using TMPro;
using UnityEngine;

public class SpeechBubble2 : MonoBehaviour
{
    public TextMeshProUGUI textUI;

    [TextArea]
    public string fullText = "";

    public float delayBeforeStart = 2f; // 시작 전 대기 시간
    public float typeDelay = 0.05f;     // 한 글자씩 출력 간격

    void Start()
    {
        if (textUI != null)
        {
            StageCheck();                      // 먼저 텍스트 결정
            textUI.text = "";                  // 초기엔 비워두기
            StartCoroutine(TypeText());        // 코루틴 시작
        }
    }

    void StageCheck()
    {
        switch (global.stageNow)
        {
            case 1:
                fullText = "서장님이 (방향키)로 교통 정리하고, (스페이스 바)로 단속하라 했지…";
                break;
            case 2:
                fullText = "수첩에다가 단속 차량을 정리 해놨으니까, 잘 확인해야 겠다";
                break;
            case 3:
                fullText = "헤드라인 뉴스 “괘씸한 절도범, 절대 용서치 않을것이오.” 초밥집 사장 대격노라…";
                break;
            case 4:
                fullText = "금일 지침 사항 “불법 개조 차량 다수 출현, 민간 피해 최소화 하며 단속할 것” 이게 말이 쉽지,,,";
                break;
            case 5:
                fullText = "그래도 일해야하는 건 변하지 않아.";
                break;
            default:
                fullText = "그래도 일해야하는 건 변하지 않아.";
                break;
        }
    }

    IEnumerator TypeText()
    {
        yield return new WaitForSeconds(delayBeforeStart);

        textUI.ForceMeshUpdate(); // 안전하게 텍스트 갱신
        for (int i = 0; i <= fullText.Length; i++)
        {
            textUI.text = fullText.Substring(0, i);
            yield return new WaitForSeconds(typeDelay);
        }
    }
}