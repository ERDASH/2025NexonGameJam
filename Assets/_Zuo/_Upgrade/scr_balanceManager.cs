using UnityEngine;
using TMPro;   // TMP 사용 시

public class scr_balanceManager : MonoBehaviour
{
    public GameObject balancePanel;           // 전체 패널 하나
    public TMP_InputField[] inputFields;      // 여러 개 드래그앤드롭 (최소 1개 이상)

    bool panelOpen = false;
    float prevTimeScale = 1f;

    void Update()
    {
        // F1 누르면 열기/닫기
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (!panelOpen) OpenPanel();
            else ClosePanel();
        }
    }

    void OpenPanel()
    {
        panelOpen = true;

        // ------------------------------------------------------
        // [0] 차량 속도
        if (inputFields != null && inputFields.Length > 0 && inputFields[0] != null)
            inputFields[0].text = global.carSpeed.ToString();
        // [1] 리젠 속도
        if (inputFields != null && inputFields.Length > 0 && inputFields[1] != null)
            inputFields[1].text = global.carSpawnSpeed.ToString();
        // [2] 특이차량 등장 확률
        if (inputFields != null && inputFields.Length > 0 && inputFields[2] != null)
            inputFields[2].text = global.carBadPer.ToString();

        // [3] 초당 라이프 감소    
        if (inputFields != null && inputFields.Length > 0 && inputFields[3] != null)
            inputFields[3].text = global.lifeSubTick.ToString();
        // [4] 단속 실수/실패 라이프 감소
        if (inputFields != null && inputFields.Length > 0 && inputFields[4] != null)
            inputFields[4].text = global.lifeSubBadFail.ToString();
        // [5] 블록 놓침 라이프 감소  
        if (inputFields != null && inputFields.Length > 0 && inputFields[5] != null)
            inputFields[5].text = global.lifeSubMiss.ToString();
        // [6] 라인 파괴됨 라이프 감소
        if (inputFields != null && inputFields.Length > 0 && inputFields[6] != null)
            inputFields[6].text = global.lifeSubLineBreak.ToString();

        // [7] 단속 성공 라이프 회복    
        if (inputFields != null && inputFields.Length > 0 && inputFields[7] != null)
            inputFields[7].text = global.lifeAddBadSuccess.ToString();
        // [8] 1줄 완성 라이프 회복
        if (inputFields != null && inputFields.Length > 0 && inputFields[8] != null)
            inputFields[8].text = global.lifeAddLine1.ToString();
        // [9] 2줄 완성 라이프 회복
        if (inputFields != null && inputFields.Length > 0 && inputFields[9] != null)
            inputFields[9].text = global.lifeAddLine2.ToString();
        // [10] 3줄 완성 라이프 회복
        if (inputFields != null && inputFields.Length > 0 && inputFields[10] != null)
            inputFields[10].text = global.lifeAddLine3.ToString();
        // [11] 4줄 완성 라이프 회복
        if (inputFields != null && inputFields.Length > 0 && inputFields[11] != null)
            inputFields[11].text = global.lifeAddLine4.ToString();
        // ------------------------------------------------------

        // 게임 정지
        prevTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        balancePanel.SetActive(true);
    }

    // 확인 버튼 OnClick 에 연결
    public void OnClickConfirm()
    {
        // 안전 검사
        if (inputFields == null || inputFields.Length < 12)
        {
//            Debug.LogWarning("[BalanceManager] inputFields 길이가 부족합니다!");
            ClosePanel();
            return;
        }

        // ------------------------------------------------------
        // [0] 차량 속도
        if (float.TryParse(inputFields[0].text, out float v0))
            global.carSpeed = v0;
        // [1] 리젠 속도
        if (float.TryParse(inputFields[1].text, out float v1))
            global.carSpawnSpeed = v1;
        // [2] 특이차량 등장 확률
        if (int.TryParse(inputFields[2].text, out int v2))
            global.carBadPer = v2;

        // [3] 틱당 라이프 감소
        if (int.TryParse(inputFields[3].text, out int v3))
            global.lifeSubTick = v3;
        // [4] 단속 실수/실패 라이프 감소
        if (int.TryParse(inputFields[4].text, out int v4))
            global.lifeSubBadFail = v4;
        // [5] 블록 놓침 라이프 감소
        if (int.TryParse(inputFields[5].text, out int v5))
            global.lifeSubMiss = v5;
        // [6] 라인 파괴됨 라이프 감소
        if (int.TryParse(inputFields[6].text, out int v6))
            global.lifeSubLineBreak = v6;

        // [7] 단속 성공 라이프 회복
        if (int.TryParse(inputFields[7].text, out int v7))
            global.lifeAddBadSuccess = v7;
        // [8] 1줄 완성 라이프 회복
        if (int.TryParse(inputFields[8].text, out int v8))
            global.lifeAddLine1 = v8;
        // [9] 2줄 완성 라이프 회복
        if (int.TryParse(inputFields[9].text, out int v9))
            global.lifeAddLine2 = v9;
        // [10] 3줄 완성 라이프 회복
        if (int.TryParse(inputFields[10].text, out int v10))
            global.lifeAddLine3 = v10;
        // [11] 4줄 완성 라이프 회복
        if (int.TryParse(inputFields[11].text, out int v11))
            global.lifeAddLine4 = v11;
        // ------------------------------------------------------

        Debug.Log("[BalanceManager] 밸런스 값 저장 완료");

        ClosePanel();
    }

    void ClosePanel()
    {
        panelOpen = false;
        balancePanel.SetActive(false);
        Time.timeScale = prevTimeScale;     // 게임 재생
    }
}