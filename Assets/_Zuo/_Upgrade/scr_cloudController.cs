using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_cloudController : MonoBehaviour
{

    StageController stageController;

    public Sprite[] cloudSprites;

    void Start()
    {
        stageController = FindObjectOfType<StageController>();

        // 🔹 자식 "Square" 오브젝트를 찾고 SpriteRenderer 가져오기
        Transform square = transform.Find("Square");
        if (square != null && cloudSprites.Length > 0)
        {
            SpriteRenderer sr = square.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                // 🔹 랜덤 스프라이트 적용
                sr.sprite = cloudSprites[Random.Range(0, cloudSprites.Length)];
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (stageController != null && !stageController.cloudMode)
        {
            Destroy(gameObject);
            return;
        }


        transform.Translate(Vector3.left * 1 * Time.deltaTime);

        // X가 -10보다 작아지면 삭제
        if (transform.position.x < -10f)
        {
            Destroy(gameObject);
        }
    }
}
