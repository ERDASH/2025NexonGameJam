using UnityEngine;

public class Sensor : MonoBehaviour
{
    public bool isOccupied = false;
    public int gridX, gridY;
    public GameObject sensorGrid;

    void Start()
    {
        UpdateGrid();
    }

    private void Update()
    {
        UpdateGrid();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Block"))
        {
            isOccupied = true;
            UpdateGrid();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Block"))
        {
            isOccupied = false;
            UpdateGrid();
        }
    }

    public void UpdateGrid()
    {
        if (sensorGrid != null)
        {
            sensorGrid.SetActive(isOccupied);
        }
    }



    /*
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Block"))
        {
            FallingBlock block = other.GetComponent<FallingBlock>();

            if (block != null)
            {
                // 🔹 블록이 떨어지는 중이면 센서 비활성화
                if (block.ImFalling)
                {
                    isOccupied = false;
                }
                else
                {
                    isOccupied = true;
                }
            }
            else
            {
                // FallingBlock 컴포넌트 없으면 일반 블록으로 간주
                isOccupied = true;
            }

            UpdateGrid();
        }
    }
    */





}
