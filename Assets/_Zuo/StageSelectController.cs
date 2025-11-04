using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static scr_gameselect_buttonmanager;

public class StageSelectController : MonoBehaviour
{

    public bool WorkSheetToggle = false;
   
    public GameObject WorkSheetObject;
    public GameObject WorkSheelAlpha;

    [Header("스테이지 버튼 (1~20)")]
    public Button[] StageButtons = new Button[20];

    // 드래그앤드롭한 스프라이트들
    public Sprite SpriteStageNow;  // 현재 진행 중인 스테이지 스프라이트
    public Sprite SpriteStageCleared;  // 클리어된 스테이지 스프라이트
    public Sprite SpriteStageLocked;  // 잠금된 스테이지 스프라이트 (추가적으로 필요할 경우)

    public bool InfiniteSheetToggle = false;
    public GameObject InfiniteSheetObject;
    public GameObject InfiniteSheetAlpha;

    [Header("스테이지 미리보기 이미지 관련")]
    public Image PreviewImage;                  // 표시할 이미지 오브젝트
    public Sprite[] StagePreviewSprites = new Sprite[10]; // 드래그 앤 드롭할 미리보기 이미지 배열

    void Start()
    {
        global.stageNowTemp = 0;
        SoundManager.Instance.PlayBGM("StageSelect");
        SetStageButtons(global.stage-1);  // global.stage에 맞춰 버튼을 활성화
    }

    void Update()
    {

        //------------------------------------------
        // DEBUG CODE
        //------------------------------------------
        if (Input.GetKeyDown(KeyCode.D))
        {
            global.stage += 1;
            SetStageButtons(global.stage - 1);
        }
        //------------------------------------------

        // === WorkSheet 애니메이션 ===
        Vector3 pos = WorkSheetObject.transform.position;
        Image img = WorkSheelAlpha.GetComponent<Image>();
        Color c = img.color;

        if (WorkSheetToggle)
        {
            pos.y += (Screen.height/2 - pos.y) / 5f;
            c.a += (0.6f - c.a) / 5f;
        }
        else
        {
            pos.y += ((Screen.height*3)/2 - pos.y) / 5f;
            c.a += (0f - c.a) / 5f;
        }
        img.color = c;
        WorkSheetObject.transform.position = pos;


        // === InfiniteSheet 애니메이션 ===
        Vector3 pos2 = InfiniteSheetObject.transform.position;
        Image img2 = InfiniteSheetAlpha.GetComponent<Image>();
        Color c2 = img2.color;

        if (InfiniteSheetToggle)
        {
            pos2.y += (Screen.height / 2 - pos2.y) / 5f;
            c2.a += (0.6f - c2.a) / 5f;
        }
        else
        {
            pos2.y += ((Screen.height * 3) / 2 - pos2.y) / 5f;
            c2.a += (0f - c2.a) / 5f;
        }
        img2.color = c2;
        InfiniteSheetObject.transform.position = pos2;

        UpdatePreviewImage();
    }


    void UpdatePreviewImage()
    {
     //   if (PreviewImage == null || StagePreviewSprites.Length == 0)
       //     return;

        Sprite targetSprite = StagePreviewSprites[0]; // 기본 이미지

        if (global.stageNowTemp == 5)
        {
            if (StagePreviewSprites.Length > 1)
                targetSprite = StagePreviewSprites[1];
        }
        else if (global.stageNowTemp == 10)
        {
            if (StagePreviewSprites.Length > 2)
                targetSprite = StagePreviewSprites[2];
        }
        else if (global.stageNowTemp == 15)
        {
            if (StagePreviewSprites.Length > 3)
                targetSprite = StagePreviewSprites[3];
        }
        else if (global.stageNowTemp == 20)
        {
            if (StagePreviewSprites.Length > 4)
                targetSprite = StagePreviewSprites[4];
        }
        else if (global.stageNowTemp == 21)
        {
            if (StagePreviewSprites.Length > 4)
                targetSprite = StagePreviewSprites[4];
        }

        else if (global.stageNowTemp == 22)
        {
            if (StagePreviewSprites.Length > 4)
                targetSprite = StagePreviewSprites[4];
        }
        else if (global.stageNowTemp == 23)
        {
            if (StagePreviewSprites.Length > 4)
                targetSprite = StagePreviewSprites[4];
        }


        PreviewImage.sprite = targetSprite;
    }



    void SetStageButtons(int stage)
    {
        // 기본적으로 모두 비활성화
        foreach (Button button in StageButtons)
        {
            button.interactable = false;
            button.GetComponent<Image>().sprite = SpriteStageLocked;  // 잠금 상태로 이미지 설정
            SetButtonTransparency(button, 0f);  // 비활성화된 버튼의 투명도를 50%로 설정
        }

        // stage 값에 따라 버튼 순차적으로 활성화
        for (int i = 0; i < StageButtons.Length; i++)
        {
            if (i < stage)  // stage 값보다 작은 인덱스는 이미 클리어된 스테이지
            {
                StageButtons[i].interactable = true;
                StageButtons[i].GetComponent<Image>().sprite = SpriteStageCleared;  // 클리어된 스테이지 이미지로 변경
                SetButtonTransparency(StageButtons[i], 1f);  // 클리어된 버튼은 불투명(투명도 255)
            }
            else if (i == stage)  // 현재 진행 중인 스테이지
            {
                StageButtons[i].interactable = true;
                StageButtons[i].GetComponent<Image>().sprite = SpriteStageNow;  // 현재 진행 중인 스테이지 이미지로 변경
                SetButtonTransparency(StageButtons[i], 1f);  // 현재 진행 중인 버튼은 불투명(투명도 255)
            }
            // else는 기본 상태로 이미지가 이미 설정되어 있으므로 따로 변경하지 않음
        }
    }

    // 버튼의 투명도를 설정하는 함수
    void SetButtonTransparency(Button button, float transparency)
    {
        Color color = button.GetComponent<Image>().color;
        color.a = transparency;  // 투명도 설정
        button.GetComponent<Image>().color = color;  // 버튼의 이미지에 반영
    }




    public void ButtonBackClick()
    {
        SceneManager.LoadScene("Scene_Title");
    }

    public void ButtonToggleSheetON()
    {
        WorkSheetToggle = true;
        Debug.Log("TETETE");
    }

    public void ButtonToggleSheetOFF()
    {
        WorkSheetToggle = false;
    }


    public void ButtonToggleInfiniteSheetON()
    {
        InfiniteSheetToggle = true;
        Debug.Log("Infinite Sheet ON");
    }

    public void ButtonToggleInfiniteSheetOFF()
    {
        InfiniteSheetToggle = false;
        Debug.Log("Infinite Sheet OFF");
    }


}