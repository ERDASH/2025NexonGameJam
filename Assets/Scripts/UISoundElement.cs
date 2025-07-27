using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UISoundElement : MonoBehaviour
{
    [Header("재생할 효과음 이름 (SoundManager의 리스트 기준)")]
    public string sfxName = "Button";

    void Start()
    {
        // 버튼 컴포넌트 가져오기
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(PlaySound);
        }
    }

    void PlaySound()
    {
        if (!string.IsNullOrEmpty(sfxName) && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(sfxName);
        }
    }
}
