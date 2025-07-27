using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class GameStageBlockController : MonoBehaviour
{

    public GameObject moveCar;

    public GameObject sensorPrefab, blockPrefab, previewPrefab;
    private float cellSize = 0.88f;
    private float gridSpacingX = 0.68f;
    private float gridSpacingY = 0f;
    private float gridOriginOffsetX = -3f;
    private float gridOriginOffsetY = -3.5f;
    private int gridsPerRow = 3;


    private float moveDelay = 0.2f; // 키를 꾹 눌렀을 때 첫 지연 시간
    private float moveRepeatRate = 0.05f; // 그 이후 반복 간격
    private float moveTimer = 0f;
    private int moveDirection = 0; // -1 = 왼쪽, 1 = 오른쪽
    private bool isMoving = false;

    public GameObject playerPrefab;

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

    private int RealStart = 0;


    enum BlockType { OneByOne, OneByTwo, OneByFour, TwoByTwo }
    private BlockType currentBlockType;

    void Start() => CreateGrids();

    //    private GameObject existingCar = GameObject.FindWithTag("Car");

    public void FunctionDestoryBlock()
    {
        //   ClearCurrentBlock();
        GameObject existingCar = GameObject.FindWithTag("Car");
        if (existingCar != null)
        {
            Destroy(existingCar);
        }
        carSpawnTimer = 0;

    }

    public void FunctionReloadBlock()
    {
        global.CarPass = 1;
        // 기존 Car 오브젝트 삭제
        GameObject existingCar = GameObject.FindWithTag("Car");
        if (existingCar != null)
        {
            Destroy(existingCar);
        }
        carSpawnTimer = 0;

        // currentBlock 삭제 또는 투명화
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

        // previewBlock 삭제 또는 투명화
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
    }

    public void FunctionSpawnBlock()
    {
        //   ClearCurrentBlock();
        if (global.isGameOver == 0)
        {
            global.CarPass = 0;
            carSpawnTimer = 0;
            isGameStart = true;
            SpawnBlock();
            if (moveCar != null)
            {
                Instantiate(moveCar, new Vector2(-10f, 2.3f), Quaternion.identity);
            }
        }
    }

    void CarExistCheck()
    {
        GameObject existingCar = GameObject.FindWithTag("Car");
        // 씬에 Car 태그 오브젝트가 없다면 타이머 작동
        if (existingCar == null && RealStart == 1)
        {
            carSpawnTimer += Time.deltaTime;

            if (carSpawnTimer >= global.carSpawnSpeed)
            {
                FunctionSpawnBlock(); // 원하는 생성 함수 호출
                carSpawnTimer = 0f; // 타이머 초기화
            }
        }
        else
        {
            // Car가 존재하면 타이머 리셋
            carSpawnTimer = 0f;
        }
    }

    void StartCheck()
    {
        RealStart = 1;
    }

    void Update()
    {
        Debug.Log(global.carSpawnSpeedFirst);
        //   Debug.Log(previewBlock);
        if (isGameStart == true)
        {
            CarExistCheck();
        }

        Invoke("StartCheck", global.carSpawnSpeedFirst);

        //if (Input.GetKeyDown(KeyCode.A) && currentBlock == null) SpawnBlock();

        if (global.isGameOver == 0)
        {
            //   if (Input.GetKeyDown(KeyCode.S)) { ClearCurrentBlock(); SpawnBlock(); }
            if (currentBlock == null) return;

            //     if (global.CarPass == 0)
            // {

            /*
                if (Input.GetKeyDown(KeyCode.LeftArrow) && pos.x > 0) pos.x -= 1;
                if (Input.GetKeyDown(KeyCode.RightArrow) && pos.x < (gridsPerRow * 4 - 1)) pos.x += 1;
                */

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

            /*
            if (Input.GetKeyDown(KeyCode.UpArrow) && currentBlockType == BlockType.TwoByTwo)
            {
                rotation = (rotation == 0) ? 90 : 0;
            }
            */
            if (Input.GetKeyDown(KeyCode.UpArrow) && currentBlockType != BlockType.TwoByTwo)
            {
                rotation = (rotation == 0) ? 270 : 0;
            }// rotation = (rotation + 90) % 360;


            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
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
            }
            //  }
        }
        UpdateBlock();
        UpdatePreview();


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
                if (currentBlock != null) Destroy(currentBlock);

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

                /*
                foreach (GameObject b in blockList)
                {
                    if (b != null)
                    {
                        // Rigidbody2D 추가 및 설정
                        if (b.GetComponent<Rigidbody2D>() == null)
                        {
                            Rigidbody2D rb = b.AddComponent<Rigidbody2D>();
                            rb.gravityScale = 3f;
                            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                        }

                        // Collider2D 추가 및 isTrigger 설정
                        Collider2D col = b.GetComponent<Collider2D>();
                        if (col == null)
                        {
                            col = b.AddComponent<BoxCollider2D>();
                        }
                        col.isTrigger = true; // 충돌 무시

                        // FallingBlock 스크립트 추가
                        if (b.GetComponent<FallingBlock>() == null)
                        {
                            b.AddComponent<FallingBlock>();
                        }
                    }
                }
                */
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

    void SpawnBlock()
    {
        global.isBadCar = false;

        int randReal = Random.Range(0, 100);
        int[] choice_car = { 1, 12, 13, 14, 15, 21, 31 };
        global.carNow = choice_car[Random.Range(0, choice_car.Length)];

        if (randReal < global.carBadPer)
        {
            if (global.stageNow == 1) { int[] choice_car2 = { 2, 3, 11 }; global.carNow = choice_car2[Random.Range(0, choice_car2.Length)]; }
            if (global.stageNow == 2) { int[] choice_car2 = { 2, 3, 11 }; global.carNow = choice_car2[Random.Range(0, choice_car2.Length)]; }
            if (global.stageNow == 3) { int[] choice_car2 = { 2, 3, 11 }; global.carNow = choice_car2[Random.Range(0, choice_car2.Length)]; }
            if (global.stageNow == 4) { int[] choice_car2 = { 2, 3, 11 }; global.carNow = choice_car2[Random.Range(0, choice_car2.Length)]; }
        }




        int rand = (int)(global.carNow / 10);
        currentBlockType = (BlockType)rand;

        currentBlock = Instantiate(blockPrefab, GetWorldPosition(pos), Quaternion.identity);
        previewBlock = Instantiate(previewPrefab, GetWorldPosition(pos), Quaternion.identity);

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

        // ▶ 프리뷰 렌더러 처리
        Transform previewChild = previewBlock.transform.Find(activeName);
        if (previewChild != null)
        {
            previewRenderer = previewChild.GetComponent<SpriteRenderer>();
            defaultColor = previewRenderer.color;
            defaultColor.a = 1f;
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
                    21 => "car1x4_01",
                    31 => "car2x2_01",
                    _ => null
                };

                if (!string.IsNullOrEmpty(spriteName))
                {
                    string path = $"_Res_Zuo/Res_Stage/Res_Stage_Car/grid/{spriteName}";
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
                    21 => "car1x4_01",
                    31 => "car2x2_01",
                    _ => null
                };

                if (!string.IsNullOrEmpty(spriteName))
                {
                    string path = $"_Res_Zuo/Res_Stage/Res_Stage_Car/grid/{spriteName}";
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
    }

    void ActivateOnlyChild(GameObject parent, string targetName)
    {
        foreach (Transform child in parent.transform)
            child.gameObject.SetActive(child.name == targetName);
    }
}