using System.Collections.Generic;
using UnityEngine;

public class MoveCarController : MonoBehaviour
{
    public GameObject GameStageBlockController;
    private float boundaryX = 10f;
    private float moveSpeed = global.carSpeed;
    private GameObject playerPrefab;
    public GameObject rushHourCarPrefab;
    public int rushHourCarNumber = 0;
    private int rushHourCarExist = 0;
    public bool IsFalling = false;

    void Start()
    {

        playerPrefab = GameObject.Find("Player");
        string spriteIndex = "";
        StageController stageCtrl = FindObjectOfType<StageController>();


        // 차량 번호에 따른 스프라이트로 변경 및 사운드 재생
        // 1*1
        if (stageCtrl.rushHourMode == false && global.carNow == 1 || stageCtrl.rushHourMode == true && rushHourCarNumber == 1) { spriteIndex = "car1x1_01"; soundPlay("Car1x1"); }
        else if (stageCtrl.rushHourMode == false && global.carNow == 2 || stageCtrl.rushHourMode == true && rushHourCarNumber == 2) { spriteIndex = "car1x1_02"; soundPlay("Bike"); }
        else if (stageCtrl.rushHourMode == false && global.carNow == 3 || stageCtrl.rushHourMode == true && rushHourCarNumber == 3) { spriteIndex = "car1x1_03"; soundPlay("Bike"); }
        // 2*1
        else if (stageCtrl.rushHourMode == false && global.carNow == 11 || stageCtrl.rushHourMode == true && rushHourCarNumber == 11) { spriteIndex = "car1x2_01"; soundPlay("Truck"); }
        else if (stageCtrl.rushHourMode == false && global.carNow == 12 || stageCtrl.rushHourMode == true && rushHourCarNumber == 12) { spriteIndex = "car1x2_02"; soundPlay("Truck"); }
        else if (stageCtrl.rushHourMode == false && global.carNow == 13 || stageCtrl.rushHourMode == true && rushHourCarNumber == 13) { spriteIndex = "car1x2_03"; soundPlay("Truck"); }
        else if (stageCtrl.rushHourMode == false && global.carNow == 14 || stageCtrl.rushHourMode == true && rushHourCarNumber == 14) { spriteIndex = "car1x2_04"; soundPlay("Truck"); }
        else if (stageCtrl.rushHourMode == false && global.carNow == 15 || stageCtrl.rushHourMode == true && rushHourCarNumber == 15) { spriteIndex = "car1x2_05"; soundPlay("Truck"); }
        //NEW
        else if (stageCtrl.rushHourMode == false && global.carNow == 16 || stageCtrl.rushHourMode == true && rushHourCarNumber == 16) { spriteIndex = "car1x2_06"; soundPlay("Truck"); }
        else if (stageCtrl.rushHourMode == false && global.carNow == 17 || stageCtrl.rushHourMode == true && rushHourCarNumber == 17) { spriteIndex = "car1x2_07"; soundPlay("Truck"); }
        // 4*1
        else if (stageCtrl.rushHourMode == false && global.carNow == 21 || stageCtrl.rushHourMode == true && rushHourCarNumber == 21) { spriteIndex = "car1x4_01"; soundPlay("Limousine"); }
        // 2*2
        else if (stageCtrl.rushHourMode == false && global.carNow == 31 || stageCtrl.rushHourMode == true && rushHourCarNumber == 31) { spriteIndex = "car2x2_01"; soundPlay("BigTruck"); }

        // 3x1
        else if (stageCtrl.rushHourMode == false && global.carNow == 41 || stageCtrl.rushHourMode == true && rushHourCarNumber == 41) { spriteIndex = "car1x3_01"; soundPlay("BigTruck"); }

        string path = $"_Res_Zuo/Res_Stage/Res_Stage_Car/{spriteIndex}";

        Sprite newSprite = Resources.Load<Sprite>(path);

        if (newSprite != null)
        {
            SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = newSprite;
            }
        }
        else
        {
//            Debug.LogWarning($"Sprite not found at path: {path}");
        }

        // 30% 확률로 bad car 지정
        // 위 주석은 왜있는거지? BadCar 지정은 이미 다른데서 하고 여기는 그냥 드로우인데?
        if (global.carNow == 2 || global.carNow == 3 || global.carNow == 11) { global.isBadCar = true; Debug.Log(global.carNow); }

        if (stageCtrl != null && stageCtrl.trafficLightMode)
        {
            // 🚦 신호등 모드일 땐 13, 14, 15 외엔 전부 불량 차량 처리
            if (global.carNow != 13 && global.carNow != 14 && global.carNow != 15)
            {
                global.isBadCar = true;
            }
        }


        if (global.isBadCar)
        {
            SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
            PlayerFSM PlayerScr = playerPrefab.GetComponent<PlayerFSM>();
            if (PlayerScr != null)
            {
                PlayerScr.ComeLegal(); // 단속 차량 등장 표정
            }
        }
        else
        {
            SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        }
    }

    void soundPlay(string snd)
    {
        StageController stageCtrl = FindObjectOfType<StageController>();
        if (stageCtrl.rushHourMode == false)
        {
            SoundManager.Instance.PlaySFX(snd);
        }
    }
    void LateUpdate()
    {
        CheckCarFront();
    }

    void Update()
    {

        // ----------------------------------------------------------------
        // DEBUG CODE
        // ----------------------------------------------------------------

        // 내 우측에 차량이 있는지 체크 / 시각화
        // 분명 차량이 존재할텐데 왜 여러개가 같이 날라가고 있는거지?
        // 체크해보니까 꽁무니만 노리네? 2f 로 하니까 멈춰있다 가는 그거발생하고
        // 이게 그건가? rigidbody 때문인가? 저 DrawRay도 충돌로 인식을 하나?
        /*
        Vector2 myPos = transform.position;
        Vector2 targetPos = myPos + new Vector2(3f, 0f);
        Debug.DrawLine(myPos, targetPos, Color.yellow);
        Debug.DrawRay(targetPos, Vector2.up * 1f, Color.red);
        rushHourCarExist = 0;
        GameObject[] cars = GameObject.FindGameObjectsWithTag("CarRushHour");
        foreach (GameObject car in cars)
        {
            if (Vector2.Distance(car.transform.position, targetPos) < 1f)
            {
                rushHourCarExist = 1;
                return;
            }
        }

        */
       //CheckCarFront();

        // ----------------------------------------------------------------

        StageController stageCtrl = FindObjectOfType<StageController>();
        GameStageBlockController component = GameStageBlockController.GetComponent<GameStageBlockController>();
        // 이거 그냥 싹 덮어도 될듯? 게임 끝났는데 뭐 날리게 할거 아니잖아?
        if (global.isGameOver == 0)
        {
            moveSpeed = global.carSpeed;
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);

            // SPACE 눌러서 차량 날릴 경우 처리
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (rushHourCarExist == 0)
                {
                    IsFalling = true;
                    SoundManager.Instance.PlaySFX("SpaceBar");
                    if (global.isBadCar == true)
                    {
                        GameManager.Instance.AddLife(global.lifeAddBadSuccess);
                        GameManager.Instance.AddScore(150);
                        PlayerFSM PlayerScr = playerPrefab.GetComponent<PlayerFSM>();
                        if (PlayerScr != null)
                        {
                            component.ComboUpdate(0);
                            PlayerScr.CheckSuccess(); // 단속 성공 표정
                        }
                    }
                    else
                    {
                        GameManager.Instance.AddLife(-global.lifeSubBadFail);
                        PlayerFSM PlayerScr = playerPrefab.GetComponent<PlayerFSM>();
                        if (PlayerScr != null)
                        {
                            component.ComboUpdate(1);
                            PlayerScr.CheckFail(); // 단속 실수 표정
                        }
                    }


                    //스페이스바 누를시 블럭이 날라가는 연출
                    GetComponent<FallingBlock>().FallGone();

                    //임시로 여기에 붙여봄. 맵 밖으로 나가지 않아도 PreviewBlock 을 바로 제거 할 수 있나 해서.
                    //이거 지우니까, 날라가는 이펙트도 없이 바로 사라지네. 이 부분은 함 체크해야할듯.
                    //뭐.. 러쉬아워에서는 오히려좋긴하지만, 일반모드에서는 연출이 사라지는거니까
                    //달려오는 놈은 맵 나가면 지우고, 아래있는 Preview만 바로제거되도록.
                    //아마 ReloadBlock을 즉시실행하고, DestroyBlock 을 아래 맵 나갈시 작동되는 코드에서 실행하면 될듯.

                    if (GameStageBlockController != null)
                    {

                        if (component != null)
                        {
                            // 러쉬 아워 모드에선 스페이스바 누르자마자 바로 작동

                            if (stageCtrl != null && stageCtrl.rushHourMode)
                            {
                                //component.DestroyPreviewBlock();
                            }



                            component.FunctionReloadBlock();
                            component.FunctionDestoryBlock();
                        }
                    }
                    global.isBadCar = false;


                }
            }


            // 차량을 설치/단속 하지 않아서 오른쪽 밖으로 나갔을 때 처리
            if (transform.position.x > boundaryX)
            {
                GameManager.Instance.AddLife(global.lifeSubMiss);
                PlayerFSM PlayerScr = playerPrefab.GetComponent<PlayerFSM>();
                if (PlayerScr != null)
                {
                    if (global.isBadCar == true)
                    {
                        PlayerScr.CheckFail();
                    }
                    else
                    {
                        PlayerScr.CheckMistake();
                    }
                }

                if (stageCtrl != null && stageCtrl.rushHourMode)
                {
                    //component.DestroyPreviewBlock();
                    component.FunctionRushHourReload();
                }
                ReloadCar();
            }
        }
    }

    public void ReloadCar()
    {
        if (GameStageBlockController != null)
        {
            GameStageBlockController component = GameStageBlockController.GetComponent<GameStageBlockController>();
            if (component != null)
            {
                component.FunctionReloadBlock();
                component.FunctionDestoryBlock();
            }
        }
        global.isBadCar = false;
        Destroy(gameObject);
    }

    void CheckCarFront()
    {
        // 기본값: 앞에 차량이 없다고 가정
        rushHourCarExist = 0;

        // 씬 안의 모든 CarRushHour 오브젝트 검색
        GameObject[] cars = GameObject.FindGameObjectsWithTag("CarRushHour");

        // 내 위치
        float myX = transform.position.x;

        foreach (GameObject car in cars)
        {
            if (car == null || car == gameObject)
                continue;

            MoveCarController mc = car.GetComponent<MoveCarController>();
            if (mc == null)
                continue;

            // IsFalling이 true면 무시
            if (mc.IsFalling)
                continue;

            // 자신보다 오른쪽(x가 더 큼)에 있는 차량이 하나라도 있다면
            if (car.transform.position.x > myX + 0.5f)
            {
                rushHourCarExist = 1;
                break; // 이미 하나 찾았으면 더 볼 필요 없음
            }
        }
    }


}