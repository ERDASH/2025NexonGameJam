using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;   

public class WorkSheet : MonoBehaviour
{
    public Button Button01;
    public Button Button02;
    public Button Button03;
    public Button Button04;
    public Button Button05;

    void OnEnable()
    {
        Button01.interactable = false;
        Button02.interactable = false;
        Button03.interactable = false;
        Button04.interactable = false;
        Button05.interactable = false;

        int stage = global.stage;
        // stage 값에 따라 버튼 순차적으로 활성화
        if (stage >= 2)
        {
            Transform check = Button01.transform.Find("Check");
            check.gameObject.SetActive(true);
        }
        if (stage >= 3)
        {
            Transform check = Button02.transform.Find("Check");
            check.gameObject.SetActive(true);
        }
        if (stage >= 4)
        {
            Transform check = Button03.transform.Find("Check");
            check.gameObject.SetActive(true);
        }
        if (stage >= 5)
        {
            Transform check = Button04.transform.Find("Check");
            check.gameObject.SetActive(true);
        }
        if (stage >= 6)
        {
            Transform check = Button05.transform.Find("Check");
            check.gameObject.SetActive(true);
        }
    }
}
