using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Police : MonoBehaviour
{
    public GameObject speechBubble;
    public Sprite policeDefault;
    public Sprite policeWorkOut;

    public GameObject speechPoliceBubble;

    private Image image;
    private float speechTimer = 0f;
    private bool bubbleTriggered = false;

    void Start()
    {
        image = GetComponent<Image>();
        if (image == null)
        {
//            Debug.LogError("Image 컴포넌트가 필요합니다.");
            return;
        }

        StartCoroutine(AnimatePoliceSprite());
    }

    void Update()
    {
        if (speechBubble.activeSelf)
        {
            speechTimer += Time.deltaTime;

            if (!bubbleTriggered && speechTimer >= 5f)
            {
                speechPoliceBubble.SetActive(true);
                bubbleTriggered = true;
            }
        }
        else
        {
            // 말풍선 꺼졌을 때 상태 초기화
            speechTimer = 0f;
            bubbleTriggered = false;
            speechPoliceBubble.SetActive(false);
        }
    }

    IEnumerator AnimatePoliceSprite()
    {
        while (true)
        {
            image.sprite = policeDefault;
            yield return new WaitForSeconds(5f);

            image.sprite = policeWorkOut;
            yield return new WaitForSeconds(1f);
        }
    }
}
