using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using static Unity.Collections.AllocatorManager;

public class GameStageBlockController : MonoBehaviour
{

    public GameObject moveCar;
    public GameObject rushHourCar;
    public GameObject sensorPrefab, blockPrefab, previewPrefab;
    public GameObject playerPrefab;

    // Grid 사이즈 및 간격 조절
    private float cellSize = 0.88f;
    private float gridSpacingX = 0.68f;
    private float gridSpacingY = 0f;
    private float gridOriginOffsetX = -3f;
    private float gridOriginOffsetY = -3.5f;
    private int gridsPerRow = 3;

    // 키보드 입력 관련
    private float moveDelay = 0.2f; // 키를 꾹 눌렀을 때 첫 지연 시간
    private float moveRepeatRate = 0.05f; // 그 이후 반복 간격
    private float moveTimer = 0f;
    private int moveDirection = 0; // -1 = 왼쪽, 1 = 오른쪽
    private bool isMoving = false;

    private Sensor[,] sensors1 = new Sensor[4, 4];
    private Sensor[,] sensors2 = new Sensor[4, 4];
    private Sensor[,] sensors3 = new Sensor[4, 4];

    private List<GameObject> blocks1 = new List<GameObject>();
    private List<GameObject> blocks2 = new List<GameObject>();
    private List<GameObject> blocks3 = new List<GameObject>();

    private GameObject currentBlock, previewBlock;
    private Vector2 pos = new Vector2(0, 3);
    private int rotation = 0;
    private SpriteRenderer previewRenderer;
    private Color defaultColor;

    private float carSpawnTimer = 0f;
    private bool isGameStart = false;
    public int RealStart = 0;
    public bool infiniteModeDelay = false;

    private int rushHourCarRandom = 0;
    private int[] rushHourCarList = new int[10];



    enum BlockType { OneByOne, OneByTwo, OneByFour, TwoByTwo }
    private BlockType currentBlockType;

    void Start()
    {
        CreateGrids();
    }




    public void FunctionRushHourInit()
    {
        StageController stageCtrl = FindObjectOfType<StageController>();
        if (stageCtrl != null && stageCtrl.rushHourMode)
        {
            GameObject[] rushCars = GameObject.FindGameObjectsWithTag("CarRushHour");
            if (rushCars.Length < 10)
            {
                // 0번이 사라졌다고 가정 → 모든 차량을 우측으로 한 칸씩 당기기
                // 즉, arr[1] → arr[0], arr[2] → arr[1], ..., arr[9]는 비게 됨
                for (int i = 0; i < rushHourCarList.Length - 1; i++)
                {
                    rushHourCarList[i] = rushHourCarList[i + 1];
                }

                // 맨 오른쪽(=가장 왼쪽 위치에 해당하는) 9번 자리에 랜덤 차량 추가

                int rushHourCarRandom = SpawnBlockRandom(); // Random.Range(11, 16);
                rushHourCarList[rushHourCarList.Length - 1] = rushHourCarRandom;

                // 스폰 위치 계산
                Vector2 spawnPos;

                if (rushCars.Length == 0)
                {
                    // 맵에 rushCars가 하나도 없으면 기본 위치에 생성
                    spawnPos = new Vector2(-10f, 2.3f);
                }
                else
                {
                    // rushCars 중 "가장 왼쪽(x가 가장 작은)" 차량 찾기
                    GameObject leftMostCar = rushCars[0];
                    float leftMostX = leftMostCar.transform.position.x;

                    for (int i = 1; i < rushCars.Length; i++)
                    {
                        if (rushCars[i].transform.position.x < leftMostX)
                        {
                            leftMostCar = rushCars[i];
                            leftMostX = rushCars[i].transform.position.x;
                        }
                    }

                    // 맨 왼쪽 차량보다 더 왼쪽(-2.5f) 지점에 새 차량 스폰
                    spawnPos = new Vector2(leftMostX - 3f, 2.3f);
                }

                // 새 차량 생성
                DestroyPreviewBlock();
                GameObject newCar = Instantiate(rushHourCar, spawnPos, Quaternion.identity);

                // 차량 정보 세팅
                MoveCarController carScript = newCar.GetComponent<MoveCarController>();
                if (carScript != null)
                {
                    carScript.rushHourCarNumber = rushHourCarRandom;
                }

                SpawnBlock();
            }

            // 차량 당기기가 모두 완료되면, 현재 플레이어가 관여하는 차량을 맨 우측 차량으로 선택
            global.carNow = rushHourCarList[0];
        }
        else
        {
            for (int i = 0; i < rushHourCarList.Length; i++)
            {
                rushHourCarList[i] = -1;
            }
        }
    }

    public void FunctionRushHourReload()
    {
        StageController stageCtrl = FindObjectOfType<StageController>();
        if (stageCtrl != null && stageCtrl.rushHourMode)
        {
            //DestroyPreviewBlock();
            // 맵 밖에 나간 것에 대해서는 잔상이 존재함.
            // 스페이스바로 날린 것은 Preview 가 즉시 사라지지 않음.
            // 이 부분 수정 필요
        }
    }


    public void FunctionDestoryBlock()
    {

        Debug.Log("이건작동되나111");

        //   ClearCurrentBlock();

        for (var i = 0; i < 10; i++)
        {
            GameObject existingCar = GameObject.FindWithTag("Car");
            if (existingCar != null)
            {
                //Debug.Log("why?");
                Destroy(existingCar);
            }


            GameObject existingCar2 = GameObject.FindWithTag("CarRushHour");
            if (existingCar2 != null)
            {
                Destroy(existingCar2);
            }
        }
        carSpawnTimer = 0;

    }

    public void FunctionReloadBlock()
    {
        Debug.Log("이건작동되나222");
        global.CarPass = 1;


        GameObject existingCar = GameObject.FindWithTag("Block");
        if (existingCar != null)
        {
            FallingBlock fb = existingCar.GetComponent<FallingBlock>();
            if (fb != null && fb.isPreviewBlock == true)
            {
                Destroy(existingCar);
            }
        }

        GameObject existingCar2 = GameObject.FindWithTag("CarRushHour");
        if (existingCar2 != null)
        {
            Destroy(existingCar2);
        }

        carSpawnTimer = 0;


        // --------------------------------------------------------
        // CurrentBlock 삭제 및 투명화 로직
        // --------------------------------------------------------
        if (currentBlock != null)
        {
            foreach (Transform child in currentBlock.transform)
            {
                SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    Color c = sr.color;
                    c.a = 0f;
                    sr.color = c;
                }
            }
            Destroy(currentBlock);
            currentBlock = null;
        }

        if (previewBlock != null)
        {
            foreach (Transform child in previewBlock.transform)
            {
                SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    Color c = sr.color;
                    c.a = 0f;
                    sr.color = c;
                }
            }
            Destroy(previewBlock);
            previewBlock = null;
        }

        // previewRenderer도 투명화
        if (previewRenderer != null)
        {
            Color c = previewRenderer.color;
            c.a = 0f;
            previewRenderer.color = c;
        }
        // --------------------------------------------------------

        FunctionRushHourReload();
    }

    public void FunctionSpawnBlock()
    {

        StageController stageCtrl = FindObjectOfType<StageController>();
        ClearCurrentBlock();
        if (global.isGameOver == 0)
        {
            global.CarPass = 0;
            carSpawnTimer = 0;
            isGameStart = true;
            SpawnBlock();
        }
    }

    void CarExistCheck()
    {
        GameObject existingCar = GameObject.FindWithTag("Car");
        StageController stageCtrl = FindObjectOfType<StageController>();


        // 🔸 러시아워 모드면 대기 없이 즉시 스폰
        if (stageCtrl != null && stageCtrl.rushHourMode && RealStart == 1)
        {

            // 20251022 001 ########################################################
            DestroyPreviewBlock();
            GameObject[] rushCars = GameObject.FindGameObjectsWithTag("CarRushHour");
            if (rushCars.Length < 10)
            {
                FunctionSpawnBlock();
            }
            // 20251022 001 ########################################################

        }
        // 씬에 Car 태그 오브젝트가 없다면 타이머 작동
        else if (existingCar == null && RealStart == 1)
        {
            carSpawnTimer += Time.deltaTime;


            if (carSpawnTimer >= global.carSpawnSpeed)
            {
                FunctionSpawnBlock();
                carSpawnTimer = 0f;
            }
        }
        else
        {
            carSpawnTimer = 0f;
        }
    }



    void StartCheck()
    {
        RealStart = 1;
    }

    void Update()
    {
        //------------------------------------
        // Debug 
        //------------------------------------
        /*
        if (Input.GetKeyDown(KeyCode.P))
        {
            CheckAndClearLineBlock(3);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("현재 컨트롤할 차량 : " + global.carNow);
        }
        */
        //------------------------------------

        if (isGameStart == true && infiniteModeDelay == false)
        {
            CarExistCheck();
            FunctionRushHourInit();
            UpdateKeyInput();
            UpdateBlock();
            UpdatePreview();
        }
        Invoke("StartCheck", global.carSpawnSpeedFirst);

    }


    void TryMoveLeft()
    {
        if (pos.x > 0)
            pos.x -= 1;
    }

    void TryMoveRight()
    {
        if (pos.x < (gridsPerRow * 4 - 1))
            pos.x += 1;
    }

    public void ClearCurrentBlock()
    {
        if (currentBlock != null) Destroy(currentBlock);
        if (previewBlock != null) Destroy(previewBlock);
        currentBlock = null;
        previewBlock = null;
    }

    void CreateGrids()
    {

        Invoke("FunctionSpawnBlock", global.carSpawnSpeedFirst);

        for (int g = 0; g < 3; g++)
        {
            Sensor[,] sensors = g == 0 ? sensors1 : (g == 1 ? sensors2 : sensors3);
            float offsetX = g * (4 * cellSize + gridSpacingX);
            float offsetY = g * gridSpacingY;

            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    Vector2 pos = new Vector2(
                        gridOriginOffsetX + x * cellSize + offsetX,
                        gridOriginOffsetY + y * cellSize + offsetY);
                    GameObject s = Instantiate(sensorPrefab, pos, Quaternion.identity);
                    sensors[x, y] = s.GetComponent<Sensor>();
                }
            }
        }
    }

    void UpdateKeyInput()
    {

        if (global.isGameOver == 0)
        {

            if (currentBlock == null)
            {
                isMoving = false;
                moveDirection = 0;
                return;
            }



            // 누르는 순간: 방향 설정 + 즉시 이동
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                moveDirection = -1;
                moveTimer = moveDelay;
                TryMoveLeft();
                isMoving = true;
                SoundManager.Instance.PlaySFX("LeftRightUpArrow");
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                moveDirection = 1;
                moveTimer = moveDelay;
                TryMoveRight();
                isMoving = true;
                SoundManager.Instance.PlaySFX("LeftRightUpArrow");
            }
            else if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
            {
                moveDirection = 0;
                isMoving = false;
            }


            if (Input.GetKeyDown(KeyCode.UpArrow) && currentBlockType != BlockType.TwoByTwo)
            {
                rotation = (rotation == 0) ? 270 : 0;
            }


            if (Input.GetKeyDown(KeyCode.DownArrow))
            {

                SoundManager.Instance.PlaySFX("DownArrow");

                StageController stageCtrl = FindObjectOfType<StageController>();
                int gridIndex = Mathf.FloorToInt(pos.x / 4); // 현재 블록이 속한 그리드 (0~2)

                // 💡 기본 badcar 처리
                bool isBad = global.isBadCar;

                // 🚦 trafficLightMode일 경우, 배치 가능 차량 확인
                if (stageCtrl != null && stageCtrl.trafficLightMode)
                {
                    int requiredColor = stageCtrl.trafficLightLine[gridIndex]; // 1=빨강, 2=파랑, 3=초록
                    int carColor = global.carNow switch
                    {
                        15 => 1, // 빨강
                        13 => 2, // 파랑
                        14 => 3, // 초록
                        _ => 0
                    };

                    // ❌ 색이 다르면 잘못된 배치로 처리
                    if (requiredColor != 0 && requiredColor != carColor)
                    {
                        isBad = true;
                        Debug.Log($"[TrafficLightMode] ❌ 잘못된 차량 배치! (필요={requiredColor}, 현재={carColor})");
                    }
                    else
                    {
                        Debug.Log($"[TrafficLightMode] ✅ 올바른 차량 배치! (Grid {gridIndex + 1}, 색상 {carColor})");
                    }
                }

                if (isBad)
                {
                    PlayerFSM PlayerScr = playerPrefab.GetComponent<PlayerFSM>();
                    if (PlayerScr != null)
                        PlayerScr.CheckFail();

                    GameManager.Instance.AddLife(-100);
                    FunctionReloadBlock();
                    FunctionDestoryBlock();
                }
                else
                {
                    PlaceBlock();
                }

                /*
                SoundManager.Instance.PlaySFX("DownArrow");
                if (global.isBadCar == true)
                {

                    PlayerFSM PlayerScr = playerPrefab.GetComponent<PlayerFSM>();
                    if (PlayerScr != null)
                    {
                        PlayerScr.CheckFail();
                    }


                    GameManager.Instance.AddLife(-100);
                    //GameManager.Instance.AddScore(-100);
                    FunctionReloadBlock();
                    FunctionDestoryBlock();

                }
                else
                {
                    PlaceBlock();
                }
                */
            }


            if (!Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
            {
                isMoving = false;
                moveDirection = 0;
                return;
            }



            // 연속 입력 처리
            if (isMoving && moveDirection != 0)
            {
                moveTimer -= Time.deltaTime;
                if (moveTimer <= 0f)
                {
                    if (moveDirection == -1) TryMoveLeft();
                    else if (moveDirection == 1) TryMoveRight();
                    moveTimer = moveRepeatRate; // 반복 간격 재설정
                }
            }
        }

    }

    void UpdateBlock()
    {
        if (currentBlock != null)
        {
            currentBlock.transform.position = GetWorldPosition(pos);
            currentBlock.transform.rotation = Quaternion.Euler(0, 0, rotation);
        }
    }

    void UpdatePreview()
    {
        if (previewBlock == null || previewRenderer == null) return;

        Vector2 previewPos = pos;

        // 먼저 떨어질 수 있을 때까지 떨어뜨림
        while (previewPos.y > 0 && CanDropBlock(GetBlockPositions(new Vector2(previewPos.x, previewPos.y - 1))))
            previewPos.y--;

        // ↓ 이제 진짜 위치 기준으로 판단
        Vector2[] positions = GetBlockPositions(previewPos);
        bool canPlace = CanPlaceBlock(positions);

        if (global.CarPass == 1)
        {
            previewRenderer.color = new Color(1f, 0f, 0f, 0f);
        }
        else
        {
            Color colorToSet = canPlace ? defaultColor : Color.red;
            colorToSet.a = defaultColor.a;
            previewRenderer.color = colorToSet;
        }

        previewBlock.transform.position = GetWorldPosition(previewPos);
        previewBlock.transform.rotation = Quaternion.Euler(0, 0, rotation);
    }

    bool CanDropBlock(Vector2[] positions)
    {
        foreach (Vector2 p in positions)
        {
            int px = Mathf.RoundToInt(p.x);
            int py = Mathf.RoundToInt(p.y);
            if (px < 0 || px >= 12 || py < 0 || py >= 4) return false;
            Sensor[,] grid = GetCurrentGrid(px);
            if (grid[px % 4, py].isOccupied) return false;
        }
        return true;
    }

    Vector2 GetWorldPosition(Vector2 logicalPos)
    {
        int gridIndex = (int)logicalPos.x / 4;
        float gridOffsetX = gridIndex * (4 * cellSize + gridSpacingX);
        float localX = (logicalPos.x % 4) * cellSize;
        float worldX = gridOriginOffsetX + gridOffsetX + localX;
        float worldY = gridOriginOffsetY + logicalPos.y * cellSize;
        return new Vector2(worldX, worldY);
    }

    Sensor[,] GetCurrentGrid(float x)
    {
        if (x < 4) return sensors1;
        else if (x < 8) return sensors2;
        else return sensors3;
    }

    bool CanPlaceBlock(Vector2[] positions)
    {
        if (positions.Length == 0) return false;
        int gridIndex = (int)positions[0].x / 4;
        foreach (Vector2 p in positions)
        {
            int px = Mathf.RoundToInt(p.x);
            int py = Mathf.RoundToInt(p.y);
            if ((px / 4) != gridIndex) return false;
            if (px < 0 || px >= 12 || py < 0 || py >= 4) return false;
            Sensor[,] grid = GetCurrentGrid(px);
            if (grid[px % 4, py].isOccupied) return false;
        }

        return true;
    }

    void PlaceBlock()
    {



        Vector2 previewPos = pos;
        while (previewPos.y > 0 && CanPlaceBlock(GetBlockPositions(new Vector2(previewPos.x, previewPos.y - 1))))
            previewPos.y--;

        Vector2[] positions = GetBlockPositions(previewPos);
        if (CanPlaceBlock(positions))
        {
            FunctionDestoryBlock();
            foreach (Vector2 p in positions)
            {
                int px = Mathf.RoundToInt(p.x);
                int py = Mathf.RoundToInt(p.y);
                Sensor[,] grid = GetCurrentGrid(px);
                grid[px % 4, py].isOccupied = true;
            }

            currentBlock.transform.position = GetWorldPosition(previewPos);
            foreach (Transform child in currentBlock.transform)
            {
                if (child.gameObject.activeSelf)
                {
                    SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        Color solidColor = sr.color;
                        solidColor.a = 1f;
                        sr.color = solidColor;
                    }
                }
            }

            int gridIndex = Mathf.FloorToInt(previewPos.x / 4);
            List<GameObject> targetList = gridIndex switch
            {
                0 => blocks1,
                1 => blocks2,
                2 => blocks3,
                _ => null
            };
            if (targetList != null) targetList.Add(currentBlock);

            Destroy(previewBlock);
            currentBlock = null;
            previewBlock = null;

            CheckAndClearFullBlocks();
        }
        else
        {
            bool isTryingTopRow = false;
            foreach (Vector2 p in positions)
            {
                int py = Mathf.RoundToInt(p.y);
                if (py == 3)
                {
                    isTryingTopRow = true;
                    break;
                }
            }

            if (isTryingTopRow)
            {
                int gridIndex = Mathf.FloorToInt(previewPos.x / 4);

                // 센서 초기화
                Sensor[,] grid = gridIndex switch
                {
                    0 => sensors1,
                    1 => sensors2,
                    2 => sensors3,
                    _ => null
                };
                if (grid != null)
                {
                    for (int x = 0; x < 4; x++)
                        for (int y = 0; y < 4; y++)
                            grid[x, y].isOccupied = false;
                }

                // 기존 블록 FallOff 처리
                List<GameObject> targetList = gridIndex switch
                {
                    0 => blocks1,
                    1 => blocks2,
                    2 => blocks3,
                    _ => null
                };
                if (targetList != null)
                {
                    foreach (GameObject b in targetList)
                    {
                        if (b != null)
                        {
                            FallingBlock fb = b.GetComponent<FallingBlock>();
                            if (fb != null) fb.FallOff();
                            SoundManager.Instance.PlaySFX("TrackFail");
                        }
                    }
                    targetList.Clear();
                }

                //
                if (previewBlock != null)
                {
                    if (!previewBlock.TryGetComponent<FallingBlock>(out var fb))
                        fb = previewBlock.AddComponent<FallingBlock>();
                    fb.FallOff();
                }

                // currentBlock은 그냥 제거
                if (currentBlock != null)
                {
                    Destroy(currentBlock);
                }

                currentBlock = null;
                previewBlock = null;
            }

            FunctionDestoryBlock();
        }
    }


    void CheckAndClearFullBlocks()
    {
        int totalClearedLines = 0;

        for (int g = 0; g < 3; g++)
        {
            Sensor[,] grid = g == 0 ? sensors1 : (g == 1 ? sensors2 : sensors3);
            List<GameObject> blockList = g == 0 ? blocks1 : (g == 1 ? blocks2 : blocks3);
            List<int> fullRows = new List<int>();

            for (int y = 0; y < 4; y++)
            {
                bool full = true;
                for (int x = 0; x < 4; x++)
                    if (!grid[x, y].isOccupied) full = false;
                if (full) fullRows.Add(y);
            }

            bool allOtherEmpty = true;
            for (int y = 0; y < 4; y++)
            {
                if (fullRows.Contains(y)) continue;
                for (int x = 0; x < 4; x++)
                    if (grid[x, y].isOccupied) allOtherEmpty = false;
            }

            if (fullRows.Count > 0 && allOtherEmpty)
            {
                foreach (int y in fullRows)
                    for (int x = 0; x < 4; x++)
                        grid[x, y].isOccupied = false;

                foreach (GameObject b in blockList)
                {
                    if (b != null)
                    {
                        FallingBlock fb = b.GetComponent<FallingBlock>();
                        if (fb != null)
                        {
                            fb.FallOn();
                            SoundManager.Instance.PlaySFX("BlockBoom");
                        }
                    }
                }
                blockList.Clear();

                PlayerFSM PlayerScr = playerPrefab.GetComponent<PlayerFSM>();
                if (PlayerScr != null)
                {
                    PlayerScr.SuccessBoom();
                }

                totalClearedLines += fullRows.Count;
            }
        }

        switch (totalClearedLines)
        {
            case 1:
                GameManager.Instance.AddScore(150);
                GameManager.Instance.AddLife(100);
                break;
            case 2:
                GameManager.Instance.AddScore(250);
                GameManager.Instance.AddLife(300);
                break;
            case 3:
                GameManager.Instance.AddScore(400);
                GameManager.Instance.AddLife(700);
                break;
            case 4:
                GameManager.Instance.AddScore(600);
                GameManager.Instance.AddLife(1500);
                break;
        }
    }


    Vector2[] GetBlockPositions(Vector2 center)
    {
        switch (currentBlockType)
        {
            case BlockType.OneByOne: return new Vector2[] { center };
            case BlockType.OneByTwo: return new Vector2[] { center, GetOffset(center, 1) };
            case BlockType.OneByFour: return new Vector2[] { center, GetOffset(center, 1), GetOffset(center, 2), GetOffset(center, 3) };
            case BlockType.TwoByTwo:
                return new Vector2[] {
                    center,
                    center + Vector2.right,
                    center + Vector2.up,
                    center + Vector2.right + Vector2.up
                };
        }
        return new Vector2[] { center };
    }

    Vector2 GetOffset(Vector2 basePos, int distance)
    {
        switch (rotation)
        {
            case 0: return basePos + Vector2.right * distance;
            case 90: return basePos + Vector2.up * distance;
            case 180: return basePos + Vector2.left * distance;
            case 270: return basePos + Vector2.down * distance;
        }
        return basePos;
    }

    public void DestroyPreviewBlock()
    {
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");

        foreach (GameObject block in blocks)
        {
            if (block == null) continue;

            FallingBlock fb = block.GetComponent<FallingBlock>();
            if (fb == null) continue;

            // ✅ currentBlock 또는 previewBlock 은 건너뛰기
            if (block == currentBlock || block == previewBlock)
                continue;

            bool shouldDestroy = false;

            // 🔍 자식들 포함한 모든 SpriteRenderer 검사
            SpriteRenderer[] renderers = block.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sr in renderers)
            {
                if (sr == null) continue;

                float alpha = sr.color.a;
                if (alpha > 0f && alpha < 0.9f)
                {
                    shouldDestroy = true;
                    break;
                }
            }

            // 조건에 맞는 블록 제거
            if (shouldDestroy)
            {
                Destroy(block);
                Debug.Log($"Destroyed preview-like block: {block.name}");
            }
        }
    }




    int SpawnBlockRandom()
    {
        int randReal = Random.Range(0, 100);
        int[] choice_car = { 1, 12, 13, 14, 15, 21, 31 };

        StageController stageCtrl = FindObjectOfType<StageController>();
        if (stageCtrl != null && stageCtrl.trafficLightMode)
        {
            // trafficLightMode가 켜져 있을 때는 반드시 12~15만 선택
            choice_car = new int[] { 12, 13, 14, 15, 16, 17 };
            Debug.Log("[TrafficLightMode] 전용 차량 선택 모드 ON (12~15만 등장)");
        }


        return choice_car[Random.Range(0, choice_car.Length)];


    }


    void SpawnBlock()
    {
        global.isBadCar = false;
        int randReal = Random.Range(0, 100);
        int[] choice_car = { 1, 12, 13, 14, 15, 21, 31 };
        StageController stageCtrl = FindObjectOfType<StageController>();

        // ----------------------------------------------
        // DEBUG 용 코드
        // ----------------------------------------------

        // 작동시 하단의 Block 중 Preview 블럭만 골라서 제거
        DestroyPreviewBlock();

        // ----------------------------------------------



        // ----------------------------------------------
        // 만약 신호등 모드이면 특정한 차만나오게
        // ----------------------------------------------
        if (stageCtrl != null && stageCtrl.trafficLightMode)
        {
            // trafficLightMode가 켜져 있을 때는 반드시 12~15만 선택
            choice_car = new int[] { 12, 13, 14, 15, 16, 17 };
            Debug.Log("[TrafficLightMode] 전용 차량 선택 모드 ON (12~15만 등장)");
        }
        // ----------------------------------------------




        // ----------------------------------------------
        // 소환할 차 선택
        global.carNow = choice_car[Random.Range(0, choice_car.Length)];

        if (randReal < global.carBadPer)
        {
            if (stageCtrl != null && stageCtrl.trafficLightMode)
            {
                // 신호등 모드일 경우 특수블럭 처리 (현재 비워둠)
                // 신호등 외 색상인 것들 리스트 넣어서 처리하면 됨.
            }
            else
            {
                // 일반 모드의 경우 특수블럭 처리
                if (global.stageNow == 1) { int[] choice_car2 = { 2, 3, 11 }; global.carNow = choice_car2[Random.Range(0, choice_car2.Length)]; }
                if (global.stageNow == 2) { int[] choice_car2 = { 2, 3, 11 }; global.carNow = choice_car2[Random.Range(0, choice_car2.Length)]; }
                if (global.stageNow == 3) { int[] choice_car2 = { 2, 3, 11 }; global.carNow = choice_car2[Random.Range(0, choice_car2.Length)]; }
                if (global.stageNow == 4) { int[] choice_car2 = { 2, 3, 11 }; global.carNow = choice_car2[Random.Range(0, choice_car2.Length)]; }
            }
        }




        // 러시아워 모드 작동 시엔, moveCar 나오지 않게
        // 그 이유는 RushHour 전용 MoveCar 을 생성할 것이기 때문
        if (stageCtrl != null && stageCtrl.rushHourMode == false)
        {
            GameObject newCar = Instantiate(moveCar, new Vector2(-10f, 2.3f), Quaternion.identity);
        }
        else
        {
            global.carNow = rushHourCarList[0];
        }
        // ----------------------------------------------








        // ----------------------------------------------
        // 소환할 차의 하단이미지 세팅
        // 각 값의 앖자리에 따라 1x1인지 2x1인지 4x1인지 2x2인지 판별
        int rand = (int)(global.carNow / 10);
        currentBlockType = (BlockType)rand;

        currentBlock = Instantiate(blockPrefab, GetWorldPosition(pos), Quaternion.identity);
        previewBlock = Instantiate(previewPrefab, GetWorldPosition(pos), Quaternion.identity);

        var blockScript = previewBlock.GetComponent<FallingBlock>();
        if (blockScript != null)
        {
            blockScript.isPreviewBlock = true;
        }



        string activeName = currentBlockType switch
        {
            BlockType.OneByOne => "Block01",
            BlockType.OneByTwo => "Block11",
            BlockType.OneByFour => "Block21",
            BlockType.TwoByTwo => "Block31",
            _ => ""
        };

        ActivateOnlyChild(currentBlock, activeName);
        ActivateOnlyChild(previewBlock, activeName);


        Transform previewChild = previewBlock.transform.Find(activeName);
        if (previewChild != null)
        {
            previewRenderer = previewChild.GetComponent<SpriteRenderer>();
            defaultColor = previewRenderer.color;
            defaultColor.a = 0.5f;
            previewRenderer.color = defaultColor;
        }

        //pos = new Vector2(0, 3);
        rotation = 0;

        // ▶ 현재 블록 자식 오브젝트 찾아서 알파값 조정 + 스프라이트 변경
        Transform currentChild = currentBlock.transform.Find(activeName);
        if (currentChild != null)
        {
            SpriteRenderer sr = currentChild.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                // 알파값 0으로 투명화
                Color c = sr.color;
                c.a = 0f;
                sr.color = c;

                // ▶ carNow에 맞는 스프라이트 로드
                string spriteName = global.carNow switch
                {
                    1 => "car1x1_01",
                    2 => "car1x1_02",
                    3 => "car1x1_03",
                    11 => "car1x2_01",
                    12 => "car1x2_02",
                    13 => "car1x2_03",
                    14 => "car1x2_04",
                    15 => "car1x2_05",
                    16 => "car1x2_06",
                    17 => "car1x2_07",
                    21 => "car1x4_01",
                    31 => "car2x2_01",
                    _ => null
                };

                if (!string.IsNullOrEmpty(spriteName))
                {
                    string path = $"_Res_Zuo/Res_Stage/Res_Stage_Car/gird_new/{spriteName}";
                    Sprite newSprite = Resources.Load<Sprite>(path);

                    if (newSprite != null)
                    {
                        sr.sprite = newSprite;
                    }
                    else
                    {
                        Debug.LogWarning($" Sprite not found at: Resources/{path}");
                    }
                }
            }
        }

        // ▶ previewBlock 스프라이트 설정
        Transform previewSpriteChild = previewBlock.transform.Find(activeName);
        if (previewSpriteChild != null)
        {
            SpriteRenderer sr = previewSpriteChild.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                string spriteName = global.carNow switch
                {
                    1 => "car1x1_01",
                    2 => "car1x1_02",
                    3 => "car1x1_03",
                    11 => "car1x2_01",
                    12 => "car1x2_02",
                    13 => "car1x2_03",
                    14 => "car1x2_04",
                    15 => "car1x2_05",
                    16 => "car1x2_06",
                    17 => "car1x2_07",
                    21 => "car1x4_01",
                    31 => "car2x2_01",
                    _ => null
                };

                if (!string.IsNullOrEmpty(spriteName))
                {
                    string path = $"_Res_Zuo/Res_Stage/Res_Stage_Car/gird_new/{spriteName}";
                    Sprite newSprite = Resources.Load<Sprite>(path);

                    if (newSprite != null)
                    {
                        sr.sprite = newSprite;
                    }
                    else
                    {
                        Debug.LogWarning($"Preview Sprite not found at: Resources/{path}");
                    }
                }
            }
        }
        // ----------------------------------------------



    }

    public void ResetAllBlocks()
    {
        foreach (var obj in GameObject.FindGameObjectsWithTag("Block")) Destroy(obj);
        foreach (var obj in GameObject.FindGameObjectsWithTag("Car")) Destroy(obj);
        foreach (var obj in GameObject.FindGameObjectsWithTag("CarRushHour")) Destroy(obj);

        ResetSensors();

        blocks1.Clear();
        blocks2.Clear();
        blocks3.Clear();
        currentBlock = null;
        previewBlock = null;
    }
    void ResetSensors()
    {
        Sensor[] allSensors = FindObjectsOfType<Sensor>();
        foreach (Sensor s in allSensors)
        {
            s.isOccupied = false;
        }
    }



    void ActivateOnlyChild(GameObject parent, string targetName)
    {
        foreach (Transform child in parent.transform)
            child.gameObject.SetActive(child.name == targetName);
    }



    // ----------------------------------------------
    // 라인 클리어 모드 관련 시스템
    // ----------------------------------------------
    public void CheckAndClearLineBlock(int destroyLine)
    {

        /*
        생각. Block 을 제거하려는게 맞는지, 센서를 제거하려는건 아닌지
        또한, 제거할때 Preview Block 은 제거하지 않아야한다.
         
         */


        /*
        if (destroyLine < 1 || destroyLine > 12)
        {
            Debug.LogWarning($"잘못된 destroyLine 값: {destroyLine}");
            return;
        }

        int lineIndex = destroyLine - 1;
        int gridIndex = lineIndex / 4;    // 어느 4x4 그리드인지 (0,1,2)
        int localX = lineIndex % 4;       // 그리드 내부 X좌표

        // 대상 그리드와 블록 리스트 선택
        Sensor[,] targetGrid = gridIndex switch
        {
            0 => sensors1,
            1 => sensors2,
            2 => sensors3,
            _ => null
        };
        List<GameObject> blockList = gridIndex switch
        {
            0 => blocks1,
            1 => blocks2,
            2 => blocks3,
            _ => null
        };
        if (targetGrid == null || blockList == null) return;

        // 🎯 1. 센서에서 실제로 켜진(isOccupied) 셀들 확인
        List<Sensor> hitSensors = new List<Sensor>();
        for (int y = 0; y < 4; y++)
        {
            if (targetGrid[localX, y].isOccupied)
            {
                hitSensors.Add(targetGrid[localX, y]);
                targetGrid[localX, y].isOccupied = false; // 바로 비활성화
            }
        }

        if (hitSensors.Count == 0)
        {
            Debug.Log($"라인 {destroyLine}: 센서에 블록 없음");
            return;
        }

        // 🎯 2. 각 블록이 해당 센서 위치에 올라있는지 확인
        foreach (GameObject b in new List<GameObject>(blockList))
        {
            if (b == null) continue;

            Vector3 bPos = b.transform.position;

            foreach (Sensor s in hitSensors)
            {
                // 블록 중심이 센서 근처에 있는지 체크 (좌표 차이 아주 미세하게)
                if (Vector2.Distance(s.transform.position, bPos) < cellSize * 0.4f)
                {
                    // 💥 해당 블록 제거
                    FallingBlock fb = b.GetComponent<FallingBlock>();
                    if (fb != null) fb.FallOff();
                    else Destroy(b);

                    blockList.Remove(b);
                    break; // 하나라도 맞으면 이 블록은 끝
                }
            }
        }

        // 🎵 효과 및 피드백
        SoundManager.Instance.PlaySFX("TrackFail");
        PlayerFSM playerScr = playerPrefab.GetComponent<PlayerFSM>();
        if (playerScr != null)
            playerScr.CheckFail();

        Debug.Log($"라인 {destroyLine} 제거 완료 ✅");
        */
    }
        
    

}



