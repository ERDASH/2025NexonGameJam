using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class scr_debugMesssage : MonoBehaviour
{
    // 🔹 인스펙터에 드래그앤드롭할 TMP 텍스트 두 개
    public TMP_Text text1;
    public TMP_Text text2;
    StageController stageController;
    string Mode = "일반";
    int Stage = 1;
    int Point = 0;
    string[] ModeOnOff = new string[] { "꺼짐", "꺼짐", "꺼짐", "꺼짐" };

    void Update()
    {
        stageController = FindObjectOfType<StageController>();

        if (stageController != null)
        {
            if (stageController.cloudMode == true) { ModeOnOff[0] = "켜짐"; } else { ModeOnOff[0] = "꺼짐"; }
            if (stageController.trafficLightMode == true) { ModeOnOff[1] = "켜짐"; } else { ModeOnOff[1] = "꺼짐"; }
            if (stageController.breakerMode == true) { ModeOnOff[2] = "켜짐"; } else { ModeOnOff[2] = "꺼짐"; }
            if (stageController.rushHourMode == true) { ModeOnOff[3] = "켜짐"; } else { ModeOnOff[3] = "꺼짐"; }

            if (stageController.infiniteMode == true)
            {
                Point = stageController.infiniteModeTargetScore;
                Mode = "무한";
                Stage = stageController.infiniteModeStage;
            }
            else
            {
                Point = GameManager.Instance.targetScore;
                Stage = global.stageNow;
            }
            if (text1 != null)
                text1.text = "모드 : "+Mode+"\n스테이지 : "+ Stage+"\n현재 차량 속도 : "+global.carSpeed+"\n속도 감소(O), 증가(P)"+"\n리젠 속도 : "+global.carSpawnSpeed+"\n목표점수 : "+Point+"\n현재체력 : "+GameManager.Instance.GetLife();   // 줄바꿈 포함

            if (text2 != null)
                text2.text = "구름(C) : "+ModeOnOff[0]+ "\n색상차량(B) : "+ModeOnOff[1]+"\n청소차(V) : " + ModeOnOff[2]+"\n러쉬아워(N) : " + ModeOnOff[3]+ "\n체력감소(S)\n점수추가(D)";
        }
    }
}