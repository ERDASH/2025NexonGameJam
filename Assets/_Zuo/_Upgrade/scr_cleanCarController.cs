using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_cleanCarController : MonoBehaviour
{
    private float speed = 8f; // 초당 3 단위 이동
    private float destroyY;  // 삭제 기준 Y 좌표

    void Start()
    {
        // 메인 카메라 기준으로 화면 세로 크기의 1/4 아래를 계산
        Camera cam = Camera.main;
        float camHeight = cam.orthographicSize * 2f;
        float bottomEdge = cam.transform.position.y - cam.orthographicSize;

        destroyY = bottomEdge - (camHeight / 4f); // 화면 아래 1/4 더 내려가면 삭제
    }

    void Update()
    {
        // 아래로 이동
        transform.Translate(Vector2.down * speed * Time.deltaTime);

        // 기준선 아래로 내려가면 삭제
        if (transform.position.y < destroyY)
        {
            Destroy(gameObject);
        }
    }
}
