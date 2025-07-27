using UnityEngine;

public class MoveCarController : MonoBehaviour
{
    public GameObject A; // GameStageBlockController 오브젝트
    private float boundaryX = 10f;
    private float moveSpeed = global.carSpeed;
    private GameObject playerPrefab;


    //    private bool isBadCar;

    void Start()
    {
        
        playerPrefab = GameObject.Find("Player");

        //  스프라이트 이름 설정
        string spriteIndex = "";

        if (global.carNow == 1) { spriteIndex = "car1x1_01"; SoundManager.Instance.PlaySFX("Car1x1"); }
        else if (global.carNow == 2) { spriteIndex = "car1x1_02"; SoundManager.Instance.PlaySFX("Bike");  }
        else if (global.carNow == 3) { spriteIndex = "car1x1_03"; SoundManager.Instance.PlaySFX("Bike");  }

        else if (global.carNow == 11) { spriteIndex = "car1x2_01"; SoundManager.Instance.PlaySFX("Truck");  }
        else if (global.carNow == 12) { spriteIndex = "car1x2_02"; SoundManager.Instance.PlaySFX("Truck");  }
        else if (global.carNow == 13) { spriteIndex = "car1x2_03"; SoundManager.Instance.PlaySFX("Truck");  }
        else if (global.carNow == 14) { spriteIndex = "car1x2_04"; SoundManager.Instance.PlaySFX("Truck");  }
        else if (global.carNow == 15) { spriteIndex = "car1x2_05"; SoundManager.Instance.PlaySFX("Truck");  }

        else if (global.carNow == 21) { spriteIndex = "car1x4_01"; SoundManager.Instance.PlaySFX("Limousine");  }

        else if (global.carNow == 31) { spriteIndex = "car2x2_01"; SoundManager.Instance.PlaySFX("BigTruck"); }

            string path = $"_Res_Zuo/Res_Stage/Res_Stage_Car/{spriteIndex}"; // Resources.Load 경로

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
                Debug.LogWarning($"Sprite not found at path: {path}");
            }

            // 30% 확률로 bad car 지정


            if (global.carNow == 2 || global.carNow == 3 || global.carNow == 11) { global.isBadCar = true; Debug.Log(global.carNow); }




            //  global.isBadCar = Random.value < 0.3f;

            if (global.isBadCar)
            {
                SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
                //if (sr != null) sr.color = Color.red;

                PlayerFSM PlayerScr = playerPrefab.GetComponent<PlayerFSM>();
                if (PlayerScr != null)
                {
                    PlayerScr.ComeLegal(); // 단속 차량 등장
                }
            }
            else
            {
                SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
                //if (sr != null) sr.color = Color.white;
            }
        }




    void Update()
    {
        if (global.isGameOver == 0)
        {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SoundManager.Instance.PlaySFX("SpaceBar");
            if (global.isBadCar == true)
            {
                GameManager.Instance.AddLife(300);
                GameManager.Instance.AddScore(150);

                PlayerFSM PlayerScr = playerPrefab.GetComponent<PlayerFSM>();
                if (PlayerScr != null)
                {
                    PlayerScr.CheckSuccess(); //단속 성공
                }
            }
            else
            {
                GameManager.Instance.AddLife(-200);
             //   GameManager.Instance.AddScore(-100);

                PlayerFSM PlayerScr = playerPrefab.GetComponent<PlayerFSM>();
                if (PlayerScr != null)
                {
                    PlayerScr.CheckFail(); //실수
                    //Debug.Log("CheckFail");
                }
            }

            GetComponent<FallingBlock>().FallGone();
           // ReloadCar();
        }

        if (transform.position.x > boundaryX) 
        {
            GameManager.Instance.AddLife(-200);
           // GameManager.Instance.AddScore(-100);

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
                //실수
                //Debug.Log("CheckMistake");
            }

            
            ReloadCar();
        }
    }

    public void ReloadCar()
    {
        if (A != null)
        {
            GameStageBlockController component = A.GetComponent<GameStageBlockController>();
            if (component != null)
            {
                component.FunctionReloadBlock();
                component.FunctionDestoryBlock();
            }
        }
        global.isBadCar = false;
        Destroy(gameObject);
    }
}