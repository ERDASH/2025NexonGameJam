using UnityEngine;

public class FallingBlock : MonoBehaviour
{
    private float destroyY = 10f; // 위로 날아가니까 기준 위로 변경
    private int ON = 0;

    private Vector3 velocity;
    private float angularVelocity;

    public bool isPreviewBlock = true;
    public bool ImFalling = false;


    void Update()
    {
        UpdateDepthByAlpha();
        if (ON == 1)
        {
            transform.position += Vector3.down * 9f * Time.deltaTime;
        }
        else if (ON == 2)
        {
            transform.position += velocity * Time.deltaTime;
            transform.Rotate(Vector3.forward, angularVelocity * Time.deltaTime);
        }
        else if (ON == 3)
        {
            transform.position += velocity * Time.deltaTime;
            transform.Rotate(Vector3.forward, angularVelocity * Time.deltaTime);
        }

        if (transform.position.y > destroyY || transform.position.y < -10f)
        {
            if (ON == 3)
            {
                //##DELETE 2025-10-22
                //GetComponent<MoveCarController>().ReloadCar();
                Destroy(gameObject);
            }
        }

        //그리고 삭제하는것도, 주변삭제가아니라 그냥 일자로내려가는것에 부딪히면 삭제로
        //-----------------------------------------
        // DEBUG CODE
        //-----------------------------------------
        /*
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            if (hit.collider != null)
            {
                GameObject clickedObj = hit.collider.gameObject;

                // 클릭된 오브젝트의 자식 중 SpriteRenderer 검사
                SpriteRenderer[] renderers = clickedObj.GetComponentsInChildren<SpriteRenderer>();

                foreach (SpriteRenderer sr in renderers)
                {
                    if (sr != null && Mathf.Approximately(sr.color.a, 1f))
                    {
                        Destroy(clickedObj);
                        break;
                    }
                }
            }
        }
        */
        //-----------------------------------------

    }


    void UpdateDepthByAlpha()
    {
        SpriteRenderer[] srs = GetComponentsInChildren<SpriteRenderer>();
        if (srs.Length == 0) return;

        // 자식 중 하나라도 반투명이면 이 오브젝트 전체를 깊이 10으로
        bool hasFadeSprite = false;

        foreach (var sr in srs)
        {
            float a = sr.color.a;
            if (a > 0.3f && a < 1f)
            {
                hasFadeSprite = true;
                break;
            }
        }

        int order = hasFadeSprite ? 10 : 0;

        foreach (var sr in srs)
            sr.sortingOrder = order;
    }


    public void FallOn()
    {
        if (ON == 0)
        {
            GameManager.Instance.AddLife(global.lifeSubLineBreak);
            ON = 1;
        }
    }

    public void FallOff()
    {
        if (ON == 0)
        {
            ON = 2;

            float randomX = Random.Range(-10f, 10f);     // 좌/우 랜덤 방향
            float upwardY = Random.Range(10f, 16f);     // 위로 튀는 힘

            velocity = new Vector3(randomX, upwardY, 0f);
            angularVelocity = Random.Range(-360f, 360f);  // 회전 속도 랜덤
        }
    }


    /*
    public void FallGone()
    {
        if (ON == 0)
        {
            ImFalling=true;
            ON = 3;

            float randomX = Random.Range(-10f, 10f);     // 좌/우 랜덤 방향
            float upwardY = Random.Range(10f, 16f);     // 위로 튀는 힘

            velocity = new Vector3(randomX, upwardY, 0f);
            angularVelocity = Random.Range(-360f, 360f);  // 회전 속도 랜덤
        }
    }*/


    public void FallGone()
    {
        if (ON == 0)
        {
            ImFalling = true;
            ON = 3;

            float randomX = Random.Range(-10f, 10f);
            float upwardY = Random.Range(10f, 16f);
            velocity = new Vector3(randomX, upwardY, 0f);
            angularVelocity = Random.Range(-360f, 360f);

            // 🔹 지금 겹쳐져 있는 Sensor들 찾아서 isOccupied = false
            Collider2D[] hits = GetComponentsInChildren<Collider2D>();
          //  Debug.Log($"[FallGone] Collider2D 개수 = {hits.Length}");

            foreach (var myCol in hits)
            {
                if (!myCol.enabled) continue;
           //     Debug.Log($"겹쳐져있나? ({myCol.name})");

                ContactFilter2D filter = new ContactFilter2D();
                filter.useTriggers = true;
                filter.NoFilter();

                Collider2D[] overlap = new Collider2D[20];
                int count = myCol.OverlapCollider(filter, overlap);
         //       Debug.Log($"  ↳ Overlap count = {count}");

                for (int i = 0; i < count; i++)
                {
                    Collider2D other = overlap[i];
                    if (other == null) continue;

                    // 🔸 부모 Sensor에 태그가 있으므로, 부모에서 Sensor를 찾는다
                    Sensor s = other.GetComponentInParent<Sensor>();
                    if (s != null)
                    {
              //          Debug.Log($"겹겹 → Sensor: {s.name}");
                        s.isOccupied = false;
                        s.UpdateGrid();
                    }
                }
            }

            // 🔹 자기 콜라이더 꺼서 다른 센서 안 건드리게
            Collider2D selfCol = GetComponent<Collider2D>();
            if (selfCol != null)
                selfCol.enabled = false;
        }
    }


    /*
    public void FallGone()
    {
        if (ON == 0)
        {
            ImFalling = true;
            ON = 3;

            float randomX = Random.Range(-10f, 10f);
            float upwardY = Random.Range(10f, 16f);
            velocity = new Vector3(randomX, upwardY, 0f);
            angularVelocity = Random.Range(-360f, 360f);

            // 🔹 지금 겹쳐져 있는 Sensor들 찾아서 isOccupied = false
            //Collider2D[] hits = GetComponents<Collider2D>();
            Collider2D[] hits = GetComponentsInChildren<Collider2D>();
            Debug.Log($"[FallGone] Collider2D 개수 = {hits.Length}");
            foreach (var myCol in hits)
            {
                Debug.Log("겹쳐져있나?");
                ContactFilter2D filter = new ContactFilter2D();
                filter.useTriggers = true;
                filter.NoFilter();
                Collider2D[] overlap = new Collider2D[10];
                int count = myCol.OverlapCollider(filter, overlap);
                for (int i = 0; i < count; i++)
                {
                    Collider2D other = overlap[i];
                    if (other != null && other.CompareTag("Sensor"))
                    {
                        Debug.Log("겹겹");
                        Sensor s = other.GetComponent<Sensor>();
                        if (s != null)
                        {
                            s.isOccupied = false;
                            s.UpdateGrid();
                        }
                    }
                }
            }

            // 🔹 이제 자기 콜라이더 꺼서 다른 센서 안 건드리게
            Collider2D selfCol = GetComponent<Collider2D>();
            if (selfCol != null)
                selfCol.enabled = false;
        }
    }
    */


    //-----------------------------------------
    // 🔸 StageObj_Breaker의 자식인 Square와 충돌하면 파괴
    //-----------------------------------------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        /*
        GameObject other = collision.gameObject;

        // 1️⃣ 기본 충돌 감지
        Debug.Log($"[부딪힘] {gameObject.name} 이(가) {other.name} 와 충돌함");

        bool hitAlpha1Child = false;
        bool finalHit = false;

        // 2️⃣ 자식 중 alpha=1인 SpriteRenderer가 실제 충돌한 경우
        foreach (ContactPoint2D contact in collision.contacts)
        {
            Transform hitTransform = contact.collider.transform;

            SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sr in renderers)
            {
                if (sr != null && Mathf.Approximately(sr.color.a, 1f))
                {
                    if (sr.gameObject == hitTransform.gameObject)
                    {
                        Debug.Log($"[진짜 부딪힘] 알파 1 자식 {sr.name} 이(가) {other.name} 와 충돌함");
                        hitAlpha1Child = true;
                        break;
                    }
                }
            }
        }

        // 3️⃣ 부딪힌 대상이 StageObj_Breaker/Square 일 경우
        Transform parent = other.transform.parent;
        if (parent != null && parent.name == "StageObj_Breaker" && other.name == "Square")
        {
            Debug.Log($"[최종부딪힘] {gameObject.name} 이(가) StageObj_Breaker/Square 와 충돌함");
            finalHit = true;
        }

        // 🔹 필요 시 파괴 조건까지 넣기
        if (hitAlpha1Child && finalHit)
        {
            Destroy(gameObject);
        }
        */
    }




}