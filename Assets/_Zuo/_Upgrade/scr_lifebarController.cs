using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_lifebarController : MonoBehaviour
{
    [Header("이미지 배열 (0: false, 1: true)")]
    public Sprite[] sprites; // 드래그해서 두 개 넣기

    [Header("A 스크립트 참조")]
    

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        GameStageBlockController controller = FindObjectOfType<GameStageBlockController>();
        if (controller == null || sprites.Length < 2) return;

        // A.cs의 bool 변수(as)에 따라 이미지 교체
        if (controller.infiniteModeDelay == true)
        {
            sr.sprite = sprites[1];
        }
        else
        {
            sr.sprite = sprites[0];
        }
    }
}
