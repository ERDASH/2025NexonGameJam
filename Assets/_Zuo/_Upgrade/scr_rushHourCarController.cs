using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_rushHourCarController : MonoBehaviour
{
    public int carNumber;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(5 * Time.deltaTime, 0, 0);

        if (transform.position.x > 10000f)
        {
            CarDestroy();
        }
    }

    void CarDestroy()
    {
//        Debug.Log("[RushHourCar] 차량 제거됨");
//        Destroy(gameObject); // 차량 오브젝트 제거
    }
}
