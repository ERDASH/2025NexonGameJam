using UnityEngine;
using UnityEngine.UI;

public class Sensor : MonoBehaviour
{
    public bool isOccupied = false;
    public int gridX, gridY;
    public Text sensorText;

    void Start()
    {
        UpdateText();
    }

    private void Update()
    {
        UpdateText();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Block"))
        {
            isOccupied = true;
            UpdateText();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Block"))
        {
            isOccupied = false;
            UpdateText();
        }
    }

    void UpdateText()
    {
        if (sensorText != null)
        {
            sensorText.text = isOccupied ? "" : "";
            sensorText.color = isOccupied ? Color.red : Color.green;
        }
    }
}