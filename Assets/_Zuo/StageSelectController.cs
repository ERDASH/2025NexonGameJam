using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSelectController : MonoBehaviour
{
    public Button Button01;
    public Button Button02;
    public Button Button03;
    public Button Button04;
    public Button Button05;

    void Start()
    {
        SoundManager.Instance.PlayBGM("StageSelect");
        SetStageButtons(global.stage);
    }

    void SetStageButtons(int stage)
    {
        // 기본적으로 모두 비활성화
        Button01.interactable = false;
        Button02.interactable = false;
        Button03.interactable = false;
        Button04.interactable = false;
        Button05.interactable = false;

        // stage 값에 따라 버튼 순차적으로 활성화
        if (stage >= 1) Button01.interactable = true;
        if (stage >= 2) Button02.interactable = true;
        if (stage >= 3) Button03.interactable = true;
        if (stage >= 4) Button04.interactable = true;
        if (stage >= 5) Button05.interactable = true;
    }

    public void ButtonBackClick()
    {
        SceneManager.LoadScene("Scene_Title");
    }
}