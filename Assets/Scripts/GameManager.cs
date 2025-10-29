using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class global
{
    public static int stage = 1;
    public static int stageNow = 0;
    public static float carSpeed = 4;
    public static bool isBadCar = false;
    public static int isGameOver = 0;
    public static int CarPass = 0;
    public static float carSpawnSpeedFirst = 7f; //스테이지 시작시 첫 자동차 소환 딜레이시간
    public static float carSpawnSpeed = 1f; //자동차 사라졌을때 몇초 뒤에 소환하는가

    public static float carBadPer = 30;

    public static string mapChange = "";

    public static int StageMap=0;

    public static int carNow = 0;
}




public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // 
    [Header("Game Data")]
    public int score = 0;
    public int life = 10000;
    public int targetScore = 1000;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            //Screen.SetResolution(1280, 720, false);
            QualitySettings.vSyncCount = 0; // VSync 비활성화 (직접 FPS 제한)
            Application.targetFrameRate = 60; // FPS를 60으로 고정
            Time.timeScale = 1f;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int value)
    {
        score += value;
    }

    public void AddLife(int value)
    {
        life += value;
    }
    public void SetLife(int value)
    {
        life = value;
    }




    public int GetScore() => score;
    public int GetLife() => life;
    public int GetTargetScore() => targetScore;


    //for Debug
    void Update()
    {
        OnRightClick();
        if (score < 1) { score = 0; }
    }

    void OnRightClick()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log(global.stage);
        }
    }

}
