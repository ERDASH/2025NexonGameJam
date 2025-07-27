using UnityEngine;

public class FallingBlock : MonoBehaviour
{
    private float destroyY = 10f; // 위로 날아가니까 기준 위로 변경
    private int ON = 0;

    private Vector3 velocity;
    private float angularVelocity;

    void Update()
    {
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
                GetComponent<MoveCarController>().ReloadCar();
                Destroy(gameObject);
            }
        }
    }

    public void FallOn()
    {
        if (ON == 0)
        {
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

    public void FallGone()
    {
        if (ON == 0)
        {
            ON = 3;

            float randomX = Random.Range(-10f, 10f);     // 좌/우 랜덤 방향
            float upwardY = Random.Range(10f, 16f);     // 위로 튀는 힘

            velocity = new Vector3(randomX, upwardY, 0f);
            angularVelocity = Random.Range(-360f, 360f);  // 회전 속도 랜덤
        }
    }
}