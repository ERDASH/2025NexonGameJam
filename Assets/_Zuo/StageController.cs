using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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




    void Start()
    {
        
        SoundManager.Instance.PlayBGM("Ingame");


        if (global.stageNow == 1) { GameManager.Instance.targetScore = 3000; global.carSpeed = 5;  stageText.text = "STAGE1 - 산뜻한 월요일"; }
        if (global.stageNow == 2) { GameManager.Instance.targetScore = 4000; global.carSpeed = 7; stageText.text = "STAGE2 - 뜨거운 화요일"; }
        if (global.stageNow == 3) { GameManager.Instance.targetScore = 5000; global.carSpeed = 8; stageText.text = "STAGE3 - 긴박한 수요일"; }
        if (global.stageNow == 4) { GameManager.Instance.targetScore = 6000; global.carSpeed = 8f; stageText.text = "STAGE4 - 지친 목요일"; }
        if (global.stageNow == 5) { GameManager.Instance.targetScore = 7000*700; global.carSpeed = 11; stageText.text = "STAGE5 - 불타는 금요일"; }

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
        Debug.Log("now: "+global.stageNow);
        Debug.Log("stg: "+global.stage);

        if (GameManager.Instance.GetLife() > maxLife)
        {
            GameManager.Instance.SetLife(maxLife);
        }
        if (GameManager.Instance == null) return;

        GameManager.Instance.AddLife(-1);
        float life = Mathf.Clamp(GameManager.Instance.GetLife(), 0, maxLife);

        // 즉시 반영 게이지
        if (lifeBarRenderer != null)
        {
            Vector3 scale = lifeBarRenderer.transform.localScale;
            scale.x = (life * 100f) / maxLife;
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
        if (Input.GetKeyDown(KeyCode.G))
        {
            GameManager.Instance.AddLife(-500);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            GameManager.Instance.AddLife(200000);
         //   GameManager.Instance.AddScore(100);
        }
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            GameManager.Instance.AddScore(500);
        }


        if (GameManager.Instance.GetScore() >= GameManager.Instance.targetScore && global.stageNow < 5)
        {
            GameManager.Instance.AddLife(-10000);
        }

        if (!resultShown && GameManager.Instance.GetLife() <= 0)
        {
            resultShown = true;
            global.isGameOver = 1;

           


            if (canvasResult != null)
            {
                
                if (global.stageNow == 5)
                {
                    if (canvasNick != null)
                    {
                        canvasNick.SetActive(true);
                        global.stage += 1;
                    }
                }
                else  if (GameManager.Instance.GetScore() >= GameManager.Instance.targetScore)
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
}