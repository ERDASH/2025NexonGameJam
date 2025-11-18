using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class TutorialManager : MonoBehaviour
{
    public Image policeTalk;
    public GameObject police;
    public TextMeshProUGUI text;
    public float duration = 2f;

    public Image[] images;
    public int idx = 0;

    // Start is called before the first frame update
    void Start()
    {
        TypingEffect();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (idx == 0)
            {
                policeTalk.gameObject.SetActive(false);
                police.SetActive(false);
            }
            if (idx == images.Length-1)
            {
                SceneManager.LoadScene("AnimTest");
            }
            else
            {
                images[idx].gameObject.SetActive(true);
                idx++;
            }
        }
    }

    public static void TMPDOText(TextMeshProUGUI text, float duration)
    {
//        Debug.Log("잘 나와용", text);
        text.maxVisibleCharacters = 0;
        DOTween.To(x => text.maxVisibleCharacters = (int)x, 0f, text.text.Length, duration);
    }

    public void TypingEffect()
    {
        if (text != null)
        {
            text.text = "어이. 신입\n오늘 첫 출근이니깐\n잘 보고 숙지하라고!!";
            TMPDOText(text, duration);
        }
    }
}
