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
            //1주차
            case 1:
                fullText = "서장님이 (방향키)로 교통 정리하고, (스페이스 바)로 단속하라 했지…";
                break;
            case 2:
                fullText = "단속 차량 메모 완료.";
                break;
            case 3:
                fullText = "금일 특이 사항 없음...!";
                break;
            case 4:
                fullText = "집중 단속 기간,,, 유심히 살펴보자.";
                break;
            case 5:
                fullText = "오늘은 안개가 많네... 조심,조심.";
                break;

            //2주차
            case 6:
                fullText = "주말은 항상 빨리 지나가...";
                break;
            case 7:
                fullText = "이상한 차량들이 늘어난 기분이야.";
                break;
            case 8:
                fullText = "서장님이 화내는 건지 응원해 주는 건지 헷갈려…";
                break;
            case 9:
                fullText = "좋았어, 오늘도 힘내보자!!!";
                break;
            case 10:
                fullText = "금일 특이사항... 요일제 차량 시범 운영... 도로색과 차량색을 유의할 것.";
                break;

            //3주차
            case 11:
                fullText = "모자 확인, 완장 확인, 근무 준비 완료!";
                break;
            case 12:
                fullText = "단속을 해도 해도 줄어들지 않는 거 같지?";
                break;
            case 13:
                fullText = "그래도 일해야 하는건 변하지 않아.";
                break;
            case 14:
                fullText = "드디어 내일 첫 월급이야!";
                break;
            case 15:
                fullText = "금일 특이사항... 청소차 운영 중, 빠른 교통정리 희망? 왜?";
                break;

            //4주차
            case 16:
                fullText = "오전은 비교적 한가한 편이야.";
                break;
            case 17:
                fullText = "폭주시티에는 슬픈 전설이 있어...";
                break;
            case 18:
                fullText = "최근에 서장님이 너무 조용하신거 같아...";
                break;
            case 19:
                fullText = "오늘따라 너무 평화로운데...어랏?";
                break;
            case 20:
                fullText = "퇴근 시간대라 도로가 너무 혼잡해, 정신 똑바로 차려야겠어.";
                break;

            //5주차
            case 21:
                fullText = "안개도 많이 꼈고, 도로 청소까지 있네...";
                break;
            case 22:
                fullText = "요일제인데 퇴근 시간까지 겹쳤잖아?!";
                break;
            case 23:
                fullText = "퇴근 시간이랑 청소차가 동시에... 큰일이야...";
                break;

            //기본
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