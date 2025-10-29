using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq.Expressions;

public class StageController : MonoBehaviour
{

    public GameObject transitionPrefab;
    public SpriteRenderer lifeBarRenderer;       // 즉시 반영되는 게이지
    public SpriteRenderer lifeBarTempRenderer;   // 느리게 따라오는 게이지
    public TMP_Text scoreText;
    public TMP_Text targetScoreText;
    public TMP_Text stageText;

    public GameObject canvasResult;
    public GameObject canvasWin;
    public GameObject canvasLose;
    public GameObject canvasNick;
    public Button btnNextStage;                  // ← 여기 추가 (인스펙터 연결용)


    public GameObject playerPrefab;

    private int maxLife = 10000;
    private bool resultShown = false;

    public bool cloudMode = false;
    private int cloudTimer = 0;
    public GameObject cloudPrefab;

    [Header("브레이커 관련")]
    public GameObject breakerPrefab;   // <- 인스펙터에서 scr_breakerController 프리팹 연결
    public bool breakerMode = false;
    [Header("신호등 모드 관련")]
    public bool trafficLightMode = false;
    public int[] trafficLightLine = new int[] { 0, 0, 0 };

    [Header("신호등 모드 관련")]
    public bool rushHourMode = false;

    public bool infiniteMode = false;
    public int infiniteModeTargetScore = 1500;
    public int infiniteModeStage = 1;
    public GameObject infiniteModeLevelupPrefab;


    //무한모드 시스템 관련
    int[] result = new int[4];       // 0~3 자리, 기본은 0
    int[] prevResult = new int[4];   // 이전 상태 저장용

    void Start()
    {
        SoundManager.Instance.PlayBGM("Ingame");
        StageBalanceSetting();
        InfiniteBalanceSetting();
        global.isGameOver = 0;
        GameManager.Instance.life = 10000;
        GameManager.Instance.score = 0;

        if (lifeBarRenderer == null)
            Debug.LogError("lifeBarRenderer가 연결되지 않았습니다.");

        if (lifeBarTempRenderer == null)
            Debug.LogError("lifeBarTempRenderer가 연결되지 않았습니다.");

        if (btnNextStage == null)
            Debug.LogError("btnNextStage 버튼이 연결되지 않았습니다.");

        if (canvasResult != null) canvasResult.SetActive(false);
        if (canvasWin != null) canvasWin.SetActive(false);
        if (canvasLose != null) canvasLose.SetActive(false);
        if (canvasNick != null) canvasNick.SetActive(false);


        targetScoreText.text = $"TargetScore : {GameManager.Instance.GetTargetScore()}";
    }


    void Update()
    {
        //---------------------------------------------------------------
        //DEBUG FUNCTION
        //---------------------------------------------------------------
        /*
        if (Input.GetKeyDown(KeyCode.I))
        {
            global.StageMap += 1;
            if (global.StageMap>2)
            {
                global.StageMap = 0;
            }
        }*/

        if (Input.GetKeyDown(KeyCode.P))
        {
            global.carSpeed += 1;
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            global.carSpeed -= 1;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            GameManager.Instance.AddLife(-500);
        }
        /*
        if (Input.GetKeyDown(KeyCode.S))
        {
            GameManager.Instance.AddLife(200000);
        }
        */
        if (Input.GetKeyDown(KeyCode.D))
        {
            GameManager.Instance.AddScore(500);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (cloudMode == false) { cloudMode = true; } else { cloudMode = false; }
            cloudTimer = 1300;
            Debug.Log("CloudMode" + cloudMode);
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            rushHourMode = !rushHourMode;
            Debug.Log("rushHour Mode: " + rushHourMode);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            trafficLightMode = !trafficLightMode;
            Debug.Log("TrafficLight Mode: " + trafficLightMode);

            if (trafficLightMode)
            {
                trafficLightLine = new int[] { 1, 2, 3 };
                /*
                // 1, 2, 3 배열 만들고 랜덤 순서로 섞기 (간단 버전)
                int[] nums = { 1, 2, 3 };
                for (int i = 0; i < nums.Length; i++)
                {
                    int rand = Random.Range(0, nums.Length);
                    int temp = nums[i];
                    nums[i] = nums[rand];
                    nums[rand] = temp;
                }
                trafficLightLine = nums;*/
            }
            else
            {
                trafficLightLine = new int[] { 0, 0, 0 };
            }

        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            breakerMode = !breakerMode;
            Debug.Log("Breaker Mode: " + breakerMode);
        }

        //---------------------------------------------------------------



        if (cloudMode == true)
        {
            cloudTimer += 1;
            if (cloudTimer > 600)
            {
                cloudTimer = 0;
                for (var i = 0; i < 2; i++)
                {
                    Vector3 spawnPos = new Vector3(12f + i * 4, Random.Range(-4f, 0f), 0f);
                    Instantiate(cloudPrefab, spawnPos, Quaternion.identity);
                }
                Debug.Log("Cloud Created!!!");
                cloudTimer = 0;
            }
        }



        // 🔹 breaker 모드일 때 랜덤 생성 시도
        if (breakerMode)
        {
            int rand = Random.Range(1, 201); // 1~200
            if (rand == 1)
            {
                SpawnBreaker();
            }
        }


        if (GameManager.Instance.GetLife() > maxLife)
        {
            GameManager.Instance.SetLife(maxLife);
        }
        if (GameManager.Instance == null) return;



        GameStageBlockController controller = FindObjectOfType<GameStageBlockController>();
        if (controller != null)
        {
            if (controller.infiniteModeDelay == false)
            {
                GameManager.Instance.AddLife(-1);
            }
        }


        float life = Mathf.Clamp(GameManager.Instance.GetLife(), 0, maxLife);

        // 즉시 반영 게이지
        if (lifeBarRenderer != null)
        {
            Vector3 scale = lifeBarRenderer.transform.localScale;
            scale.x = (life * 86f) / maxLife;
            lifeBarRenderer.transform.localScale = scale;
        }

        // 느리게 따라오는 게이지
        if (lifeBarTempRenderer != null && lifeBarRenderer != null)
        {
            Vector3 current = lifeBarTempRenderer.transform.localScale;
            float targetX = lifeBarRenderer.transform.localScale.x;
            current.x += (targetX - current.x) / 30f;
            lifeBarTempRenderer.transform.localScale = current;
        }

        if (scoreText != null)
        {
            scoreText.text = $"{GameManager.Instance.GetScore()}";
        }


        /*
        {
            GameManager.Instance.AddLife(-500);
        }*/





        //무한모드 목표점수 도달
        if (infiniteMode == true)
        {
            if (GameManager.Instance.GetScore() >= infiniteModeTargetScore )
            {
                
                infiniteModeStage += 1;
                Debug.Log("스테이지 레벨업" + infiniteModeStage);
                InfiniteBalanceSetting();
                //Instantiate(infiniteModeLevelupPrefab);


                //GameStageBlockController controller = FindObjectOfType<GameStageBlockController>();
                if (controller != null)
                {
                    controller.infiniteModeDelay = true;
                    controller.FunctionDestoryBlock();
                    controller.DestroyPreviewBlock();
                    /*
                        위에 이동하는 차는 제거되는데, 설치한 차량과 그런건 안지워짐
                        늘 그랬던것처럼 시간지나면 다시 자동차 등장함. 자동차 안멈췄음.
                     */
                    Debug.Log("해당 코드 작동 완료");
                }


                /*
                    트렌지션연출, 여기서 global.StageMap도 같이 바꾸셈
                    Instantiate(transitionPrefab);
                    저것.isRealMove = false;
                    이러면 그냥 나왔다가 아무것도 안하고 사라지겠지.
                    화면 가리는동안 중요한거 쫙 쳐내고.
                */

                FoldTransition transition = Instantiate(transitionPrefab).GetComponent<FoldTransition>();
                transition.isRealMove = false;

                cloudMode = false;
                breakerMode = false;
                trafficLightMode = false;
                rushHourMode = false;

                
                Invoke("InfiniteModeDestroy", 0.7f);
                Invoke("InfiniteModeRestart", 2f);


                // ##### 2025 - 10 - 28 ####
                //invoke 사용해서 RealStart 도 1로 풀어주고, 모드도 랜덤세팅하는거 추가
            }
        }




        //일반모드 목표 점수 도달
        if (infiniteMode == false)
        {
            if (GameManager.Instance.GetScore() >= GameManager.Instance.targetScore && global.stageNow < 50)
            {
                GameManager.Instance.AddLife(-10000);
            }
        }




        if (!resultShown && GameManager.Instance.GetLife() <= 0)
        {
            resultShown = true;
            global.isGameOver = 1;

           


            if (canvasResult != null)
            {

                if (global.stageNow == 100)
                {
                    if (canvasNick != null)
                    {
                        canvasNick.SetActive(true);
                        //global.stage += 1;
                    }
                }
                else
                {
                    if (GameManager.Instance.GetScore() >= GameManager.Instance.targetScore)
                    {
                        if (canvasWin != null)
                        {
                            canvasWin.SetActive(true);
                        }
                        if (btnNextStage != null)
                        {
                            btnNextStage.interactable = true;
                        }
                        global.stage += 1;

                        PlayerFSM PlayerScr = playerPrefab.GetComponent<PlayerFSM>();
                        if (PlayerScr != null)
                        {
                            PlayerScr.HighScore(); //B.cs 안의 C 함수 호출
                        }
                    }
                    else
                    {
                        if (canvasLose != null)
                        {
                            canvasLose.SetActive(true);
                        }
                        if (btnNextStage != null)
                        {
                            btnNextStage.interactable = false;
                        }
                        PlayerFSM PlayerScr = playerPrefab.GetComponent<PlayerFSM>();
                        if (PlayerScr != null)
                        {
                            PlayerScr.GameOver(); //B.cs 안의 C 함수 호출
                        }


                    }
                    canvasResult.SetActive(true);
                }
            }
        }
    }

    void InfiniteModeDestroy()
    {
        GameStageBlockController controller = FindObjectOfType<GameStageBlockController>();
        if (controller != null)
        {
            controller.ResetAllBlocks();
        }
        global.StageMap += 1;
        if (global.StageMap > 2)
        {
            global.StageMap = 0;
        }
        InfiniteModeShuffle();
    }

    void InfiniteModeRestart()
    {


       


        GameStageBlockController controller = FindObjectOfType<GameStageBlockController>();
        if (controller != null)
        {
            controller.infiniteModeDelay = false;
        }



    }
    /*
     ButtonSystem
     */
    public void ButtonNext_Click()
    {
        //SceneManager.LoadScene("Scene_Title");
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
        /*
        global.mapChange = "Scene_StageSelect";
        if (!string.IsNullOrEmpty(global.mapChange)) {Instantiate(transitionPrefab);}*/
    }
    public void ButtonStage_Click()
    {
      //  SceneManager.LoadScene("Scene_StageSelect");

        global.mapChange = "Scene_StageSelect";
        if (!string.IsNullOrEmpty(global.mapChange)) { Instantiate(transitionPrefab); }

    }
    public void ButtonRestart_Click()
    {
        //SceneManager.LoadScene("Scene_Title");
        Scene currentScene = SceneManager.GetActiveScene();
        //SceneManager.LoadScene(currentScene.name);

        global.mapChange = currentScene.name;
        if (!string.IsNullOrEmpty(global.mapChange)) { Instantiate(transitionPrefab); }

    }

    public void OnClickRank()
    {
//        SaveScore();
    }


    public void InfiniteModeShuffle()
    {
        // 초기화
        for (int i = 0; i < 4; i++)
            result[i] = 0;

        int activeCount = 0;

        // 단계별로 활성 개수 결정
        if (infiniteModeStage >= 10) activeCount = 3;
        else if (infiniteModeStage >= 5) activeCount = 2;
        else if (infiniteModeStage >= 3) activeCount = 1;

        // activeCount 가 0이면 아무것도 안 켬
        if (activeCount == 0)
        {
            SavePrevResult();
            return;
        }

        // 같은 조합이 반복되지 않도록 루프
        bool isSame = true;
        int safety = 0;

        while (isSame && safety < 100)
        {
            safety++;

            // 랜덤하게 unique한 index 선택
            List<int> indices = new List<int> { 0, 1, 2, 3 };
            for (int i = 0; i < activeCount; i++)
            {
                int r = Random.Range(0, indices.Count);
                int idx = indices[r];
                result[idx] = 1;
                indices.RemoveAt(r);
            }

            // 이전과 동일한지 체크
            isSame = AreSame(result, prevResult);

            // 같으면 다시 초기화하고 재시도
            if (isSame)
            {
                for (int i = 0; i < 4; i++)
                    result[i] = 0;
            }
        }

        SavePrevResult();

        if (result[0] == 1) { cloudMode = true; cloudTimer = 1300; }
        if (result[1] == 1) { trafficLightMode = true; trafficLightLine = new int[] { 1, 2, 3 }; }
        if (result[2] == 1) { breakerMode = true; }
        if (result[3] == 1) { rushHourMode = true; }
        

        if (rushHourMode==true)
        {
            global.carSpawnSpeed = infiniteModeStage / 15;
        }

        // 디버그 출력용
        Debug.Log($"[Stage {infiniteModeStage}] 결과: {string.Join(",", result)}");
    }

    bool AreSame(int[] a, int[] b)
    {
        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] != b[i]) return false;
        }
        return true;
    }

    void SavePrevResult()
    {
        for (int i = 0; i < 4; i++)
            prevResult[i] = result[i];
    }







    void InfiniteBalanceSetting()
    {
        if (infiniteMode == true)
        {
            infiniteModeTargetScore = 1200 + (infiniteModeStage - 1) * 1200 + infiniteModeStage * 300;
            global.carSpeed = 5 + infiniteModeStage * 0.3f;
        }
        /*
            여기서 랜덤하게 모드 실행하면 됨
            스테이지 1에서 3까지는 뭐없게
            3,4,5,6까지는 1가지
            7,8,9,10 까지는 2가지
            11부터 3가지 중첩될 수 있도록 세팅.

            또한 여기서 맵 배경 바뀌는 가벼운 연출 있었으면 좋겠는데, 태고의달인? 북도리 똑똑! 하고 휙 넘겨지면서 배경바뀌고 스피드업 연출나오는듯이?
            러쉬아워 끄면 바로 새거나오나? 이상없게, 러쉬아워 끄면 있는거 다 사라진 다음에 spawn 되게 하면 좋겠네.
        */
    }

    void StageBalanceSetting()
    {
        if (infiniteMode == false)
        {

            int dayInCycle = (global.stageNow - 1) % 5 + 1;
            switch (dayInCycle)
            {
                case 1:
                case 2:
                    global.StageMap = 0;
                    break;
                case 3:
                    global.StageMap = 1;
                    break;
                case 4:
                    global.StageMap = 2;
                    break;
                case 5:
                    global.StageMap = 0;
                    break;
            }


            // 기본값
            GameManager.Instance.targetScore = 1600 + global.stageNow * 400;
            global.carSpeed = 4 + (global.stageNow*0.3f);
            global.carSpawnSpeed = 1 - (global.stageNow - 1) * 0.04f;
            stageText.text = global.stageNow + "일차";

            // 1번째 금요일 : 안개
            if (global.stageNow == 5)
            {
                stageText.text = global.stageNow + "일차 (구름)";
                cloudMode = true;
                cloudTimer = 1300;
            }
            // 2번째 금요일 : 색깔 차량 단속
            else if (global.stageNow == 10)
            {
                stageText.text = global.stageNow + "일차 (신호등)";
                trafficLightMode = true;
                trafficLightLine = new int[] { 1, 2, 3 };
            }
            // 3번째 금요일 : 청소차
            else if (global.stageNow == 15)
            {
                stageText.text = global.stageNow + "일차 (청소차)";
                breakerMode = true;
            }
            // 4번째 금요일 : 러쉬 아워
            else if (global.stageNow == 20)
            {
                global.carSpeed = 5;
                stageText.text = global.stageNow + "일차 (러쉬아워)";
                rushHourMode = true;
            }
            // 무한모드 기본 세팅
            else if (global.stageNow == 100)
            {
                GameManager.Instance.targetScore = 999999999;
                global.carSpeed = 5;
                global.carSpawnSpeed = 1;
                stageText.text = "무한모드";
                infiniteMode = true;
            }
        }

    }


    void SpawnBreaker()
    {
        // 🔒 이미 브레이커가 존재하면 새로 소환하지 않음
        if (FindObjectOfType<scr_breakerController>() != null)
            return;

        int lineNum = Random.Range(1, 13); // 1~12 중 하나
        Vector3 spawnPos = GetLinePosition(lineNum);

        GameObject obj = Instantiate(breakerPrefab, spawnPos, Quaternion.identity);
        scr_breakerController breaker = obj.GetComponent<scr_breakerController>();
        if (breaker != null)
        {
            breaker.targetLine = lineNum;
        }

        Debug.Log($"[Breaker] 라인 {lineNum} 소환됨");
    }


    // 🔹 센서 첫번째 줄의 라인별 위치 반환 (가짜 예시, 네 좌표에 맞게 수정)
    // 라인 번호(1~12)에 맞춰 실제 그리드 기준 X좌표 반환
    Vector3 GetLinePosition(int lineIndex)
    {
        // 라인 인덱스 보정
        int index = lineIndex - 1;

        // 각 4x4 그리드의 구간 구분
        int gridIndex = index / 4;     // 0, 1, 2 중 하나
        int localX = index % 4;        // 각 4x4 내부 X좌표

        // GameStageBlockController의 파라미터와 동일하게 맞춤
        float cellSize = 0.88f;
        float gridSpacingX = 0.68f;
        float gridOriginOffsetX = -3f;

        // 🎯 X 좌표 계산
        float offsetX = gridIndex * (4 * cellSize + gridSpacingX);
        float worldX = gridOriginOffsetX + localX * cellSize + offsetX;

        // 🎯 Y 좌표 고정 (요청대로)
        float worldY = -2f;

        return new Vector3(worldX, worldY, 0f);
    }



}