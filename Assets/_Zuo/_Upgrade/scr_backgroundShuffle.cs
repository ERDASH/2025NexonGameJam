using UnityEngine;
using UnityEngine.UI;

public class scr_backgroundShuffle : MonoBehaviour
{
    public Sprite[] backgroundSprites;  // 3개 넣을 배열
    private Image imageComponent;

    void Start()
    {
        imageComponent = GetComponent<Image>();
        UpdateBackground();
    }

    void Update()
    {
        // global.StageMap 값이 바뀔 수 있으니, 매 프레임 갱신
        UpdateBackground();
    }

    void UpdateBackground()
    {
        if (backgroundSprites == null || backgroundSprites.Length == 0)
            return;

        int index = Mathf.Clamp(global.StageMap, 0, backgroundSprites.Length - 1);
        if (imageComponent.sprite != backgroundSprites[index])
        {
            imageComponent.sprite = backgroundSprites[index];
        }
    }
}