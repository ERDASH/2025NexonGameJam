using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 

[RequireComponent(typeof(Button))]
public class PaperUIElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("재생할 효과음 이름 (SoundManager의 리스트 기준)")]
    public string sfxName = "PaperSelect";

    [Header("말풍선 나오게 하기")]
    public SpeechBubble speechBubble;

    int stageNum = 0;

    void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(PlaySound);
        }
        stageNum = outputNumber();
    }

    void PlaySound()
    {
        if (!string.IsNullOrEmpty(sfxName) && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(sfxName);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (global.stage != stageNum)
        {
            return;
        }

        if (speechBubble != null)
        {
            speechBubble.gameObject.SetActive(true);
            speechBubble.TypingEffect();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (speechBubble != null)
        {
            speechBubble.gameObject.SetActive(false);
        }
    }

    int outputNumber()
    {
        // 1. 부모 이름 가져오기
        string parentName = transform.name; // 예: "BtnStageSelect01"

        // 2. 숫자만 추출 (뒤에서 숫자만 남기기)
        string numberStr = System.Text.RegularExpressions.Regex.Match(parentName, @"\d+$").Value;

        // 3. 정수형으로 변환
        if (int.TryParse(numberStr, out int stageNumber))
        {
            Debug.Log("스테이지 번호: " + stageNumber);

            return stageNumber;
        }
        else
        {
            Debug.LogWarning("숫자 추출 실패: " + parentName);
            return 0;
        }
    }
}
